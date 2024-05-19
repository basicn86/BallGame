using Godot;
using System;

public partial class player : RigidBody3D
{


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 moveVector = new Vector3();
		moveVector.X = Input.GetAxis("left", "right");
		moveVector.Z = Input.GetAxis("forward", "backward");

		moveVector = moveVector.Normalized();

		ApplyCentralForce(moveVector * 1000f * (float)delta);


		if (Input.IsActionJustPressed("ui_accept"))
		{
			ApplyCentralImpulse(new Vector3(0, 5f, 0));
		}
	}
}
