using Godot;
using System;

public partial class LaserPistol : Node3D
{
	[Export]
	Vector3 offset;
	[Export]
	Vector3 projectileSpawnOffset = Vector3.Zero;
	[Export]
	float velocity;
	[Export]
	PackedScene projectileScene;
	[Export]
	PackedScene particleShootScene;
	[Export]
	AudioStreamPlayer3D fireSound;

	[Export]
	Timer fireRateTimer;
	private double fireRate = 0.0f;

	// Used to fire the laser on the next frame, aka input buffering
	private bool fireNextFrame = false;
	private Vector3 fireNextFrameTarget = Vector3.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (projectileScene == null)
		{
			QueueFree();
		}

		fireRate = fireRateTimer.WaitTime;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (fireNextFrame)
		{
			fireNextFrame = false;
			Fire(fireNextFrameTarget);
		}
	}

	public void UpdatePosition(Vector3 position, Basis basis)
	{
		GlobalPosition = position + offset * basis.Inverse();
		Rotation = basis.GetEuler();
	}

	public void Fire(Vector3 targetPos)
	{
		if (!fireRateTimer.IsStopped()) {
			fireNextFrame = true;
			fireNextFrameTarget = targetPos;
			return;
		}
		fireRateTimer.Start(fireRate);

		LaserProjectile projectile = (LaserProjectile)projectileScene.Instantiate();
		projectile.GlobalTransform = GlobalTransform;

		GetParent().AddChild(projectile);

		projectile.GlobalPosition = GlobalPosition;
		projectile.TranslateObjectLocal(projectileSpawnOffset);

		//Dumb hack: We need to reset the model interpolator so it doesn't spawn at the world origin
		projectile.ResetInterpolator();

		Vector3 finalDirection = targetPos - projectile.GlobalPosition;
		finalDirection = finalDirection.Normalized() * velocity;
		projectile.LinearVelocity = finalDirection;
		projectile.LookAt(targetPos);

		fireSound.Play();

		Node3D _particles = (Node3D)particleShootScene.Instantiate();
		AddChild(_particles);
	}
}
