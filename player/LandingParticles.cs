using Godot;
using System;

public partial class LandingParticles : GpuParticles3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Emitting)
		{
			GD.Print("Particles are done emitting, removing node");
			QueueFree();
		}
	}
}
