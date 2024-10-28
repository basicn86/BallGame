using Godot;
using System;

public partial class Spinner : AnimatableBody3D
{
	[Export]
	public float RotationSpeed = 0.1f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}


	public override void _PhysicsProcess(double delta)
	{
		RotateY(RotationSpeed);
	}
}
