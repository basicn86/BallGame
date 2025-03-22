using Godot;
using System;

public partial class LockOnArea : Area3D
{
	[Export]
	private MeshInstance3D meshInstance;
	[Export]
	private Vector3 meshOffset;

	public override void _Ready()
	{
		meshInstance.Visible = false;
	}

	public override void _Process(double delta)
	{
		meshInstance.GlobalPosition = GlobalPosition + meshOffset;
		meshInstance.RotateY((float)delta * 4f);
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
