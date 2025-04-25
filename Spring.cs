using Godot;
using System;

public partial class Spring : Node3D
{


	public void _on_area_3d_body_entered(Node3D body)
	{
		if (body is not Player) return;
		Player player = body as Player;
		
		Vector3 playerVelocity = player.LinearVelocity;
		playerVelocity.Y = 0.0f;
		player.LinearVelocity = playerVelocity;

		player.ApplyCentralImpulse(Vector3.Up * 20f);
	}
}
