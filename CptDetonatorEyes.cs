using Godot;
using System;

public partial class CptDetonatorEyes : Node3D
{
	public Player player;

	public override void _Process(double delta)
	{
		if (player == null) return;
		LookAt(player.GlobalPosition, Vector3.Up);
	}
}
