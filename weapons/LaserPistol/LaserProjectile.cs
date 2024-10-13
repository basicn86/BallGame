using Godot;
using System;

public partial class LaserProjectile : RigidBody3D
{
	[Export]
	private Node3D particleSpawnPoint;
	[Export]
	private PackedScene particleScene;
	[Export]
	private LaserProjectileInterpolator model;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{
	}

	public void ResetInterpolator()
	{
		model.ResetInterpolator();
	}

	public override void _PhysicsProcess(double delta)
	{
		TestCollision();
	}

	private void TestCollision()
	{
		if (GetContactCount() > 0)
		{
			QueueFree();
			Node3D _particles = (Node3D)particleScene.Instantiate();
			GetParent().AddChild(_particles);
			_particles.GlobalPosition = particleSpawnPoint.GlobalPosition;
			QueueFree();
		}
	}


	private void _on_lifetime_timeout()
	{
		QueueFree();
	}
}
