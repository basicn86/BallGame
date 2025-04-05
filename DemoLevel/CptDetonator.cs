using Godot;
using System.Collections.Generic;
using System;

public partial class CptDetonator : RigidBody3D
{
	[Export]
	public Timer nextStateTimer;

	[ExportCategory("Saw Blade")]
	[Export]
	public PackedScene sawBladeScene;
	[Export]
	public double sawBladeCooldown = 1.0; //cooldown in between saw blade shots
	[Export]
	public double sawBladeAttackApproachDistance = 10.0; //distance to the player to start moving closer.

	[ExportCategory("Rapid Bomb Attack")]
	[Export]
	public PackedScene bombScene;
	[Export]
	public double bombCooldown = 1.0;
	[Export]
	public int maxBombs = 50;
	private int bombCounter = 0;
	[Export]
	public float maxHorizontalVelocity = 5f;
	[Export]
	public float maxVerticalVelocity = 10f;

	[ExportCategory("Rocket Launching Attack")]
	[Export]
	public PackedScene rocketScene;

	[ExportCategory("Tornado Attack")]
	[Export]
	public Node3D affectedBodies;
	private List<RigidBody3D> affectedBodiesArray = new List<RigidBody3D>(); //used for caching the affected bodies
	[Export]
	public ShaderMaterial grassMaterial; //used for sucking in the grass
	[Export]
	public GpuParticles3D tornadoParticles;

	[ExportCategory("Player")]
	[Export]
	public Node3D playerDetectionNode;
	[Export]
	public CptDetonatorEyes eyes;

	[Export]
	public PackedScene jugglingBombScene;

	[Export]
	public GrassMultiMeshInstance3D grassMultiMeshInstance;

	//Important note: need to add entering and exiting states. This is to ensure the nodes are setup properly in between states.
	enum State
	{
		Idle,
		Chasing,
		RocketLaunching,

		EnterShootingSawBlades,
		ShootingSawBlades,

		ShootingBombs,

		BombWall,

		EnterTornado,
		Tornado,
		ExitTornado,

		EnterJuggling,
		Juggling,

		DashAttack
	}

	State state = State.Idle;
	State nextState = State.Idle;

	Player? player = null;

	private double sawBladeCooldownTimer = 0.0;
	private double shootBombCooldownTimer = 0.0;

	public override void _Ready()
	{
		//populate affectedBodiesArray
		for (int i = 0; i < affectedBodies.GetChildCount(); i++)
		{
			affectedBodiesArray.Add(affectedBodies.GetChild<RigidBody3D>(i));
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player == null) return;
		switch (state)
		{
			case State.Chasing:
				ChasePlayer();
				break;
			case State.DashAttack:
				DashAttack();
				break;
			case State.RocketLaunching:
				LaunchRockets();
				break;
			case State.ShootingBombs:
				ShootBombs();
				break;
			case State.BombWall:
				BombWall();
				break;
			case State.EnterShootingSawBlades:
				EnterShootingSawBlades();
				break;
			case State.ShootingSawBlades:
				ShootSawBlades();
				break;
			case State.EnterTornado:
				EnterTornado();
				break;
			case State.Tornado:
				TornadoAttack();
				break;
			case State.ExitTornado:
				ExitTornado();
				break;
			case State.EnterJuggling:
				EnterJuggling();
				break;
			case State.Juggling:
				Juggling();
				break;
			default:
				break;
		}


		if(sawBladeCooldownTimer > 0.0) sawBladeCooldownTimer -= delta;
		if (shootBombCooldownTimer > 0.0) shootBombCooldownTimer -= delta;
	}

	private void EnterJuggling()
	{
		state = State.Juggling;
		CptDetonatorJugglingBomb jugglingBomb = jugglingBombScene.Instantiate() as CptDetonatorJugglingBomb;
		GetParent().AddChild(jugglingBomb);
		jugglingBomb.GlobalPosition = GlobalPosition;
		jugglingBomb.CptDet = this;

		Vector3 direction = player.GlobalPosition - GlobalPosition;
		direction.Y = 0f;
		direction.Normalized();
		direction *= 0.2f;
		direction.Y = 10f;
		jugglingBomb.LinearVelocity = direction;
	}

	private void Juggling()
	{
		
	}

	private void ChasePlayer()
	{
		ApplyCentralForce(-LinearVelocity.Normalized() * 50f);
		ApplyCentralForce((player.GlobalTransform.Origin - GlobalTransform.Origin).Normalized() * 150f);
		if (nextStateTimer.IsStopped()) {
			nextStateTimer.Start(10.0);
			nextState = State.EnterShootingSawBlades;
		}
	}

	private void DashAttack()
	{
		LinearVelocity = Vector3.Zero;
		ApplyCentralImpulse((player.GlobalTransform.Origin - GlobalTransform.Origin).Normalized() * 100f);
		state = State.Idle;
		nextStateTimer.Start(1.0);
		nextState = State.EnterTornado;
	}

	private void BombWall()
	{
		Vector3 direction;
		direction = player.GlobalPosition - GlobalPosition;
		direction.Y = 0f;
		direction = direction.Normalized();
		direction = direction.Rotated(Vector3.Up, 0.3f);
		for (int i = 0; i < 20; i++)
		{
			CptDetonatorBomb bomb = bombScene.Instantiate() as CptDetonatorBomb;
			GetParent().AddChild(bomb);
			bomb.GlobalPosition = GlobalPosition;
			bomb.LinearVelocity = direction * 20f + new Vector3(0f, i * 3f, 0f);
			bomb.grassMultiMeshInstance3D = grassMultiMeshInstance;
		}

		direction = player.GlobalPosition - GlobalPosition;
		direction.Y = 0f;
		direction = direction.Normalized();
		direction = direction.Rotated(Vector3.Up, -0.3f);
		for (int i = 0; i < 20; i++)
		{
			CptDetonatorBomb bomb = bombScene.Instantiate() as CptDetonatorBomb;
			GetParent().AddChild(bomb);
			bomb.GlobalPosition = GlobalPosition;
			bomb.LinearVelocity = direction * 20f + new Vector3(0f, i * 3f, 0f);
			bomb.grassMultiMeshInstance3D = grassMultiMeshInstance;
		}

		state = State.ShootingSawBlades;
	}

	public void LaunchRockets()
	{
		CptDetonatorRocket rocket = rocketScene.Instantiate() as CptDetonatorRocket;
		GetParent().AddChild(rocket);
		rocket.GlobalPosition = GlobalPosition;
		Vector3 travelDir = -(player.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
		travelDir = travelDir.Slerp(Vector3.Down, 0.5f);
		travelDir = travelDir.Rotated(Vector3.Up, Random.Shared.NextSingle() * 3.0f - 1.5f);
		rocket.TravelDirection = travelDir;
	}

	public void ShootBombs()
	{
		if (bombCounter > maxBombs)
		{
			state = State.EnterShootingSawBlades;
			bombCounter = 0;
			return;
		}
		if (shootBombCooldownTimer > 0.0) return; //still in cooldown
		shootBombCooldownTimer = bombCooldown; //reset timer

		CptDetonatorBomb bomb = bombScene.Instantiate() as CptDetonatorBomb;
		GetParent().AddChild(bomb);
		bomb.GlobalPosition = GlobalPosition; //dont worry about colliding with the boss, it is not in the same layer
		Vector3 direction = GetRandomBombDirection();
		bomb.LinearVelocity = direction;

		bomb.grassMultiMeshInstance3D = grassMultiMeshInstance;

		bombCounter++;
	}

	private void EnterShootingSawBlades()
	{
		state = State.ShootingSawBlades;
	}

	private void ShootSawBlades()
	{
		ApplyCentralForce(-LinearVelocity.Normalized() * 25f);
		if ((player.GlobalTransform.Origin - GlobalTransform.Origin).Length() > sawBladeAttackApproachDistance)
		{
			ApplyCentralForce((player.GlobalTransform.Origin - GlobalTransform.Origin + player.LinearVelocity).Normalized() * 50f);
		}

		if (sawBladeCooldownTimer > 0.0) return; //still in cooldown
		sawBladeCooldownTimer = sawBladeCooldown; //reset timer

		SawBladeProjectile sawBlade = sawBladeScene.Instantiate() as SawBladeProjectile;
		GetParent().AddChild(sawBlade);
		sawBlade.GlobalPosition = GlobalPosition;

		sawBlade.speed = 25f;
		sawBlade.direction = GetSawBladeAttackDirection(sawBlade.speed);
		sawBlade.LookAt(player.GlobalTransform.Origin, Vector3.Up);
		sawBlade.SetTimer(5.0f);

		if (nextStateTimer.IsStopped())
		{
			nextStateTimer.Start(10.0);
			nextState = State.ShootingBombs;
		}
	}

	/// <summary>
	/// Calculates the best direction to shoot the saw blade at the player.
	/// This method takes into account the player's current velocity and position
	/// to predict where the player will be when the saw blade reaches them.
	/// </summary>
	/// <param name="sawBladeSpeed">The speed of the saw blade projectile.</param>
	/// <returns>A normalized Vector3 representing the direction to shoot the saw blade.</returns>
	private Vector3 GetSawBladeAttackDirection(float sawBladeSpeed)
	{
		// Get the player's current velocity
		Vector3 playerVelocity = player.LinearVelocity;

		// Ignore the Y component of the player's velocity to keep the calculation in the horizontal plane
		playerVelocity.Y = 0;

		// Calculate the direction to the player, adjusted by the player's velocity and the distance to the player
		Vector3 direction = (player.GlobalTransform.Origin - GlobalTransform.Origin + playerVelocity * (player.GlobalTransform.Origin - GlobalTransform.Origin).Length() / sawBladeSpeed).Normalized();

		// Return the normalized direction vector
		return direction;
	}

	private Vector3 GetRandomBombDirection()
	{
		Vector3 v3 = new Vector3(0f, maxVerticalVelocity, 0f);
		v3.X = Random.Shared.NextSingle() * 2f - 1f;
		v3.Z = Random.Shared.NextSingle() * 2f - 1f;

		v3.X *= maxHorizontalVelocity;
		v3.Z *= maxHorizontalVelocity;

		return v3;
	}


	private float tornadoAngle = 0f; //in radians
	private void EnterTornado()
	{
		tornadoAngle = 0f;
		state = State.Tornado;
	}

	private void ExitTornado()
	{
		state = State.Chasing;
	}

	private void TornadoAttack()
	{
		ApplyCentralForce(-LinearVelocity.Normalized() * 15f);

		SawBladeProjectile sawBlade = sawBladeScene.Instantiate() as SawBladeProjectile;
		GetParent().AddChild(sawBlade);
		sawBlade.GlobalPosition = GlobalPosition;

		SawBladeProjectile sawBladeOpposite = sawBladeScene.Instantiate() as SawBladeProjectile;
		GetParent().AddChild(sawBladeOpposite);
		sawBladeOpposite.GlobalPosition = GlobalPosition;

		Vector3 direction = player.GlobalPosition - GlobalPosition;
		direction.Y = 0f;
		direction = direction.Normalized();
		direction = direction.Rotated(Vector3.Up, tornadoAngle);
		sawBlade.speed = 25f;
		sawBlade.direction = direction;
		sawBladeOpposite.speed = 25f;
		sawBladeOpposite.direction = -direction;

		tornadoAngle += 0.2f;

		if (tornadoAngle > 300f)
		{
			state = State.Idle;
		}
	}

	public void _on_area_3d_body_entered(Node3D body)
	{
		if (body is not Player) return;
		player = body as Player;
		state = State.EnterJuggling;
		eyes.player = player;
		playerDetectionNode.QueueFree();
	}

	public void _on_timer_timeout()
	{
		state = nextState;
	}
}
