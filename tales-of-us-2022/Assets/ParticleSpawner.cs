using Godot;
using System;

public class ParticleSpawner : Godot.Particles2D
{
	
	public void EmitAt(Vector2 pos)
	{
		GlobalPosition = pos;
		Emitting = true;
	}
	
}
