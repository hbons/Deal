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

	static Hand PlayerHand;
	static int[] CardPositionsX;
	static Stage Stage;
	static Deck Deck;
	static StepButton StepButton;
	static BetButton BetButton;
	static Stack Stack;
	static int Step;

	static void Main () {

		Clutter.Application.Init ();
		Step = 1;
		CardPositionsX = new int[5];
		CardPositionsX[0] = 100;
		CardPositionsX[1] = 250;
		CardPositionsX[2] = 400;
		CardPositionsX[3] = 550;
		CardPositionsX[4] = 700;
		
		Clutter.Color White = new Clutter.Color (0xff, 0xff, 0xff, 0xff);

		// Stage
		Stage = Stage.Default;	 
		Stage.SetSize (800, 480);
		Stage.Title = "Deal";
		Stage.KeyPressEvent += HandleKeyPress;

		// Main background
		Texture Background = new Texture ("Pixmaps/Table.png");
		Background.SetSize (800, 480);
		Stage.Add (Background);

		StepButton = new StepButton ();
		StepButton.ButtonPressEvent += NextStep;
		Stage.Add (StepButton);

		BetButton = new BetButton ();
		Stage.Add (BetButton);

		Stack = new Stack ();
		Stage.Add (Stack);

		Deck = new Deck ();
		PlayerHand = new Hand (Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw (), Deck.Draw ());

		for (int i = 0; i < 5; i++) {
			PlayerHand.GetCard (i).SetPosition (CardPositionsX[i], 300);
			Stage.Add (PlayerHand.GetCard (i));
		}


			Timeline timeline2 = new Timeline (500);
			timeline2.Loop = false;
//			timeline2.NewFrame += SlideIn;
			timeline2.Start ();	




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

		Timeline timeline = new Timeline (2700);
		timeline.Loop = true;
		timeline.NewFrame += Spin;
		timeline.Start ();

		Stage.ShowAll();
		Clutter.Application.Run (); 

  }


	static void NextStep (object o, ButtonPressEventArgs args) {
		Console.WriteLine ("Signal Deal");

		if (Step == 1) {
			for (int i = 0; i < 5; i++) {
				if (PlayerHand.GetCard (i).Selected) {
					PlayerHand.GetCard (i).HideAll ();
					PlayerHand.Replace (i, Deck.Draw ());
					PlayerHand.GetCard (i).SetPosition (CardPositionsX[i], 300);
					Stage.Add (PlayerHand.GetCard (i));
				}
			}
			//Step++;
			
		} else if (Step == 2) {

			// Check hand score

		

		}

	}

	static void Spin (object o, NewFrameArgs args) 
	{
		for (int i = 0; i < 5; i++)
	 		PlayerHand.GetCard(i).SetRotation (RotateAxis.Y, args.FrameNum / 3, 0, 0, 0);
	}


/*
	static void SlideIn (object o, NewFrameArgs args) 
	{
		for (int i = 0; i < 5; i++) {
	 		PlayerHand.GetCard(i).SetPosition( CardPositionsX[i], -200 + args.FrameNum);
}

	}
*/

	static void HandleKeyPress (object o, KeyPressEventArgs args)
	{
		 Clutter.Application.Quit ();
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
		ButtonPressEvent += ToggleSelection;
		Reactive = true;

	}
	
	public void ToggleSelection (object o, ButtonPressEventArgs args) {
		float x, y;
		GetPosition(out x, out y);
		if (Selected) {
			for(float i = y; i < y + 100; i += (float) 0.05)
				SetPosition(x, (int)i);
			Selected = false;
		} else {
			for(float i = y; i > y - 100; i -= (float) 0.05)
				SetPosition(x, (int)i);
			Selected = true;
		}
	}

}

public class StepButton : Texture {

	public int State;

	public StepButton () {
		State = 0;
		SetSize (100, 50);
		SetPosition (675, 415);
		SetFromFile ("Pixmaps/back.png");
		Reactive = true;
	}
	
	public void ChangeState () {
		if (State == 0)
			State = 1;
		else
			State = 0;
	}

}

public class BetButton : Texture {

	public int Presses;

	public BetButton () {
		Presses = 0;
		SetSize (100, 50);
		SetPosition (510, 415);
		SetFromFile ("Pixmaps/back.png");
		Reactive = true;
		ButtonPressEvent += Pressed;
	}
	
	public void Pressed (object o, ButtonPressEventArgs args) {
		Presses++;
	}

}





public class Deck {

	private Card [] Cards;
	private int j;

	public Deck () {
		Cards = new Card [25];
		Random r = new Random (Environment.TickCount);
		j = 0;
		for (int i = 0; i < 25; i++) {
			Cards [i] = new Card (r.Next (5));
		}
	}

	public Card Draw () {
		Card Card = Cards [j];
		Cards [j] = null;
		j++;
		return Card;
	}

//	public void Insert (int Card) {
//		Cards[Card]++;
//	}

//	public void Reset () {
//	}

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

}

public class Stack : Group {

	private int Amount;  

	public Stack () {
		Amount = 1000;

		Texture Coin = new Texture ("Pixmaps/Coin.png");
		Coin.SetPosition (20, 410);
		Add (Coin);
		Text ScoreText = new Text ("Droid Sans Bold 21", "" + Amount);
		ScoreText.SetPosition (80, 415);
		ScoreText.SetColor (new Clutter.Color (0xff, 0xff, 0xff, 0xff));
		Add (ScoreText);

	}

	public void Increase (int amount) {
		Amount += amount;
	}

	public void Decrease (int amount) {
		Amount -= amount;
	}

	public int getAmount () {
		return Amount;
	}

}
