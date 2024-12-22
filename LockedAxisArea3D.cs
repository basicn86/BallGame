using Godot;
using System;

public partial class LockedAxisArea3D : Area3D
{
	[Export]
	public bool LockX = false;
	[Export]
	public bool LockY = false;
	[Export]
	public bool LockZ = false;

	private void _on_body_entered(Node body)
	{
		if (body is not Player) return;
		Player player = (Player)body;

		player.AxisLockLinearX = LockX;
		player.AxisLockLinearY = LockY;
		player.AxisLockLinearZ = LockZ;
	}

	private void _on_body_exited(Node body)
	{
		if (body is not Player) return;
		Player player = (Player)body;
		player.AxisLockLinearX = false;
		player.AxisLockLinearY = false;
		player.AxisLockLinearZ = false;
	}
}
