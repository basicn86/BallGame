using Godot;
using System;

public partial class PlayerSpeedingParticle : RigidBody3D
{
	[Export]
	private Timer despawnTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		if(GetContactCount() > 0 && despawnTimer.IsStopped())
		{
			despawnTimer.Start();
		}
	}

	private void _on_despawn_timer_timeout()
	{
		QueueFree();
	}
}
