using Godot;
using System;

public class Fadeable : Node
{
	
	public float FadeTime = 1.25f;
	public float FadeOutTime = 1.25f;
	public float WaitTime = 4f;
	public string FadingState = "Invisible";
	public float TimeOnScreen = 0f;
	public Action Callback;
	public CanvasItem Parent;
	
	public override void _Ready()
	{
		Parent = (CanvasItem) GetParent();
	}

	
	public override void _Process(float deltaTime)
	{
		if (IsInstanceValid(Parent) == false)
			return;
		if (FadingState == "In")
		{
			Utils.AddSpriteAlpha(Parent, deltaTime / FadeTime);
			if (Parent.Modulate.a >= 1) {
				FadingState = "Visible";
				TimeOnScreen = 0f;
				if (Callback != null)
				{
					Callback();
				}
				Callback = null;
			}
		}
		else if (FadingState == "Out")
		{
			Utils.AddSpriteAlpha(Parent, - deltaTime / FadeOutTime);
			if (Parent.Modulate.a <= 0) {
				FadingState = "Invisible";
			}
		}
		else if (FadingState == "Visible")
		{
			TimeOnScreen += deltaTime;
			if (TimeOnScreen >= WaitTime)
			{
				FadeOut();
			}
		}
		
	}
	
	public void FadeIn(float waitTime = 4f, Action callback = null)
	{
		WaitTime = waitTime;
		Utils.SetSpriteAlpha(Parent, 0);
		FadingState = "In";
		TimeOnScreen = 0f;
		Callback = callback;
	}
	public void FadeOut(float overTime = 1.25f)
	{
		FadeOutTime = overTime;
		Utils.SetSpriteAlpha(Parent, 1);
		FadingState = "Out";
		TimeOnScreen = 0f;
	}
}
