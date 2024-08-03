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
	AudioStreamPlayer3D fireSound;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (projectileScene == null)
		{
			QueueFree();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void UpdatePosition(Vector3 position, Basis basis)
	{
		GlobalPosition = position + offset * basis.Inverse();
		Rotation = basis.GetEuler();
	}

	public void Fire(Vector3 targetPos)
	{
		RigidBody3D projectile = (RigidBody3D)projectileScene.Instantiate();
		projectile.Position = GlobalTransform.Origin;

		GetParent().AddChild(projectile);

		Vector3 finalDirection = targetPos - projectile.GlobalPosition;
		finalDirection = finalDirection.Normalized() * velocity;
		projectile.LinearVelocity = finalDirection;
		projectile.LookAt(targetPos);

		fireSound.Play();
	}
}
