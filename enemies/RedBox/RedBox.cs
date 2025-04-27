using Godot;
using System;

public partial class RedBox : RigidBody3D
{
	[Export]
	private Area3D PlayerDetectionArea;

	[Export]
	private Area3D DamageSender;

	private Player? player;

	[Export]
	private float AttackCooldown;
	private float attackCooldownTimer = 0f;

	private Vector3 initialSpawnPosition = Vector3.Zero;

	[Export]
	private PackedScene CoinScene;

	public override void _Ready()
	{
		initialSpawnPosition = GlobalPosition;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player == null) return;

		attackCooldownTimer -= (float)delta;
		if (attackCooldownTimer < 0f && LinearVelocity.Length() < 1f && AngularVelocity.Length() < 0.1f)
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

		Vector3 torqueAxis = impulseDirection;
		torqueAxis.Y = 0f;
		ApplyTorqueImpulse(torqueAxis.Cross(Vector3.Down));
	}

	public void _player_area_entered(Node3D body)
	{
		if (body is not Player) return;
		player = body as Player;
		PlayerDetectionArea.QueueFree();

		AttackPlayer();
	}

	public void jumped_by_player()
	{
		if (player == null)
		{
			DisableProcessing();
			return;
		}
		Vector3 playerVelocity = player.LinearVelocity;
		playerVelocity.Y = 0f;
		player.LinearVelocity = playerVelocity;
		player.ApplyCentralImpulse(Vector3.Up * 15f);

		DisableProcessing();
		SpawnCoins();
	}


	public void take_damage(int damage, int team, Vector3 knockbackForce)
	{
		DisableProcessing();
	}

	public void PlayerDied()
	{
		EnableProcessing();
		GlobalPosition = initialSpawnPosition;
		GlobalRotation = Vector3.Zero;
		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;
		ResetPhysicsInterpolation();
	}

	private void SpawnCoins()
	{
		for (int i = 0; i < 10; i++) {
			DynamicCoin coin = CoinScene.Instantiate() as DynamicCoin;
			GetParent().AddChild(coin);
			coin.GlobalPosition = GlobalPosition;
			Vector3 RandomCoinDirection = new Vector3(
				(float)GD.RandRange(-1.0, 1.0),
				1.0f,
				(float)GD.RandRange(-1.0, 1.0)
			).Normalized();
			coin.ApplyCentralImpulse(RandomCoinDirection * 5f);
		}
	}

	private void DisableProcessing()
	{
		SetDeferred("process_mode", (int)ProcessModeEnum.Disabled);
		DamageSender.SetDeferred("monitorable", false);
		DamageSender.SetDeferred("monitoring", false);
		Visible = false;
	}
	private void EnableProcessing()
	{
		SetDeferred("process_mode", (int)ProcessModeEnum.Inherit);
		DamageSender.SetDeferred("monitorable", true);
		DamageSender.SetDeferred("monitoring", true);
		Visible = true;
	}
}
