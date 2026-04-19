using Godot;
using System;

public partial class LockOnArea : Area3D
{
	[Export]
	private MeshInstance3D meshInstance;
	[Export]
	private Vector3 meshOffset;

	public Vector3 GetInterpolatedPos()
	{
		return prevPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
	}

	Vector3 currentPos;
	Vector3 prevPos;

	public override void _Ready()
	{
		meshInstance.Visible = false;
	}

	public override void _Process(double delta)
	{
		meshInstance.GlobalPosition = GlobalPosition + meshOffset;
		meshInstance.RotateY((float)delta * 4f);
	}

	public override void _PhysicsProcess(double delta)
	{
		prevPos = currentPos;
		currentPos = GlobalPosition;
	}

	public void TargetLocked()
	{
		meshInstance.Visible = true;
	}

	public void TargetUnlocked()
	{
		meshInstance.Visible = false;
	}
}
