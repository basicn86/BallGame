using Godot;
using System;

public partial class SawBladeShooter : Node3D
{
	[Export]
	public PackedScene sawBladeScene;
	[Export]
	public Timer shootTimer;


	[Export]
	public float shootInterval = 1.0f;

	[Export]
	public Vector3 direction;

	[Export]
	public float bladeSpeed = 10.0f;

	[Export]
	public float bladeLifeTime = 2.0f;

	public override void _Ready()
	{
		shootTimer.WaitTime = shootInterval;
		shootTimer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_timer_timeout()
	{
		SawBladeProjectile sawBlade = (SawBladeProjectile)sawBladeScene.Instantiate();
		GetParent().AddChild(sawBlade);

		sawBlade.GlobalTransform = GlobalTransform;

		sawBlade.direction = direction;
		sawBlade.speed = bladeSpeed;
		sawBlade.SetTimer(bladeLifeTime);
	}
}
