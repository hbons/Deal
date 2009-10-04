using Clutter;
using GLib;
using System;

public class Deal
{

	static int [] CardPositionsX = { 100, 250, 400, 550, 700 };
	static int [] CoinPositionsX = { 225, 250, 275, 300, 325 };
	static Stage Stage;
	static Deck Deck;
	static Hand PlayerHand;
	static Hand OpponentHand;
	static Stack Stack;
	static int Bet;
	static StepButton StepButton;
	static BetButton BetButton;
	static DealButton DealButton;
	static Text ScoreText;
	static Coin [] Coins;

	static void Main ()
	{

		Clutter.Application.Init ();

		Stage = new Stage ();
		Stage.Title = "Deal!";
		Stage.Add (new Texture ("Pixmaps/Table.png"));
		Stage.SetSize (800, 480);
		Stage.KeyPressEvent += HandleKeyPress;

		Texture C = new Texture ("Pixmaps/Coin.png");
		C.SetSize (50, 50);
		C.SetPosition (35, 405);
		Stage.Add (C);

		Bet = 0;
		BetButton = new BetButton ();
		BetButton.ButtonPressEvent += IncreaseBet;
		Stage.Add (BetButton);

		DealButton = new DealButton ();
		DealButton.ButtonPressEvent += NewGame;
		Stage.Add (DealButton);

		StepButton = new StepButton ();
		StepButton.ButtonPressEvent += NextStep;
		Stage.Add (StepButton);
 
		Stack = new Stack ();
		Stack.Decrease (20);
		ScoreText = new Text ("Droid Sans Bold 21", "" + Stack.GetAmount());
		ScoreText.SetColor (new Clutter.Color (0xff, 0xff, 0xff, 0xff));
		ScoreText.SetPosition (100, 413);
		Stage.Add (ScoreText);

		Coins = new Coin [5];
		Coins [0] = new Coin ();
		Coins [1] = new Coin ();
		Coins [2] = new Coin ();
		Coins [3] = new Coin ();
		Coins [4] = new Coin ();
		for (int i = 0; i < 5; i++) {
			Coins [i].SetPosition (35, 405);
			Stage.Add (Coins [i]);
		}

		Deck = new Deck ();

		PlayerHand   = new Hand (Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw ());
		OpponentHand = new Hand (Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw ());

		SetupAnimation ();

		Stage.ShowAll();

		Clutter.Application.Run (); 

	}

	static void NewGame (object o, ButtonPressEventArgs args)
	{

		Bet = 0;

		DealButton.Opacity = 0;

		BetButton.Opacity = 255;
		BetButton.Reactive = true;
		BetButton.Show ();

		StepButton.HoldState ();
		StepButton.Show ();

		// Needs to be replaced with "return to deck" animation
		for (int i = 0; i < 5; i++) {
			PlayerHand.GetCard (i).Hide ();
			OpponentHand.GetCard (i).Hide ();
		}

		Deck = new Deck ();

		PlayerHand   = new Hand (Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw ());
		OpponentHand = new Hand (Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw ());

		SetupAnimation ();

	}

	static void SetupAnimation ()
	{

		for (int i = 0; i < 5; i++)
		{
			PlayerHand.GetCard (i).ButtonPressEvent += ToggleSelection;
			Stage.Add (PlayerHand.GetCard (i));
			Stage.Add (OpponentHand.GetCard (i));
			OpponentHand.GetCard (i).TurnToBack ();
			AnimateProperty (PlayerHand.GetCard (i),   1000, "y", 300,                (uint) (i * 250));
			AnimateProperty (PlayerHand.GetCard (i),   1000, "x", CardPositionsX [i], (uint) (i * 250));
			AnimateProperty (OpponentHand.GetCard (i), 500,  "y", 100,                (uint) 5250);
			AnimateProperty (OpponentHand.GetCard (i), 500,  "x", CardPositionsX [i], (uint) 1750);
		}

		for (int i = 0; i < 2; i++)
			AnimateProperty (Coins [i], 500, "x", CoinPositionsX[i], (uint) (i * 250));	

	}

	static void NextStep (object o, ButtonPressEventArgs args)
	{

		int j = 0;
		for (int i = 0; i < 5; i++)
		{
			// Replace the selected cards
			if (PlayerHand.GetCard (i).Selected) {
				j++;
			  PlayerHand.GetCard (i).TurnToBack ();
			  // Remove card from table
				AnimateProperty (PlayerHand.GetCard (i), 1000, "y", -200, 250);
				// Get a new card
				PlayerHand.Replace (i, Deck.Draw ());
				PlayerHand.GetCard (i).SetPosition (400, -100);
				// Return new card to the right position
				AnimateProperty (PlayerHand.GetCard (i), 1000, "y", 300,                (uint) (750 + j * 250));
				AnimateProperty (PlayerHand.GetCard (i), 1000, "x", CardPositionsX [i], (uint) (750 + j * 250));
				Stage.Add (PlayerHand.GetCard (i));
			}

		}

		for (int i = 0; i < 5; i++)
			OpponentHand.GetCard (i).TurnToFront ((uint) (1000 + j * 500));

		Timeline [] Timelines = new Timeline [5];
		for (int i = 0; i < 4; i++)
			 Timelines [i] = AnimateProperty (Coins [i], 500, "x", 35, (uint) ((1500 + (j * 500)) + (i * 250)));

		Timeline Timeline1 = AnimateProperty (Coins [4], 500, "x", 35, (uint) ((1500 + (j * 500)) + (4 * 250)));
		Timeline1.Completed += delegate { DealButton.Opacity = 255; };

		StepButton.SwapState ();
		BetButton.Hide ();
		StepButton.Hide ();

		foreach (Card Card in PlayerHand.Cards)
			Card.Reactive = false;

	}

	public static void ToggleSelection (object o, ButtonPressEventArgs args)
	{

		Card Card = (Card) o;
		float x, y;
		Card.GetPosition(out x, out y);	
		if (Card.Selected) {
			AnimateProperty (Card, 200, "y", y + 35, 0); // Deselect the card
			Card.Selected = false;
		} else {
			AnimateProperty (Card, 200, "y", y - 35, 0); // Select the card
			Card.Selected = true;
		}

		if (PlayerHand.NumberOfCardsSelected () >= 1)
			StepButton.SwapState ();
		else
			StepButton.HoldState ();

	}

	static void IncreaseBet (object Object, ButtonPressEventArgs ButtonPressEventArgs)
	{
		if (++Bet == 3) {
			Timeline Timeline = new Timeline (255);
			Timeline.NewFrame += delegate (object o, NewFrameArgs args) { 
			BetButton.Opacity = (byte) (255 - args.FrameNum); };
			Timeline.Start ();
			BetButton.Reactive = false;
		}
		AnimateProperty (Coins [Bet + 1], 500, "x", CoinPositionsX [Bet + 1], 0);
		Stack.Decrease (10);
		ScoreText.Value = "" + Stack.GetAmount ();
	}

	static void HandleKeyPress (object o, KeyPressEventArgs args)
	{
		 Clutter.Application.Quit ();
	}

	public static Timeline AnimateProperty (Actor Actor, uint Time, string Property, float To, uint Delay)
	{
		Timeline Timeline = new Timeline (Time);
		Timeline.Delay = Delay;
		string [] Properties = new string[1];
		Properties [0] = Property;
		Actor.AnimateWithTimelinev (6, Timeline, Properties, new GLib.Value ((float) To));
		return Timeline;
	}

	public static bool InArray (int [] a, int b)
	{
		for (int i = 0; i < a.Length; i++)
			if (a [i] == b)
				return true;
		return false;
	}

}

public class Card : Texture
{

	public bool Selected;
	public int Type;
	private Timeline Timeline;

	public Card (int type)
	{
		Type = type;
		Selected = false;
		SetPosition (400, -100);
		SetSize (130, 152);
		SetAnchorPoint (65, 76);
		Reactive = true;
		FrontSide ();
	}

	private void BackSide ()
	{
		SetFromFile("Pixmaps/Card-Back.png");
	}

	private void FrontSide ()
	{
		SetFromFile("Pixmaps/" + Type + ".png");
	}

	public void TurnWithoutTexture ()
	{
			SetRotation(RotateAxis.Y, 180, 0, 0, 0);
	}

	public void TurnToBack ()
	{
		Timeline = new Timeline (180);
		Timeline.NewFrame += delegate (object o, NewFrameArgs args) { 
			SetRotation(RotateAxis.Y, args.FrameNum, 0, 0, 0);
			if (args.FrameNum > 90)
				BackSide (); };
		Timeline.Start ();
	}

	public void TurnToFront ()
	{
		Timeline = new Timeline (180);
		Timeline.NewFrame += delegate (object o, NewFrameArgs args) { 
			SetRotation(RotateAxis.Y, args.FrameNum, 0, 0, 0);
			if (args.FrameNum > 90)
				FrontSide ();
		};
		Timeline.Start ();
	}

	public void TurnToFront (uint Delay)
	{
		Timeline = new Timeline (180);
		Timeline.Delay = Delay;
		Timeline.NewFrame += delegate (object o, NewFrameArgs args) { 
			SetRotation(RotateAxis.Y, args.FrameNum, 0, 0, 0);
			if (args.FrameNum > 90)
				FrontSide ();
		};
		Timeline.Start ();
	}
	
	public void TurnToBack (uint Delay)
	{
		Timeline = new Timeline (180);
		Timeline.Delay = Delay;
		Timeline.NewFrame += delegate (object o, NewFrameArgs args) { 
			SetRotation(RotateAxis.Y, args.FrameNum, 0, 0, 0);
			if (args.FrameNum > 90)
				FrontSide ();
		};
		Timeline.Start ();
	}

}

public class Deck
{

	private Card [] Cards;
	private int CardPointer;

	public Deck ()
	{
		Reset ();
	}

	public void Reset ()
	{
		Cards = new Card [30];
		CardPointer = -1;
		int i = 0;
		while (i < Cards.Length)
		{
			for (int k = 0; k < 6; k++)
			{
				Cards [i] = new Card (k);
				i++;
			}
		}
		Shuffle ();
	}

	public void Shuffle ()
	{
		Random r = new Random ();
		int first, second;
		Card tmp;
		for (int i = 0; i < 1000; i++)
		{
			first = r.Next (Cards.Length);
			second = r.Next (Cards.Length);
			tmp = Cards [first];
			Cards [first] = Cards [second]; 
			Cards [second] = tmp;
		}
	}

	public Card Draw ()
	{
		return Cards [++CardPointer];
	}

}

public class Hand
{

	public Card [] Cards;

	public Hand (Card a, Card b, Card c, Card d, Card e)
	{
		Cards = new Card [5];
		Cards [0] = a; Cards [1] = b; Cards [2] = c; Cards [3] = d; Cards [4] = e;
	}

	public void Replace (int Position, Card Card)
	{
		Cards [Position] = Card;
	}

	public Card GetCard (int Position)
	{
		return Cards [Position];
	}

	public int NumberOfCardsSelected ()
	{
		int j = 0;
		foreach (Card Card in Cards)
			if (Card.Selected)
				j++;
		return j;
	}

}

public class Stack
{

	private int Amount;  

	public Stack ()
	{
		Amount = 1000;
	}

	public void Increase (int i)
	{
		Amount += i;
	}

	public void Decrease (int i)
	{
		Amount -= i;
	}

	public int GetAmount ()
	{
		return Amount;
	}

}

public class Coin : Texture
{
	public Coin ()
	{
		SetSize (50, 50);
		SetFromFile ("Pixmaps/Coin.png");
		SetPosition (35, 405);
	}
}

public class StepButton : Texture
{

	public int State;

	public StepButton ()
	{
		SetSize (132, 61);
		SetPosition (635, 400);
		HoldState ();
		Reactive = true;
	}

	public void HoldState ()
	{
			State = 0;
			SetFromFile ("Pixmaps/Button-Hold.png");
	}

	public void SwapState ()
	{
			State = 0;
			SetFromFile ("Pixmaps/Button-Swap.png");
	}

}

public class BetButton : Texture
{
	public BetButton ()
	{
		SetSize (132, 61);
		SetPosition (485, 400);
		SetFromFile ("Pixmaps/Button-Raise.png");
		Reactive = true;
	}
}

public class DealButton : Texture
{
	public DealButton ()
	{
		SetSize (132, 61);
		SetPosition (635, 400);
		SetFromFile ("Pixmaps/Button-Deal.png");
		Reactive = true;
		Opacity = 0;
	}
}
