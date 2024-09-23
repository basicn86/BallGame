using Godot;
using System;

public partial class FlyingPlatform : RigidBody3D
{
	private Vector3 _initialPosition;
	private Vector3 _initialRotation;

	//Pushback force to return the platform to its initial position
	[Export]
	public float ReturnForce = 1f;
	[Export]
	public float ReturnTorque = 1f;
	[Export]
	public float ReturnPositionTolerance = 0.01f;
	[Export]
	public float ReturnAngularTolerance = 0.03f;

	public override void _Ready()
	{
		_initialPosition = GlobalPosition;
		_initialRotation = Rotation;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GlobalPosition.DistanceTo(_initialPosition) > ReturnPositionTolerance)
		{
			Vector3 direction = _initialPosition - GlobalPosition;
			//TODO: it is probably better to apply more force the further away the platform is from its initial position. Maybe don't normalize the direction vector?
			ApplyCentralForce(direction.Normalized() * ReturnForce * (float)delta);
		}

		ApplyTorque(-(Rotation - _initialRotation) * ReturnTorque * (float)delta);
	}
}
