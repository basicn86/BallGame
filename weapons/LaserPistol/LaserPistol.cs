using Godot;
using System;
using static Godot.TextServer;

public partial class LaserPistol : Node3D
{
	[Export]
	Vector3 offset;
	[Export]
	float velocity;
	[Export]
	PackedScene projectileScene;
	[Export]
	Player player;
	[Export]
	PlayerCamera camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (projectileScene == null || player == null || camera == null)
		{
			QueueFree();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Input.IsActionJustPressed("attack")) return;
		if (camera.CrosshairRaycast.GetCollider() == null) return;

		RigidBody3D projectile = (RigidBody3D)projectileScene.Instantiate();
		projectile.Position = player.GlobalTransform.Origin + (offset * camera.Basis.Inverse());

		GetParent().AddChild(projectile);

		Vector3 finalDirection = camera.CrosshairRaycast.GetCollisionPoint() - projectile.GlobalPosition;
		finalDirection = finalDirection.Normalized() * velocity;
		projectile.LinearVelocity = finalDirection;
		projectile.LookAt(camera.CrosshairRaycast.GetCollisionPoint());
	}
}
