using Godot;
using System;
using System.Collections.Generic;

public class Card : Sprite
{
	
	[Export] public string CardName = "Iorgovan";
	public bool IsRemoved = false;
	public string ImageName = "KidsSnow.png";
	public Vector2 BaseScale;

	public static float WIDTH = 900f;
	public static float PADDING = 350f;
	
	public void SetupCard(string name)
	{
		CardName = name;
		ImageName = GetImagePathFromName(name);
		Texture = GD.Load<Texture>("res://Assets/Cards/" + ImageName);
		BaseScale = Scale;
	}
	
	public void PutOnBoard()
	{
		GetBoard().GetNode<Node>("CardsOnBoard").AddChild(this);
		ShowShadow();
		HideGlow();
		GetBoard().ReorderAllCards();
	}
	public void PutInHand()
	{
		GetBoard().GetNode<Node>("CardsInHand").AddChild(this);
		GetBoard().ReorderAllCards();
		HideShadow();
		HideGlow();
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
	public bool IsOnBoard()
	{
		return IsRemoved == false && GetParent().Name == "CardsOnBoard";
	}
	public bool IsInHand()
	{
		return IsRemoved == false && GetParent().Name == "CardsInHand";
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
	
	public void ShowShadow()
	{
		Utils.SetSpriteAlpha(GetNode<Sprite>("Shadow"), 1);
	}
	public void HideShadow()
	{
		Utils.SetSpriteAlpha(GetNode<Sprite>("Shadow"), 0);
	}
	public void ShowGlow()
	{
		Utils.SetSpriteAlpha(GetNode<Sprite>("Glow"), 1);
	}
	public void HideGlow()
	{
		Utils.SetSpriteAlpha(GetNode<Sprite>("Glow"), 0);
	}
	public void Sparkle()
	{
		GetNode<Fadeable>("WhiteCardOverlay/FadeableComponent").FadeOut();
		var particles = (ParticleSpawner) GetNode("/root/World/Particles2D");
		particles.EmitAt(GlobalPosition);
	}
	
	public void SlideToHand(string cardName = "none", Action callback = null)
	{
		string imageName;
		if (cardName == "none")
			imageName = ImageName;
		else
			imageName = GetImagePathFromName(cardName);
		var world = GetNode<Node>("/root/World");
		var card = (SlidingCardVisual) Utils.SpawnNodeOn(world, "res://Assets/Prefabs/SlidingCardVisual.tscn");
		card.GlobalPosition = GlobalPosition;
		card.Texture = GD.Load<Texture>("res://Assets/Cards/" + imageName);
		var camera = GetNode<Node>("/root/World").GetNode<Camera2D>("Camera2D");
		var x = camera.GlobalPosition.x;
		var y = camera.GlobalPosition.y + GetViewport().Size.y * 3.25f;
		card.SlideTo(new Vector2(x, y), callback);
	}
	public void GiveSelf()
	{
		var board = GetBoard();
		SlideToHand(CardName, () => {
			board.CreateCardInHand(CardName);
		});
		Remove();
	}
	public void FlipAndChange(string newName)
	{
		GetNode<Flipper>("Flipper").Flip(() => {
			SetupCard(newName);
		});
	}
	public void ResetScale()
	{
		Scale = BaseScale;
	}
	
	
	
	
	
	
	
	
	// ------------------- EVENTS ----------------------
	
	
	bool IsBeingDragged = false;
	Vector2 OriginalPosition;
	Vector2 ClickOffset;
	void _on_Area2D_input_event(object viewport, InputEvent inputEvent, int shape_idx)
	{
		if (Utils.IsMouseLeftClicked(inputEvent))
		{
			ResetScale();
			OriginalPosition = GlobalPosition;
			IsBeingDragged = true;
			var mousePos = GetGlobalMousePosition();
			ClickOffset = mousePos - GlobalPosition;
			HideShadow();
			ShowGlow();
			GetNode<Sounder>("/root/World/Sounder").PlayAudio("Card.mp3");
		}
		if (Utils.IsMouseLeftReleased(inputEvent))
		{
			GetNode<Sounder>("/root/World/Sounder").PlayAudio("CardOff.mp3");
			ResetScale();
			if (IsBeingDragged == false) return;	// To prevent this from happening to a card that's not being dragged
			IsBeingDragged = false;
			HideGlow();
			if (IsOnBoard())
				ShowShadow();
			var cardOverlappingArea = GetOverlappingCardArea();
			if (cardOverlappingArea == null)
			{
				GlobalPosition = OriginalPosition;
				OnInteractWithSelf();
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
	void OnInteractWithSelf()
	{
		GetBoard().InteractSelf(this);
	}
	private void _on_CardArea2D_mouse_entered()
	{
		Scale = BaseScale * new Vector2(1.15f, 1.15f);
	}
	private void _on_CardArea2D_mouse_exited()
	{
		ResetScale();
	}
	public override void _Process(float deltaTime)
	{
		if (IsBeingDragged)
		{
			var mousePos = GetGlobalMousePosition();
			GlobalPosition = mousePos - ClickOffset;	// Move to mouse
			GetNode<Area2D>("MouseArea2D").GlobalPosition = mousePos; // Reset mouse area
		}
	}
	
	
	
	
	
	
	
	// -------------------- Other ----------------------
	public static Dictionary<string, string> NameToImagePath = new Dictionary<string, string>()
	{
		{ "Knife", "Knife.png" },
		{ "Pole", "Pole.png" },
		{ "Bridge", "Bridge.png" },
		{ "House", "House.png" },
		{ "Well", "Well.png" },
		{ "Old Lady", "OldLady.png" },
		{ "Hair Pin", "HairPin.png" },
		{ "Rope", "Rope.png" },
		{ "Lock Pick", "LockPick.png" },
		{ "Cauldron", "Cauldron.png" },
		{ "Old Coin", "Coin.png" },
		{ "Bottle", "Bottle.png" },
		{ "Bucket", "Bucket.png" },
		{ "Dog", "DogOnLeash.png" },
		{ "Water Bucket", "CauldronWithWater.png" },
		
		{ "Merchant", "Merchant.png" },
		{ "Sign", "Sign.png" },
		{ "Fire Place", "FirePlace.png" },
		{ "Rabid Dog", "RabidDog.png" },
		{ "Torch", "Torch.png" },
		{ "Mist", "Mist.png" },
		{ "Flute", "Flute.png" },
		{ "Cloth", "Cloth.png" },
		{ "Cliff", "Cliff.png" },
	};
	public static string GetImagePathFromName(string name)
	{
		if (NameToImagePath.ContainsKey(name) == false) return "KidsSnow.png";
		return NameToImagePath[name];
	}
	
	

}









