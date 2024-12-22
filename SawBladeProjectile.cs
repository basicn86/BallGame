using Godot;
using System;

public partial class SawBladeProjectile : Node3D
{
	[Export]
	public Vector3 direction = new Vector3(0, 0, 0);
	[Export]
	public float speed = 0.0f;
	[Export]
	public float rotationSpeed = 5.0f;

	[Export]
	public Timer lifeTimer;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalTranslate(direction.Normalized() * speed * (float)delta);
		Rotate(Vector3.Down, rotationSpeed * (float)delta);
	}

	public void SetTimer(float time)
	{
		lifeTimer.WaitTime = time;
		lifeTimer.Start();
	}

	public void _on_timer_timeout()
	{
		QueueFree();
	}
}
