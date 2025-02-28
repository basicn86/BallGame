using Godot;
using System;

public partial class LockOnCamera : Node3D, IPlayerCamera
{
	[Export]
	public Node3D target;

	[Export]
	public Camera3D camera;

	//This is the pivot position the camera will sit at, this is not the target's position
	public Vector3 TargetPosition { get; set; }
	private Vector3 _targetPosition;

	public Node3D GetLockOnTarget()
	{
		return null;
	}

	public void Activate()
	{
		camera.Current = true;
	}

	public void ResetPosition(Vector3 targetPosition)
	{
		TargetPosition = targetPosition;
		_targetPosition = targetPosition;
		GlobalPosition = targetPosition;
	}

	public override void _Ready()
	{
		
	}

	public Vector3 GetCrosshairCollisionPoint()
	{
		return target.GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (target == null) return;

		_targetPosition = TargetPosition + new Vector3(0, 2f, 0);
		_targetPosition -= (target.GlobalPosition - GlobalPosition).Normalized() * 5f;
		GlobalPosition = GlobalPosition.Lerp(_targetPosition, MathF.Min(16.0f * (float)delta, 1.0f));

		//TODO: Possibly remove this? The reason why we need this is because the player's movement relies on the camera's basis, and if the camera is looking downwards, the player begins to push against the ground, adding friction to the player's movement. There may be a better way to handle this instead of using the current node's basis.
		Vector3 lookAtPos = target.GlobalPosition;
		lookAtPos.Y = GlobalPosition.Y;
		LookAt(lookAtPos);

		camera.LookAt(target.GlobalPosition);
	}
}
