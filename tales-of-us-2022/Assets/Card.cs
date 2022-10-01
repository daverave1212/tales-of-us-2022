using Godot;
using System;

public class Card : Sprite
{
	
	[Export] public string CardName = "Iorgovan";
	public bool IsRemoved = false;

	public static float WIDTH = 900f;
	public static float PADDING = 350f;
	
	public void SetupCard(string name, string imgName)
	{
		CardName = name;
		Texture = GD.Load<Texture>("res://Assets/Cards/" + imgName);
	}
	
	public void PutOnBoard()
	{
		GetBoard().GetNode<Node>("CardsOnBoard").AddChild(this);
		GetBoard().ReorderAllCards();
	}
	public void PutInHand()
	{
		GetBoard().GetNode<Node>("CardsInHand").AddChild(this);
		GetBoard().ReorderAllCards();
	}
	public void Remove()
	{
		if (IsRemoved)
			return;
		IsRemoved = true;
		var board = GetBoard();
		GetParent().RemoveChild(this);
		QueueFree();
		board.ReorderAllCards();
	}
	
	
	Board GetBoard()
	{
		return (Board) GetParent().GetParent();
	}
	
	Area2D GetOverlappingCardArea()
	{
		var area2d = GetNode<Area2D>("MouseArea2D");
		var overlappingAreas = area2d.GetOverlappingAreas();
		if (overlappingAreas.Count < 1)
			return null;
		foreach (Area2D area in overlappingAreas)
		{
			if (area.Name == "CardArea2D" && area.GetParent() != this)
				return area;
		}
		return null;
	}
	
	bool IsBeingDragged = false;
	Vector2 OriginalPosition;
	void _on_Area2D_input_event(object viewport, InputEvent inputEvent, int shape_idx)
	{
		if (Utils.IsMouseLeftClicked(inputEvent))
		{
			OriginalPosition = GlobalPosition;
			IsBeingDragged = true;
		}
		if (Utils.IsMouseLeftReleased(inputEvent))
		{
			if (IsBeingDragged == false) return;	// To prevent this from happening to a card that's not being dragged
			IsBeingDragged = false;
			var cardOverlappingArea = GetOverlappingCardArea();
			if (cardOverlappingArea == null)
			{
				GlobalPosition = OriginalPosition;
			}
			else
			{
				Card overlappingCard = (Card) cardOverlappingArea.GetParent();
				OnInteractWithAnotherCard(overlappingCard);
				if (IsRemoved)
					return;
				GlobalPosition = OriginalPosition;
				GetBoard().ReorderAllCards();
			}
		}
	}
	void OnInteractWithAnotherCard(Card card)
	{
		GetBoard().Interact(this, card);
	}
	public override void _Process(float deltaTime)
	{
		var mousePos = GetGlobalMousePosition();
		if (IsBeingDragged == false)
			return;
		
		GlobalPosition = mousePos;	// Move to mouse
		GetNode<Area2D>("MouseArea2D").GlobalPosition = mousePos; // Reset mouse area
	}

}



