using Godot;
using System;

public class Background : Sprite
{
	
	Vector2 BasePos;
	public override void _Ready()
	{
		BasePos = GlobalPosition;
	}


	Vector2 MaxBGOffset = new Vector2(-100, -50);
	public override void _Process(float deltaTime)
	{
		var mousePos = GetViewport().GetMousePosition();
		var maxMousePos = GetViewport().Size / 2;
		var mousePosRelative = mousePos - GetViewport().Size / 2;
		var mousePercentage = mousePosRelative / maxMousePos;
		
		var bgPos = BasePos + mousePercentage * MaxBGOffset;
		GlobalPosition = bgPos;
	}
}
