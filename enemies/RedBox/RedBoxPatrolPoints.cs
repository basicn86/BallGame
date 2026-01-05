using Godot;
using System;

public partial class RedBoxPatrolPoints : RigidBody3D
{
	[Export]
	private Area3D PlayerDetectionArea;

	[Export]
	private Area3D DamageSender;

	[Export]
	AudioStreamPlayer3D JumpSound;

	private Player? player;

	[Export]
	private float AttackCooldown;
	private float attackCooldownTimer = 0f;

	private Vector3 initialSpawnPosition = Vector3.Zero;

	[Export]
	private PackedScene CoinScene;

	[Export]
	private Area3D[] PatrolAreas;

	private int currentPatrolIndex = 0;

	public override void _Ready()
	{
		if (PatrolAreas.Length == 0)
		{
			GD.PrintErr("RedBoxPatrolPoints: No patrol areas assigned!");
			QueueFree();
			return;
		}

		foreach (Area3D area in PatrolAreas)
		{
			area.BodyEntered += _patrol_area_entered;
		}

		initialSpawnPosition = GlobalPosition;

		PlayerDetectionArea.TopLevel = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		attackCooldownTimer -= (float)delta;
		if (attackCooldownTimer > 0.0f) return;
		if (player == null)
		{
			if (LinearVelocity.Length() < 1f && AngularVelocity.Length() < 0.1f)
			{
				JumpTowards(PatrolAreas[currentPatrolIndex].GlobalPosition);
			}
			return;
		}


		if (LinearVelocity.Length() < 1f && AngularVelocity.Length() < 0.1f)
		{
			LinearVelocity = Vector3.Zero;
			JumpTowards(player.GlobalPosition);
		}
	}

	private void JumpTowards(Vector3 TargetPos)
	{
		Vector3 impulseDirection = TargetPos - GlobalPosition;
		impulseDirection = impulseDirection.Normalized();
		impulseDirection.Y += 1f;
		ApplyCentralImpulse(impulseDirection * 4f);

		Vector3 torqueAxis = impulseDirection;
		torqueAxis.Y = 0f;
		ApplyTorqueImpulse(torqueAxis.Cross(Vector3.Down));

		JumpSound.Play();

		attackCooldownTimer = AttackCooldown;
	}

	public void _patrol_area_entered(Node body)
	{
		if (body is not RedBoxPatrolPoints) return;
		if (body != this) return;
		currentPatrolIndex++;
		if (currentPatrolIndex >= PatrolAreas.Length) currentPatrolIndex = 0;
	}

	public void _player_area_entered(Node3D body)
	{
		if (body is not Player) return;
		player = body as Player;
	}

	public void _player_area_exited(Node3D body)
	{
		if (body is not Player) return;
		player = null;
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
		for (int i = 0; i < 10; i++)
		{
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
		Visible = false;
	}
	private void EnableProcessing()
	{
		SetDeferred("process_mode", (int)ProcessModeEnum.Inherit);
		Visible = true;
	}
}
