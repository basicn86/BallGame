using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : RigidBody3D
{
	[ExportCategory("Player References")]
	[Export]
	private PlayerCamera cameraNode;
	[Export]
	public MeshInstance3D playerModel;
	[Export]
	public RayCast3D groundCast;
	[Export]
	public PackedScene jumpParticles;
	[Export]
	public PackedScene deathParticles;

	[ExportGroup("Movement")]
	[Export]
	public float MaxVelocity = 10f;
	[Export]
	public float MaxVelocityPushback; //this is the amount of force to apply to the player when they exceed the max velocity
	[Export]
	public float GroundAcceleration = 2000f;
	[Export]
	public float AirAcceleration = 750f;
	[Export]
	public float NoInputDeceleration = 100f;
	[Export]
	public float JumpVelocityIgnoreFactor = 0.5f;


	private bool hasLanded = false;

	[ExportCategory("Weapons")]
	[Export]
	LaserPistol laserPistol;
	[Export]
	GrenadeThrower grenadeThrower;

	[ExportCategory("Sounds")]
	[Export]
	public AudioStreamPlayer3D jumpSound;
	[Export]
	public AudioStreamPlayer3D landSound;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cameraNode.PlayerRid = GetRid();

		groundCast.TopLevel = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateGroundCast();

		TryJumping();

		cameraNode.TargetPosition = playerModel.GlobalPosition;

		laserPistol.UpdatePosition(GlobalPosition, cameraNode.Basis);
		if (Input.IsActionJustPressed("attack"))
		{
			laserPistol.Fire(cameraNode.GetCrosshairCollisionPoint());
		}

		grenadeThrower.UpdatePosition(GlobalPosition, cameraNode.Basis);
		if (Input.IsActionJustPressed("throw_grenade"))
		{
			grenadeThrower.Fire();
		}
		if(Input.IsActionJustReleased("throw_grenade"))
		{
			grenadeThrower.Release(cameraNode.GetCrosshairCollisionPoint());
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		MovePlayer(delta);
		RestrictPlayerVelocity(delta);
	}

	//moves player based on input horizontally, but not vertically
	private void MovePlayer(double delta)
	{
		Vector3 moveVector = new Vector3();
		moveVector.X = Input.GetAxis("left", "right");
		moveVector.Z = Input.GetAxis("forward", "backward");
		moveVector *= cameraNode.Basis.Inverse();
		moveVector.Y = 0;

		if (moveVector.IsZeroApprox())
		{
			Vector3 vel = LinearVelocity;
			vel.Y = 0;
			ApplyCentralForce(-vel * NoInputDeceleration);
			return;
		}

		if (groundCast.IsColliding()) moveVector = AdjustMoveVectorBySlope(moveVector, groundCast.GetCollisionNormal());

		if (moveVector.Length() > 1f) moveVector = moveVector.Normalized();

		if (groundCast.IsColliding())
			ApplyCentralForce(moveVector * GroundAcceleration);
		else
			ApplyCentralForce(moveVector * AirAcceleration);
	}

	private void RestrictPlayerVelocity(double delta)
	{
		Vector3 velocity = LinearVelocity;
		velocity.Y = 0; //don't restrict vertical velocity
		if (velocity.Length() > MaxVelocity) ApplyCentralForce(-velocity.Normalized() * MaxVelocityPushback);
	}

	private void TryJumping()
	{
		if (Input.IsActionJustPressed("jump") && groundCast.IsColliding())
		{
			LinearVelocity = new Vector3(LinearVelocity.X, Mathf.Max(0f, LinearVelocity.Y * JumpVelocityIgnoreFactor), LinearVelocity.Z);
			ApplyCentralImpulse(new Vector3(0, 9f, 0));
			jumpSound.Play();
		}

		//Let the player jump higher if the jump button is held down
		if (!Input.IsActionPressed("jump") && !groundCast.IsColliding())
		{
			GravityScale = 2.5f;
		}
		else
		{
			GravityScale = 1.5f;
		}
	}

	private void SpawnLandingParticles()
	{
		GpuParticles3D jumpParticlesInstance = (GpuParticles3D)jumpParticles.Instantiate();
		GetParent().AddChild(jumpParticlesInstance);
		jumpParticlesInstance.GlobalPosition = groundCast.GetCollisionPoint() + new Vector3(0, 0.15f, 0);
		//rotate the particles to match the ground normal
		jumpParticlesInstance.RotateX(groundCast.GetCollisionNormal().AngleTo(Vector3.Up));
		Vector3 groundNormalHeightless = groundCast.GetCollisionNormal();
		groundNormalHeightless.Y = 0;
		if(groundNormalHeightless.X > 0)
			jumpParticlesInstance.RotateY(-groundNormalHeightless.AngleTo(Vector3.Forward) + Mathf.Pi);
		else
			jumpParticlesInstance.RotateY(groundNormalHeightless.AngleTo(Vector3.Forward) + Mathf.Pi);
		jumpParticlesInstance.Emitting = true;
	}

	private Vector3 AdjustMoveVectorBySlope(Vector3 moveVector, Vector3 slopeNormal)
	{
		Vector3 result = new Vector3();

		Plane p = new Plane(slopeNormal);
		result = p.Project(moveVector);
		if(result.Length() > 0)
		{
			result *= moveVector.Length() / result.Length();
		}

		return result;
	}

	private void UpdateGroundCast()
	{
		groundCast.Position = GlobalTransform.Origin;
		if (groundCast.IsColliding() && !hasLanded)
		{
			hasLanded = true;
			SpawnLandingParticles();
			GD.Print("Y Velocity: " + LinearVelocity.Y);
			landSound.Play();
		}
		else if (!groundCast.IsColliding() && hasLanded)
		{
			hasLanded = false;
		}
	}

	private void _on_area_3d_take_damage(long amount, long team)
	{
		if (team == (long)BallGame.Common.Team.Player) return;

		ProcessMode = ProcessModeEnum.Disabled;
		Node3D _deathParticles = (Node3D)deathParticles.Instantiate();
		GetParent().AddChild(_deathParticles);
		_deathParticles.GlobalPosition = GlobalPosition;
		EmitSignal("PlayerDied");
	}

	[Signal]
	public delegate void PlayerDiedEventHandler();

	private void _on_player_respawner_respawn_now()
	{
		ProcessMode = ProcessModeEnum.Inherit;
		GlobalPosition = new Vector3(0, 10, 0);
		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;
	}
}
