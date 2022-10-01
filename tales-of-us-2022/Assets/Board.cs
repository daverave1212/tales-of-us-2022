using Godot;
using System;

public class Board : Node2D
{
	
	Delay D;
	Sounder S;
	Sounder SU;
	Sounder SM;
	
	public override void _Ready()
	{
		D = (Delay) GetNode("/root/World/Delay");
		S = (Sounder) GetNode("/root/World/Sounder");
		SU = (Sounder) GetNode("/root/World/SounderSuccess");
		SM = (Sounder) GetNode("/root/World/SounderMusic");
		
		S.PlayAudio("Intro.mp3");
		SM.PlayAudio("Music1.mp3");
		

		
		ChangeBackground("Black.png");
		var delay = (Delay) GetNode("/root/World/Delay");
		delay.DoAfter(2f, () => {
			var textFader = (Fadeable) GetNode("/root/World/TransitionLabel/FadeableComponent");
			textFader.FadeIn(2.75f);
			delay.DoAfter(6f, () => {
				WhiteTransition(() => {
					ChangeBackground("ForestBackground.png");
					SetBoard(new[] {"Knife" });
					delay.DoAfter(3.0f, () => {
						ShowMessage("Witness the weapon of your demise.");
					});
				});
			});
		});
	}
	
	public void Interact(Card card1, Card card2)
	{
		var name1 = card1.CardName;
		var name2 = card2.CardName;
		bool areNames(string a, string b) { return (name1 == a && name2 == b) || (name1 == b && name2 == a); }
		
		if (areNames("Bridge", "Knife") && Progress.CrossedBridge == false) {
			S.PlayAudio("Nope.mp3");
			ShowMessage("Don't cut it! It's your only way!");
		} else if (areNames("Bridge", "Pole")) {
			ShowMessage("Balance yourself well...");
			SU.PlayAudio("Success.mp3");
			card1.Remove();
			card2.Remove();
			Progress.CrossedBridge = true;
			D.DoAfter(2.5f, () => {
				S.PlayAudio("Transition.mp3");	
				WhiteTransition(() => {
					SetBoard(new[] {"Bridge", "House", "Well", "Old Lady"});
				});
			});
		} else if (areNames("Bridge", "Knife") && Progress.CrossedBridge) {
			ShowMessage("Ingenious...");
			card1.SlideToHand("Knife", () => {
				CreateCardInHand("Knife");
			});
			D.DoAfter(0.25f, () => {
				S.PlayAudio("Rope.mp3");
				card1.SlideToHand("Rope", () => {
					card1.Remove();
					CreateCardInHand("Rope");
				});
			});
			card2.Remove();
		} else if (areNames("Knife", "Hair Pin")) {
			card1.Remove();
			card2.Remove();
			CreateCardInHand("Lock Pick");
			ShowMessage("Aha! You can open doors with that!");
		} else if (areNames("Lock Pick", "House")) {
			S.PlayAudio("Unlock.mp3");
			card1.SlideToHand("Knife", () => {
				CreateCardInHand("Knife");
			});
			card1.Remove();
			card2.Remove();
			ShowMessage("It clicks and opens...");
			CreateCardOnBoard("Cauldron");
			CreateCardOnBoard("Old Coin");
			CreateCardOnBoard("Bottle");
		} else if (areNames("Rope", "Cauldron")) {
			card1.Remove();
			card2.Remove();
			ShowMessage("A cauldron on a rope? What are you...");
			SU.PlayAudio("Success.mp3");
			CreateCardOnBoard("Bucket");
		} else if (areNames("Bucket", "Well")) {
			card1.Remove();
			card2.Remove();
			ShowMessage("Ah! A working well!");
			CreateCardOnBoard("Water Bucket");
			S.PlayAudio("Splash.mp3");
		} else if (areNames("Old Lady", "Water Bucket")) {
			Progress.OldLadyDone = true;
			card1.Remove();
			card2.Remove();
			SU.PlayAudio("Success.mp3");
			CreateCardOnBoard("Old Lady");
			ShowMessage("Thank you!");
			D.DoAfter(3f, () => {
				ShowMessage("Now, drink from the water of the after.");	
				D.DoAfter(5.5f, () => {
					ShowMessage("Forget your old life...");
					D.DoAfter(3.5f, () => {
						ShowMessage("Forget your family...");
						D.DoAfter(3.5f, () => {
							ShowMessage("And pass away, onward...");
							D.DoAfter(4.5f, () => {
								S.PlayAudio("Transition.mp3");
								WhiteTransition(() => {
									ChangeBackground("DesertBackground.png");
									SetBoard(new[] { "Rabid Dog", "Sign", "Merchant", "Fire Place" });
									D.DoAfter(1.5f, () => {
										ShowMessage("A crossroad! Which way?");
									});
								});
							});
						});
					});
				});
			});
		} else if (areNames("Rope", "Well")) {
			S.PlayAudio("Nope.mp3");
			ShowMessage("Just a rope is not enough to get water.");
		} else if (areNames("Dagger", "House")) {
			ShowMessage("The lock is too solid to slice.");
		} else if (areNames("Hair Pin", "House")) {
			ShowMessage("Lock picking? That requires 2 pins.");
		} else if (areNames("Knife", "Dog")) {
			Progress.FreedDog = true;
			ShowMessage("The dog is free. Take the 'leash'.");
			card1.SlideToHand("Knife", () => {
				CreateCardInHand("Knife");
				CreateCardInHand("Rope");				
			});
			card1.Remove();
			card2.Remove();
		} else if (areNames("Rabid Dog", "Knife")) {
			ShowMessage("You should not try that, lest you get bitten.");
		} else if (areNames("Sign", "Fire Place")) {
			card1.SlideToHand("Torch", () => {
				CreateCardInHand("Torch");
			});
			card1.Remove();
			card2.Remove();
			CreateCardOnBoard("Fire Place");
		} else if (areNames("Rabid Dog", "Torch")) {
			SU.PlayAudio("Success.mp3");
			ShowMessage("Fire! To terrify the eldritch beast!");
			card1.Remove();
			card2.Remove();
			CreateCardOnBoard("Mist");
		} else if (areNames("Merchant", "Old Coin")) {
			ShowMessage("'Fair coin. Have this flute.'");
			card1.SlideToHand("Flute", () => {
				CreateCardInHand("Flute");
			});
			Progress.MerchantCounter = 0;
			Progress.HasFlute = true;
			card1.Remove();
			card2.Remove();
			CreateCardOnBoard("Merchant");
		} else if (areNames("Torch", "Mist")) {
			ShowMessage("The torch clears the mist.");
			card1.Remove();
			card2.Remove();
			CreateCardOnBoard("Cliff");
		} else if (areNames("Cliff", "Cloth")) {
			card1.Remove();
			card2.Remove();
			ShowMessage("Off you glide... no going back!");
			D.DoAfter(4.5f, () => {
				S.PlayAudio("Transition.mp3");
				WhiteTransition(() => {
					ChangeBackground("Black.png");
					ClearBoard();
					D.DoAfter(1.5f, () => {
						ShowMessage("You poor, poor soul...");
					});
				});
			});
		} else if (areNames("Flute", "Fire Place")) {
			ShowMessage("An unusual way to use a flute. If it sits, it fits.");
			card1.SlideToHand("Torch", () => {
				CreateCardInHand("Torch");
			});
			card1.Remove();
			card2.Remove();
			CreateCardOnBoard("Fire Place");
		}
		
		
		else {
			S.PlayAudio("Nope.mp3");
		}
	}
	public void InteractSelf(Card card)
	{
		var name = card.CardName;
		if (card.CardName == "Knife" && Progress.GotKnife == false) {
			Progress.GotKnife = true;
			ShowMessage("It took your life. Now you take it.");
			card.SlideToHand("Knife", () => {
				CreateCardInHand("Knife");
			});
			card.Remove();
			D.DoAfter(2.5f, () => {
				S.PlayAudio("Transition.mp3");
				WhiteTransition(() => {
					SetBoard(new[] { "Bridge", "Dog" });
					D.DoAfter(2.5f, () => {
						ShowMessage("You must press on.");
					});
				});
			});
		} else if (card.CardName == "Knife" && Progress.GotKnife) {
			ShowMessage("The pain it caused will pass.");
		} else if (name == "Pole") {
			if (Progress.GotPole == false) {
				Progress.GotPole = true;
				ShowMessage("It's just a long stick. Of what use?");
				card.SlideToHand("Pole", () => {
					CreateCardInHand("Pole");
				});
				card.Remove();
			}
			
		} else if (name == "Bridge" && Progress.CrossedBridge == false && Progress.FreedDog == false) {
			ShowMessage("A bridge; but your job here is not done.");
		} else if (name == "Bridge" && Progress.CrossedBridge == false && Progress.FreedDog) {
			Progress.CrossedBridge = true;
			S.PlayAudio("Transition.mp3");	
			WhiteTransition(() => {
				ChangeBackground("ForestBackground2.png");
				SetBoard(new[] {"House", "Well", "Old Lady"});
			});
		} else if (name == "Bridge" && Progress.CrossedBridge) {
			ShowMessage("No going back.");
		} else if (name == "House") {
			S.PlayAudio("Locked.mp3");
			ShowMessage("The door is locked.");
		} else if (name == "Well") {
			ShowMessage("It's a well without a bucket.");
		} else if (name == "Old Lady") {
			if (Progress.OldLadyDone)
				return;
			if (Progress.OldLadyCounter == 0) {
				ShowMessage("'Are you lost, lad?'");
				Progress.OldLadyCounter++;
			} else if (Progress.OldLadyCounter == 1) {
				ShowMessage("'Please, help me out with this well.'");
				Progress.OldLadyCounter++;
			} else if (Progress.OldLadyCounter == 2) {
				ShowMessage("'And I will show you the way.'");
				Progress.OldLadyCounter++;
			} else if (Progress.OldLadyCounter == 3) {
				ShowMessage("'Take this hair pin, though. It might be of use.'");
				card.SlideToHand("Hair Pin", () => {
					CreateCardInHand("Hair Pin");
				});
				Progress.OldLadyCounter++;
			} else {
				ShowMessage("'Could you please fix this old well?'");
			}
		} else if (name == "Rope") {
			ShowMessage("You were stabbed, not hanged");
		} else if (name == "Hair Pin") {
			ShowMessage("She gave you her hair pin.");
		} else if (name == "Old Coin") {
			S.PlayAudio("Coin.mp3");
			if (card.IsOnBoard())
				card.GiveSelf();
			ShowMessage("She won't notice it missing...");
		} else if (name == "Bottle") {
			if (card.IsOnBoard())
				card.GiveSelf();
			ShowMessage("An empty bottle.");
		} else if (name == "Cauldron") {
			ShowMessage("Cold. Has not been used in years.");
		} else if (name == "Water Bucket") {
			ShowMessage("The water... it glows.");
		} else if (name == "Dog") {
			ShowMessage("A dog tied to a pole with a rope.");
		} else if (name == "Rabid Dog") {
			if (Progress.RabidDogCounter == 0) {
				ShowMessage("The dog you saved earlier couldn't make it...");
			} else  {
				ShowMessage("And has turned into the haunting Pricolici.");
			}
			Progress.RabidDogCounter++;
		} else if (name == "Sign") {
			ShowMessage("A sign pointing the way to go.");
		} else if (name == "Fire Place") {
			ShowMessage("Did you not expect fire in the land after?");
		} else if (name == "Merchant") {
			if (Progress.HasFlute == false) {
				if (Progress.MerchantCounter == 0) {
					ShowMessage("'I greet you, traveler.");
				} else if (Progress.MerchantCounter == 1) {
					ShowMessage("'I am the afterworld merchant.'");
				} else {
					ShowMessage("'Trade?'");
				}
			} else {
				if (Progress.MerchantCounter == 0) {
					ShowMessage("'Oh, do you want me gone?");
				} else if (Progress.MerchantCounter == 1) {
					ShowMessage("'But I like it here...'");
				} else {
					ShowMessage("'Fine. Happy afterlife!'");
					card.Remove();
					CreateCardOnBoard("Cloth");
				}
			}
			Progress.MerchantCounter++;
		} else if (name == "Mist") {
			ShowMessage("Mist so dense... you will surely get lost.");
		} else if (name == "Cliff") {
			ShowMessage("Cliff so steep... don't fall.");
		} else if (name == "Cloth") {
			ShowMessage("Merchant's large cape. Flutters in the wind.");
			card.GiveSelf();
		}
	}
	
	
	void ReorderCardsOnBoard()
	{
		var board = GetNode<Node>("CardsOnBoard");
		var cardsOnBoard = Utils.GetAllChildren(board);
		if (cardsOnBoard.Length == 0)
			return;
		GD.Print($"Reordering {cardsOnBoard.Length} cards");
			
		var firstCard = (Card) cardsOnBoard[0];
		var cardWidth = firstCard.Texture.GetWidth() * firstCard.Scale.x;
		var cardPadding = cardWidth * 0.33;
		var totalWidth = cardsOnBoard.Length * (cardWidth + cardPadding) - cardPadding;
		var xBase = 0f - totalWidth / 2 + cardWidth / 2;
		
		for (var i = 0; i < cardsOnBoard.Length; i++)
		{
			var x = xBase + i * (cardWidth + cardPadding);
			var y = 0f;
			var card = (Card) cardsOnBoard[i];
			GD.Print(card.CardName);
			card.GlobalPosition = new Vector2((float)x, y);
			card.ResetScale();
		}
	}
	void ReorderCardsInHand()
	{
		var board = GetNode<Node>("CardsInHand");
		var cardsInHand = Utils.GetAllChildren(board);
		if (cardsInHand.Length == 0)
			return;
			
		var firstCard = (Card) cardsInHand[0];
		var cardWidth = firstCard.Texture.GetWidth();
		var cardPadding = cardWidth * 0.25f * 1.25f;
		var totalWidth = cardsInHand.Length * (cardWidth + cardPadding) - cardPadding / 2;
		var xBase = 0f - totalWidth / 2 + (cardsInHand.Length % 2 == 0? 0 : cardWidth / 2);
		var camera = GetParent().GetNode<Camera2D>("Camera2D");
		var y = camera.GlobalPosition.y + GetViewport().Size.y * 3.25f;
		
		for (var i = 0; i < cardsInHand.Length; i++)
		{
			var x = xBase + i * (cardWidth + cardPadding);
			var card = (Card) cardsInHand[i];
			card.GlobalPosition = new Vector2((float)x, y);
			card.ResetScale();
		}
	}
	public void ReorderAllCards()
	{
		ReorderCardsOnBoard();
		ReorderCardsInHand();
	}
	public void CreateCardInHand(string name)
	{
		var cardsInHand = GetNode<Node>("CardsInHand");
		var card = (Card) Utils.SpawnNodeOn(cardsInHand, "res://Assets/Prefabs/Card.tscn");
		card.SetupCard(name);
		ReorderAllCards();
		card.Sparkle();
		SU.PlayAudio("Success.mp3");
	}
	public void CreateCardOnBoard(string name)
	{
		var cardsOnBoard = GetNode<Node>("CardsOnBoard");
		var card = (Card) Utils.SpawnNodeOn(cardsOnBoard, "res://Assets/Prefabs/Card.tscn");
		card.SetupCard(name);
		ReorderAllCards();
	}
	public void ClearBoard()
	{
		var cardsOnBoardNode = GetNode<Node>("CardsOnBoard");
		var allCardsOnBoard = Utils.GetAllChildren(cardsOnBoardNode);
		foreach (Card card in allCardsOnBoard)
		{
			card.Remove();
		}
	}
	public void SetBoard(string[] cardNames)
	{
		ClearBoard();
		foreach (var name in cardNames)
		{
			CreateCardOnBoard(name);
		}
		ReorderAllCards();
	}
	
	
		
	public void ShowMessage(string message)
	{
		var textBG = (Sprite) GetParent().GetNode<TextBackground>("TextBackground");
		var label = (Label) GetParent().GetNode<Label>("DialogueLabel");
		label.Text = message;
		label.GetNode<Fadeable>("FadeableComponent").FadeIn();
		textBG.GetNode<Fadeable>("FadeableComponent").FadeIn();
	}
	
	public void WhiteTransition(Action callback = null)
	{
		GetNode<Node>("/root/World")
			.GetNode<Sprite>("WhiteForeground")
			.GetNode<Fadeable>("FadeableComponent")
			.FadeIn(2f, () => {
				GetNode<Label>("/root/World/DialogueLabel").Text = "";
				if (callback != null)
					callback();
			});
	}
	public void ChangeBackground(string imageName)
	{
		var bg = GetParent().GetNode<Sprite>("Background");
		bg.Texture = GD.Load<Texture>("res://Assets/Backgrounds/" + imageName);
	}
	

}
