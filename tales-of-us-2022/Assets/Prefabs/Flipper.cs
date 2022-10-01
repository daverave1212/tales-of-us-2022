using Godot;
using System;

public class Flipper : Node
{
	
	public float FlipSpeed = 0.2f;
	public Sprite Parent;
	public float OriginalWidthScale = 1f;
	public Action MiddleCallback;

	public override void _Ready()
	{
		Parent = (Sprite) GetParent();
		OriginalWidthScale = Parent.Scale.x;
	}
	
	public string FlipState = "None";
	public override void _Process(float deltaTime)
	{
		if (FlipState == "Collapsing")
		{
			Parent.Scale = new Vector2(Parent.Scale.x - deltaTime / FlipSpeed, Parent.Scale.y);
			if (Parent.Scale.x <= 0)
			{
				FlipState = "Expanding";
				MiddleCallback();
			}
		}
		else if (FlipState == "Expanding")
		{
			Parent.Scale = new Vector2(Parent.Scale.x + deltaTime / FlipSpeed, Parent.Scale.y);
			if (Parent.Scale.x >= OriginalWidthScale)
			{
				FlipState = "None";
				Parent.Scale = new Vector2(OriginalWidthScale, Parent.Scale.y);
			}
		}
	}

	public void Flip(Action middleCallback)
	{
		Parent.Scale = new Vector2(OriginalWidthScale, Parent.Scale.y);
		FlipState = "Collapsing";
		MiddleCallback = middleCallback;
	}
}
