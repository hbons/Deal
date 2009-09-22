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

	static Texture[] Card;
	static int[] CardPositionsX;
	static Stage Stage;

	static void Main () {

		Clutter.Application.Init ();

		CardPositionsX = new int[5];
		CardPositionsX[0] = 100;
		CardPositionsX[1] = 250;
		CardPositionsX[2] = 400;
		CardPositionsX[3] = 550;
		CardPositionsX[4] = 700;
		Card = new Texture[10];

		Clutter.Color White = new Clutter.Color (0xff, 0xff, 0xff, 0xff);

		Deck Deck = new Deck ();
		Stack Stack = new Stack ();

		// Stage
		Stage = Stage.Default;	 
		Stage.SetSize (800, 480);
		Stage.Title = "Deal";
		Stage.KeyPressEvent += HandleKeyPress;

		// Main background
		Texture Background = new Texture ("Pixmaps/Table.png");
		Background.SetSize (800, 480);
		Stage.Add (Background);

		// Score
		Texture Coin = new Texture ("Pixmaps/Coin.png");
		Coin.SetPosition (20, 410);
		Stage.Add (Coin);
		Text ScoreText = new Text ("Droid Sans Bold 21", "" + Stack.getAmount ());
		ScoreText.SetPosition (80, 415);
		ScoreText.SetColor (White);
		Stage.Add (ScoreText);

		// Deal button
/*		Rectangle DealButton = new Rectangle ();
		DealButton.Color = new Clutter.Color (0x3f, 0x5d, 0x00, 0xff);
		DealButton.SetSize (166, 82);
		DealButton.SetPosition (800 - 4 - 166, 480 - 4 - 82);

		Stage.Add (DealButton);
*/
		Text DealText = new Text ("Droid Sans Bold 21", "Deal");
		DealText.SetPosition (675, 415);
		DealText.SetColor (White);
		Stage.Add (DealText);

		// Bet button
/*		Rectangle BetButton = new Rectangle ();
		BetButton.Color = new Clutter.Color (0x3f, 0x5d, 0x00, 0xff);
		BetButton.SetSize (166, 82);
		BetButton.SetPosition (460, 394);
		Stage.Add (BetButton);
*/
		Text BetText = new Text ("Droid Sans Bold 21", "Bet");
		BetText.SetPosition (510, 415);
		BetText.SetColor (White);
		Stage.Add (BetText);

		for (int i = 0; i < 5; i++) {
			Card[i] = new Texture ("Pixmaps/" + (Deck.Draw () + 1) + ".png");
			Card[i].SetSize (130, 150);
			if (i < 5)
				Card[i].SetPosition (CardPositionsX[i], -200);
			else
				Card[i].SetPosition (-200, 280);

			Card[i].SetAnchorPoint (65, 75);
			Stage.Add (Card[i]);

			Timeline timeline2 = new Timeline (500);
			timeline2.Loop = false;
			timeline2.NewFrame += SlideIn;
			timeline2.Start ();	

			Card[i].ButtonPressEvent += HandleButtonPress;

		}


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

		Stage.ButtonPressEvent += HandleButtonPressEvent;


		Timeline timeline = new Timeline (2700);
		timeline.Loop = true;
		timeline.NewFrame += Spin;
		timeline.Start ();

		Stage.ShowAll();
		Clutter.Application.Run (); 

  }

	static void Spin (object o, NewFrameArgs args) 
	{
		for (int i = 0; i < 5; i++) {
	 		Card[i].SetRotation (RotateAxis.Y, args.FrameNum / 3, 0, 0, 0);
		}

	}


	static void SlideIn (object o, NewFrameArgs args) 
	{
		for (int i = 0; i < 5; i++) {
	 		Card[i].SetPosition( CardPositionsX[i], -200 + args.FrameNum);
		}

	}

	static void HandleButtonPressEvent (object o, ButtonPressEventArgs args) 
	{
		Actor actor = Stage.Default.GetActorAtPos (Clutter.PickMode.All, args.Event.X, args.Event.Y);
	//	float x, y;
	//	actor.GetPosition(out x, out y);
		Console.WriteLine ((float)args.Event.X + ", " + (float)args.Event.Y);
		
	}

	static void HandleButtonPress (object o, ButtonPressEventArgs args)
	{
		Actor actor = Stage.Default.GetActorAtPos (Clutter.PickMode.All, args.Event.X, args.Event.Y);
		float x, y;
		actor.GetPosition(out x, out y);
		Console.WriteLine (x + " - " + y);

	}

	static void HandleKeyPress (object o, KeyPressEventArgs args)
	{
		 Clutter.Application.Quit ();
	}

}


public class Deck {

	private int[] Cards;

	public Deck () {
		Cards = new int[5];
	}

	public int Draw () {
		Random random = new Random (Environment.TickCount);
		int Card = random.Next (5);
		Cards[Card]--;
		return Card;
	}

	public void Insert (int Card) {
		Cards[Card]++;
	}

	public void Reset () {
		Cards[0] = 5;	// Girls
		Cards[1] = 5;	// Penguins
		Cards[2] = 5;	// Cats
		Cards[3] = 5;	// Birds
		Cards[4] = 5;	// Hearts
	}

}

public class Hand {

	private int[] Cards;

	public Hand () {
		Cards = new int[5];
	}

	public void Remove (int Card) {
		Cards[Card]--;
	}

	public void Insert (int Card) {
		Cards[Card]++;
	}

	public void Empty () {
		Cards = new int[5];
	}

}

public class Stack {

	private int Amount;  

	public Stack () {
		Amount = 1000;
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
