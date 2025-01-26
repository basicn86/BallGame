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

	[ExportCategory("Tornado Attack")]
	[Export]
	public Node3D affectedBodies;
	private List<RigidBody3D> affectedBodiesArray = new List<RigidBody3D>(); //used for caching the affected bodies
	[Export]
	public ShaderMaterial grassMaterial; //used for sucking in the grass
	[Export]
	public GpuParticles3D tornadoParticles;


	//Important note: need to add entering and exiting states. This is to ensure the nodes are setup properly in between states.
	enum State
	{
		Idle,
		Chasing,
		RocketLaunching,
		ShootingSawBlades,
		EnterTornado,
		Tornado,
		ExitTornado,
		DashAttack
	}

	State state = State.Idle;
	State nextState = State.Idle;

	Player? player = null;

	private double sawBladeCooldownTimer = 0.0;

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
			default:
				break;
		}


		if(sawBladeCooldownTimer > 0.0) sawBladeCooldownTimer -= delta;
	}

	private void ChasePlayer()
	{
		ApplyCentralForce(-LinearVelocity.Normalized() * 50f);
		ApplyCentralForce((player.GlobalTransform.Origin - GlobalTransform.Origin).Normalized() * 150f);
		if (nextStateTimer.IsStopped()) {
			nextStateTimer.Start(1.0);
			nextState = State.ShootingSawBlades;
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

	private void ShootSawBlades()
	{
		ApplyCentralForce(-LinearVelocity.Normalized() * 25f);
		ApplyCentralForce((player.GlobalTransform.Origin - GlobalTransform.Origin).Normalized() * 50f);

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
			nextState = State.Chasing;
		}
	}

	private Vector3 GetSawBladeAttackDirection(float sawBladeSpeed)
	{
		Vector3 playerVelocity = player.LinearVelocity;

		playerVelocity.Y = 0;

		Vector3 direction = (player.GlobalTransform.Origin - GlobalTransform.Origin + playerVelocity * (player.GlobalTransform.Origin - GlobalTransform.Origin).Length() / sawBladeSpeed).Normalized();

		return direction;
	}

	private void EnterTornado()
	{
		tornadoParticles.Emitting = true;
		state = State.Tornado;
	}

	private void ExitTornado()
	{
		tornadoParticles.Emitting = false;
		grassMaterial.SetShaderParameter("windStrength", 0.0f);
		state = State.Chasing;
	}

	private void TornadoAttack()
	{
		ApplyCentralForce(-LinearVelocity.Normalized() * 15f);
		ApplyCentralForce((player.GlobalTransform.Origin - GlobalTransform.Origin).Normalized() * 25f);

		ClearDespawnedBodies();

		foreach (RigidBody3D body in affectedBodiesArray)
		{
			Vector3 directionToCenter = (GlobalTransform.Origin - body.GlobalTransform.Origin).Normalized();
			Vector3 orbitalDirection = new Vector3(-directionToCenter.Z, 0, directionToCenter.X).Normalized(); // Perpendicular vector for orbiting
			Vector3 force = directionToCenter * 20 * body.Mass + orbitalDirection * 10 * body.Mass; // Combine central and orbital forces
			Vector3 upwardForce = Vector3.Up * 4f * body.Mass; // Upward force to keep bodies in the air
			body.ApplyCentralForce(force + upwardForce);
		}

		//pull the player towards the center
		player.ApplyCentralForce((GlobalTransform.Origin - player.GlobalTransform.Origin).Normalized() * 50f);

		grassMaterial.SetShaderParameter("windStrength", -1.0f);
		grassMaterial.SetShaderParameter("windPoint", new Vector2(GlobalPosition.X, GlobalPosition.Z));

		//TODO: This needs to be moved to _Process, however, we cannot communicate with the model at the moment.
		//Using _PhysicsProcess will cause the particles to update at 60 fps rather than the monitor's refresh rate.
		tornadoParticles.GlobalPosition = GlobalPosition;

		if (nextStateTimer.IsStopped())
		{
			nextStateTimer.Start(10.0);
			nextState = State.ExitTornado;
		}
	}

	private void ClearDespawnedBodies()
	{
		for (int i = 0; i < affectedBodiesArray.Count; i++)
		{
			//if a body is despawned, it will NOT be null, so we need to use IsInstanceValid
			if (!IsInstanceValid(affectedBodiesArray[i]))
			{
				affectedBodiesArray.RemoveAt(i);
				i--;
			}
		}
	}

	public void _on_area_3d_body_entered(Node3D body)
	{
		if (body is not Player) return;
		player = body as Player;
		state = State.Chasing;
	}

	public void _on_timer_timeout()
	{
		state = nextState;
	}
}
