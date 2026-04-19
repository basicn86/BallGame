using Godot;
using System;

public partial class CptDetonatorRocketParticles : Node3D
{
	[Export]
	public Timer timer;
	[Export]
	public GpuParticles3D particleSystem;

	public void StartDespawning()
	{
		particleSystem.Emitting = false;
		timer.Start();
	}

	private void _on_timer_timeout()
	{
		QueueFree();
	}
}
