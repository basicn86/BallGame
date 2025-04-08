using Godot;
using System;

public partial class RedBox : RigidBody3D
{
	[Export]
	private Area3D PlayerDetectionArea;

	private Player? player;

	[Export]
	private float AttackCooldown;
	private float attackCooldownTimer = 0f;

	public override void _PhysicsProcess(double delta)
	{
		if (player == null) return;

		attackCooldownTimer -= (float)delta;
		if (attackCooldownTimer < 0f && LinearVelocity.Length() < 1f)
		{
			LinearVelocity = Vector3.Zero;
			AttackPlayer();
			attackCooldownTimer = AttackCooldown;
		}
	}

	private void AttackPlayer()
	{
		if (player == null) return;
		Vector3 impulseDirection = player.GlobalPosition - GlobalPosition;
		impulseDirection = impulseDirection.Normalized();
		impulseDirection.Y += 1f;
		ApplyCentralImpulse(impulseDirection * 4f);
	}

	public void _player_area_entered(Node3D body)
	{
		if (body is not Player) return;
		player = body as Player;
		PlayerDetectionArea.QueueFree();

		AttackPlayer();
	}

	public void _on_hurt_box_jumped_by_player()
	{
		Vector3 playerVelocity = player.LinearVelocity;
		playerVelocity.Y = 0f;
		player.LinearVelocity = playerVelocity;
		player.ApplyCentralImpulse(Vector3.Up * 15f);
		QueueFree();
	}
}
