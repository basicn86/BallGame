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
	public float throwForce = 10f;
	[Export]
	public float throwHeight = 2.5f;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void UpdatePosition(Vector3 position, Basis basis)
	{
		GlobalPosition = position + Offset * basis.Inverse();
		Rotation = basis.GetEuler();
	}

	public void Fire(Vector3 targetPos)
	{
		RigidBody3D grenade = (RigidBody3D)grenadeScene.Instantiate();
		grenade.Position = GlobalTransform.Origin + (Offset * Basis.Inverse());

		GetParent().AddChild(grenade);

		grenade.ApplyImpulse(throwForce * (targetPos - GlobalTransform.Origin).Normalized() + new Vector3(0f,throwHeight,0f));
	}
}
