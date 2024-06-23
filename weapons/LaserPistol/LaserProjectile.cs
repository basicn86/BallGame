using Godot;
using System;

public partial class LaserProjectile : RigidBody3D
{
	[Export]
	private Node3D particleSpawnPoint;
	[Export]
	private PackedScene particleScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if(GetContactCount() > 0)
		{
			QueueFree();
			Node3D _particles = (Node3D)particleScene.Instantiate();
			_particles.Position = particleSpawnPoint.GlobalTransform.Origin;
			GetParent().AddChild(_particles);
			QueueFree();
		}
	}


	private void _on_lifetime_timeout()
	{
		QueueFree();
	}
}
