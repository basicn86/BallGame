using Godot;
using System;

namespace BallGame.Weapons;

//Todo: this entire thing needs to be redone to avoid technical debt, only using it for the tech demo
public partial class LaserPistolModelInterpolator : MeshInstance3D
{
	private Vector3 previousPos;
	private Vector3 currentPos;


	[Export]
	Node3D targetRigidBody;

	public override void _Ready()
	{
		TopLevel = true;
		previousPos = targetRigidBody.GlobalTransform.Origin;
		currentPos = targetRigidBody.GlobalTransform.Origin;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = previousPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
		GlobalRotation = targetRigidBody.GlobalRotation;
	}

	public override void _PhysicsProcess(double delta)
	{
		previousPos = currentPos;
		currentPos = targetRigidBody.GlobalTransform.Origin;
	}
}
