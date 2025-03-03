using Godot;
using System;

public partial class LockOnArea : Area3D
{
	[Export]
	private MeshInstance3D meshInstance;
	[Export]
	private Vector3 meshOffset;

	private Vector3 prevPos;
	private Vector3 currentPos;

	public override void _Ready()
	{
		meshInstance.Visible = false;
	}

	public override void _Process(double delta)
	{
		if (!meshInstance.Visible) return;

		Vector3 targetPos = prevPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
		meshInstance.GlobalPosition = targetPos + meshOffset;
		meshInstance.RotateY((float)delta * 4f);
	}

	public override void _PhysicsProcess(double delta)
	{
		prevPos = currentPos;
		currentPos = GlobalPosition;
	}

	public void TargetLocked()
	{
		prevPos = GlobalPosition;
		currentPos = GlobalPosition;
		meshInstance.Visible = true;
	}

	public void TargetUnlocked()
	{
		meshInstance.Visible = false;
	}
}
