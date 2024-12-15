using Godot;
using System;

public partial class SpinningStar : Node3D
{
	[Export]
	AnimatableBody3D Body;

	[Export]
	float RotationSpeed = 1.0f;

	[Export]
	float InitialRotation = 0.0f;
	[Export]
	float Tilt = 0.0f;

	public override void _EnterTree()
	{
		Body.RotateY(Mathf.DegToRad(InitialRotation));
		Body.RotateX(Mathf.DegToRad(Tilt));
	}

	public override void _PhysicsProcess(double delta)
	{
		Body.RotateObjectLocal(Vector3.Up, RotationSpeed * (float)delta);
	}
}
