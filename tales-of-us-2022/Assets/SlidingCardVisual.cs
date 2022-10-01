using Godot;
using System;

public class SlidingCardVisual : Sprite
{
	
	public float SlidingSpeed = 7f;
	public Vector2 SlidingToPos;
	public bool IsSliding = false;
	public Action Callback;
	
	public override void _Process(float deltaTime)
	{
		if (IsSliding == false)
			return;
			
		var distanceX = SlidingToPos.x - GlobalPosition.x;
		var distanceY = SlidingToPos.y - GlobalPosition.y;
		
		GlobalPosition = GlobalPosition + new Vector2(distanceX * SlidingSpeed, distanceY * SlidingSpeed) * deltaTime;
		
		if (distanceX < 10 && distanceY < 10)
		{
			IsSliding = false;
			if (Callback != null)
				Callback();
			QueueFree();
		}
	}
	
	public void SlideTo(Vector2 pos, Action callback = null)
	{
		IsSliding = true;
		SlidingToPos = pos;
		GetNode<Fadeable>("FadeableComponent").FadeOut(0.5f);
		Callback = callback;
	}
}
