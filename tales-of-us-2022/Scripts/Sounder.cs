using Godot;
using System;

public class Sounder : AudioStreamPlayer
{

	public override void _Ready()
	{
		
	}
	
	public void PlayAudio(string name)
	{
		var sound = GD.Load<AudioStream>("res://Assets/Audio/" + name);
		Stream = sound;
		Play();
	}

}
