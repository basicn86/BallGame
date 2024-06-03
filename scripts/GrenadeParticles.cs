using Godot;
using System;

public partial class GrenadeParticles : Node3D
{

	[Export]
	GpuParticles3D[] particles;
	public override void _Ready()
	{
		foreach (var particle in particles)
		{
			particle.Emitting = true;
		}
	}

	//Checks to see if all particles are done emitting, once they are, the node is removed
	public override void _Process(double delta)
	{
		foreach (var particle in particles)
		{
			if (particle.Emitting) return;
		}

		QueueFree();
	}
}
