using Godot;
using System;

public partial class GrenadeThrower : Node3D
{

	[Export]
	Vector3 Offset;
	[Export]
	Vector3 Direction;
	[Export]
	PackedScene grenadeScene;
	[Export]
	Player player;
	[Export]
	PlayerCamera camera;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("throw_grenade"))
		{
			RigidBody3D grenade = (RigidBody3D)grenadeScene.Instantiate();
			grenade.Position = player.GlobalTransform.Origin + (Offset * camera.Basis.Inverse());

			GetParent().AddChild(grenade);

			grenade.LinearVelocity = player.LinearVelocity;
			Vector3 finalDirection = Direction.Rotated(Vector3.Right, camera.Pitch);
			finalDirection *= camera.Basis.Inverse();
			grenade.ApplyCentralForce(finalDirection);
		}
	}
}
