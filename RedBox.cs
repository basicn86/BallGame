using Godot;
using System;

public partial class RedBox : RigidBody3D
{
	[Export]
	private Area3D PlayerDetectionArea;

	private Player? player;

	public void _player_area_entered(Node3D body)
	{
		if (body is not Player) return;
		player = body as Player;
		PlayerDetectionArea.QueueFree();

		Vector3 impulseDirection = body.GlobalPosition - GlobalPosition;

		impulseDirection.Y += 1f;

		ApplyCentralImpulse(impulseDirection * 2f);
	}

	public void _on_hurt_box_jumped_by_player()
	{
		player.ApplyCentralImpulse(Vector3.Up * 15f);
		QueueFree();
	}
}
