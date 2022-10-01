using Godot;
using System;

public class Delay : Node
{
	Action Callback = null;
	float TimeElapsed = 0f;
	float TimeToAction = 0f;
	
	public void DoAfter(float time, Action action)
	{
		Callback = action;
		TimeElapsed = 0f;
		TimeToAction = time;
	}
	
	
	public override void _Process(float deltaTime)
	{
		if (Callback == null)
			return;
			
		TimeElapsed += deltaTime;
		if (TimeElapsed >= TimeToAction)
		{
			var callbackBackup = Callback;
			Callback = null;
			callbackBackup();
		}
	}
}
