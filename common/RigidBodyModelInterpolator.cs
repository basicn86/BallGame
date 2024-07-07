using Godot;
using System;

public partial class RigidBodyModelInterpolator : MeshInstance3D
{
	private Vector3 previousPos;
	private Vector3 currentPos;

	private Quaternion previousRotation;
	private Quaternion currentRotation;

	[Export]
	public RigidBody3D targetRigidBody;

	private bool firstFrame = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TopLevel = true;
		previousPos = targetRigidBody.GlobalTransform.Origin;
		currentPos = targetRigidBody.GlobalTransform.Origin;
		previousRotation = targetRigidBody.Quaternion;
		currentRotation = targetRigidBody.Quaternion;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//We have to set the initial position in the first frame. We cannot do it in the _Ready function because the _Ready function runs when the scene is loaded, before the position can be set.
		//TODO: Possibly remove this, this does not seem to do anything anymore.
		if (firstFrame)
		{
			GlobalPosition = targetRigidBody.GlobalTransform.Origin;
			previousPos = targetRigidBody.GlobalTransform.Origin;
			currentPos = targetRigidBody.GlobalTransform.Origin;
			firstFrame = false;
			return;
		}
		Quaternion = previousRotation.Slerp(currentRotation, (float)Engine.GetPhysicsInterpolationFraction());
		GlobalPosition = previousPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
	}

	public override void _PhysicsProcess(double delta)
	{
		previousPos = currentPos;
		currentPos = targetRigidBody.GlobalTransform.Origin;

		previousRotation = currentRotation;
		currentRotation = targetRigidBody.Quaternion;
	}
}
