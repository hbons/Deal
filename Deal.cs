/*
 * Deal.
 *
 * A very simple but fun card game for mobile devices.
 *
 * Authored By Hylke Bons  <hylke.bons@intel.com>
 *
 * Copyright (C) 2009 Intel
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License version 2 as published by the Free Software Foundation.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using Clutter;

public class Deal {

	static int[] CardPositionsX;
	static Stage Stage;
	static Deck Deck;
	static Hand PlayerHand;
	static Hand OpponentHand;
	static StepButton StepButton;
	static BetButton BetButton;
	static Stack Stack;
	static int Bet;

	static void Main () {

		Clutter.Application.Init ();

		CardPositionsX = new int[5];
		CardPositionsX[0] = 100;
		CardPositionsX[1] = 250;
		CardPositionsX[2] = 400;
		CardPositionsX[3] = 550;
		CardPositionsX[4] = 700;

		Deck = new Deck ();
		Stack = new Stack ();

		Stage = Stage.Default;
		Stage.Title = "Deal!";
		Stage.UserResizable = false;
		Stage.SetSize (800, 480);
		Stage.Add (new Texture ("Pixmaps/Table.png"));
		Stage.KeyPressEvent += HandleKeyPress;

		BetButton = new BetButton ();
		BetButton.ButtonPressEvent += IncreaseBet;
		Stage.Add (BetButton);

		StepButton = new StepButton ();
		StepButton.ButtonPressEvent += NextStep;
		Stage.Add (StepButton);

		PlayerHand   = new Hand (Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw ());
		OpponentHand = new Hand (Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw ());

		for (int i = 0; i < 5; i++) {
			PlayerHand.GetCard (i).SetPosition (CardPositionsX[i], 300);
			OpponentHand.GetCard (i).SetPosition (CardPositionsX[i], 100);
			PlayerHand.GetCard (i).ButtonPressEvent += ToggleSelection;
			Stage.Add (PlayerHand.GetCard (i));
			Stage.Add (OpponentHand.GetCard (i));
		}


		Timeline timeline2 = new Timeline (500);
		timeline2.Loop = false;
//		timeline2.NewFrame += SlideIn;
		timeline2.Start ();	



/*
		Rectangle MatchBackground = new Rectangle ();
		MatchBackground.Color = new Clutter.Color (0x35, 0x3c, 0x41, 0xff);
		MatchBackground.SetSize (800, 60);
		MatchBackground.SetPosition (0, 70);
		Stage.Add (MatchBackground);
		Text MatchType = new Text ("Droid Sans Bold 21", "Full House!");
		MatchType.SetPosition (400, 100);
		MatchType.SetAnchorPoint (MatchType.Width / 2 + 1, MatchType.Height / 2);
		MatchType.SetColor (White);
		Stage.Add (MatchType);

*/



		Texture Coin = new Texture ("Pixmaps/Coin.png");
		Coin.SetPosition (35, 408);
		
		Text ScoreText = new Text ("Droid Sans Bold 21", "" + Stack.GetAmount());
		ScoreText.SetPosition (95, 413);
		ScoreText.SetColor (new Clutter.Color (0xff, 0xff, 0xff, 0xff));
		Stage.Add (ScoreText);
		Stage.Add(Coin);

		Timeline timeline = new Timeline (1800);
		timeline.Loop = false;
		timeline.NewFrame += Spin;
		timeline.Start ();

		Stage.ShowAll();
		Clutter.Application.Run (); 

  }


	static void NextStep (object o, ButtonPressEventArgs args) {
		Console.WriteLine ("Signal Deal");

		if (StepButton.State == 0) {
			for (int i = 0; i < 5; i++) {
				if (PlayerHand.GetCard (i).Selected) {
					PlayerHand.GetCard (i).HideAll ();
					PlayerHand.Replace (i, Deck.Draw ());
					PlayerHand.GetCard (i).SetPosition (CardPositionsX[i], 300);
					Stage.Add (PlayerHand.GetCard (i));

				}
			}
			StepButton.DrawState ();
			BetButton.Hide ();
			StepButton.Hide ();
		
		
		int [] CardQuantities = new int[6];
		
		for (int i = 0; i < 5; i++) {
			CardQuantities [PlayerHand.GetCard (i).Type]++;
		}

		int j = 0;
		for (int i = 0; i < 6; i++) {
			if (CardQuantities [i] >= j)
				j = i;
		}
		
		string[] Types = new string[6];
		Types[0] = "Blue"; Types[1] = "Purple"; Types[2] = "Red"; Types[3] = "Green"; Types[4] = "Orange"; Types[5] = "Marine";
		Console.WriteLine ("Type: " + Types[j] + ", Count: " + CardQuantities [j]);
		
		if (CardQuantities [j] == 5)
			Console.WriteLine ("Five of a kind!");

		if (CardQuantities [j] == 4)
			Console.WriteLine ("Four of a kind");

		if (CardQuantities [j] == 3) {
			CardQuantities [j] = 0;
			if (InArray (CardQuantities, 2)) {
				Console.WriteLine ("Full House");
			} else {
				Console.WriteLine ("Three of a kind");
			}
		}

		if (CardQuantities [j] == 2) {
			CardQuantities [j] = 0;
			if (InArray (CardQuantities, 2)) {
				Console.WriteLine ("Two Pair");
			} else {
				Console.WriteLine ("One Pair");
			}
		}
						
		if (CardQuantities [j] == 1)
			Console.WriteLine ("Single");
			
				
		} else if (StepButton.State == 1) {
		


		}

	}
	
	
		public static bool InArray (int [] a, int b) {
			for (int i = 0; i < a.Length; i++) {
				if (a[i] == b)
					return true;
			}
			return false;
		}

	static void Spin (object o, NewFrameArgs args) {
		for (int i = 0; i < 5; i++) {
			if (args.FrameNum > 900)
				OpponentHand.GetCard(i).SetFromFile ("Pixmaps/Card-Back.png");
		 	OpponentHand.GetCard(i).SetRotation (RotateAxis.Y, args.FrameNum/10, 0, 0, 0);
		}
	}

	static void IncreaseBet (object o, ButtonPressEventArgs args) {
		Bet += 1;
		if (Bet == 3)
			BetButton.Hide ();
	}


/*
	static void SlideIn (object o, NewFrameArgs args) 
	{
		for (int i = 0; i < 5; i++) {
	 		PlayerHand.GetCard(i).SetPosition( CardPositionsX[i], -200 + args.FrameNum);
}

	}
*/

	static void HandleKeyPress (object o, KeyPressEventArgs args) {
		 Clutter.Application.Quit ();
	}

	public static void ToggleSelection (object o, ButtonPressEventArgs args) {

		Card Card = (Card) o;		
		float x, y;
		Card.GetPosition(out x, out y);	
		if (Card.Selected) {
			for(float h = y; h < y + 35; h += (float) 0.05)
				Card.SetPosition(x, (int)h);
			Card.Selected = false;
		} else {
			for(float h = y; h > y - 35; h -= (float) 0.05)
				Card.SetPosition(x, (int)h);
			Card.Selected = true;
		}

		if (PlayerHand.NumberOfCardsSelected () >= 1)
			StepButton.DrawState ();
		else
			StepButton.HoldState ();

	}

}

public class Card : Texture {

	public bool Selected;
	public int Type;

	public Card (int type) {
		Type = type;
		Selected = false;
		SetSize (130, 150);
		SetAnchorPoint (65, 75);
		SetFromFile("Pixmaps/" + (type + 1) + ".png");
		Reactive = true;
	}

}

public class Deck {

	private Card [] Cards;
	private int j;

	public Deck () {
		Cards = new Card [30];
		Random r = new Random (Environment.TickCount);
		j = 0;
		for (int i = 0; i < 30; i++)
			Cards [i] = new Card (r.Next (6));
	}

	public Card Draw () {
		Card Card = Cards [j];
		Cards [j] = null;
		j++;
		return Card;
	}

	public void Reset () {
		Cards = new Card [30];
		Random r = new Random (Environment.TickCount);
		j = 0;
		for (int i = 0; i < 30; i++)
			Cards [i] = new Card (r.Next (6));
		
	}

}

public class Hand {

	private Card [] Cards;

	public Hand (Card a, Card b, Card c, Card d, Card e) {
		Cards = new Card [5];
		Cards [0] = a;
		Cards [1] = b;
		Cards [2] = c;
		Cards [3] = d;
		Cards [4] = e;
	}

	public void Replace (int Position, Card Card) {
		Cards [Position] = Card;
	}

	public Card GetCard (int Position) {
		return Cards [Position];
	}

	public int NumberOfCardsSelected () {
		int j = 0;
		for (int i = 0; i < 5; i++) {
			if (Cards [i].Selected)
				j++;
		}
		return j;
	}

}

public class Stack {

	private int Amount;  

	public Stack () {
		Amount = 100;
	}

	public void Increase (int amount) {
		Amount += amount;
	}

	public void Decrease (int amount) {
		Amount -= amount;
	}

	public int GetAmount () {
		return Amount;
	}

}

public class StepButton : Texture {

	public int State;

	public StepButton () {
		State = 0;
		SetSize (132, 61);
		SetPosition (635, 400);
		SetFromFile ("Pixmaps/Button-Hold.png");
		Reactive = true;
	}
	
	public void HoldState () {
			State = 0;
			SetFromFile ("Pixmaps/Button-Hold.png");
	}

	public void DrawState () {
			State = 0;
			SetFromFile ("Pixmaps/Button-Draw.png");
	}
	
}

public class BetButton : Texture {

	public int Presses;

	public BetButton () {
		Presses = 0;
		SetSize (132, 61);
		SetPosition (485, 400);
		SetFromFile ("Pixmaps/Button-Raise.png");
		Reactive = true;
		ButtonPressEvent += Pressed;
	}
	
	public void Pressed (object o, ButtonPressEventArgs args) {
		Presses++;
	}

}
