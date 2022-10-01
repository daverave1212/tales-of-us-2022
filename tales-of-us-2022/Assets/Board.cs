using Godot;
using System;

public class Board : Node2D
{
	
	
	void ReorderCardsOnBoard()
	{
		var board = GetNode<Node>("CardsOnBoard");
		var cardsOnBoard = Utils.GetAllChildren(board);
		if (cardsOnBoard.Length == 0)
			return;
		GD.Print($"There are now {cardsOnBoard.Length} cards");
			
		var firstCard = (Card) cardsOnBoard[0];
		var cardWidth = firstCard.Texture.GetWidth();
		var cardPadding = cardWidth * 0.33;
		var totalWidth = cardsOnBoard.Length * (cardWidth + cardPadding) - cardPadding / 2;
		var xBase = 0f - totalWidth / 2;
		
		for (var i = 0; i < cardsOnBoard.Length; i++)
		{
			var x = xBase + i * (cardWidth + cardPadding);
			var y = 0f;
			var card = (Card) cardsOnBoard[i];
			card.GlobalPosition = new Vector2((float)x, y);
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
		var cardPadding = cardWidth * 0.1;
		var totalWidth = cardsInHand.Length * (cardWidth + cardPadding) - cardPadding / 2;
		var xBase = 0f - totalWidth / 2;
		var camera = GetParent().GetNode<Camera2D>("Camera2D");
		var y = camera.GlobalPosition.y + GetViewport().Size.y * 3.25f;
		
		for (var i = 0; i < cardsInHand.Length; i++)
		{
			var x = xBase + i * (cardWidth + cardPadding);
			var card = (Card) cardsInHand[i];
			card.GlobalPosition = new Vector2((float)x, y);
		}
	}
	public void ReorderAllCards()
	{
		ReorderCardsOnBoard();
		ReorderCardsInHand();
	}
	public void CreateCardInHand(string name, string imgPath)
	{
		var cardsInHand = GetNode<Node>("CardsInHand");
		var card = (Card) Utils.SpawnNodeOn(cardsInHand, "res://Assets/Prefabs/Card.tscn");
		card.SetupCard(name, imgPath);
		ReorderAllCards();
	}
	
	public override void _Ready()
	{
		ReorderCardsOnBoard();
		ReorderCardsInHand();
	}
	
	public void Interact(Card card1, Card card2)
	{
		var name1 = card1.CardName;
		var name2 = card2.CardName;
		bool areNames(string a, string b) { return (name1 == a && name2 == b) || (name1 == b && name2 == a); }
		
		GD.Print(name1);
		GD.Print(name2);
		if (areNames("K", "Mateiu")) {
			card1.Remove();
			card2.Remove();
			ReorderAllCards();
		}
	}
	

}
