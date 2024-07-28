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


	private bool hasLanded = false;

	[ExportCategory("Weapons")]
	[Export]
	LaserPistol laserPistol;
	[Export]
	GrenadeThrower grenadeThrower;

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

		RunFpsDebug();

		cameraNode.TargetPosition = playerModel.Transform.Origin;

		laserPistol.UpdatePosition(GlobalPosition, cameraNode.Basis);
		if (Input.IsActionJustPressed("attack"))
		{
			laserPistol.Fire(cameraNode.GetCrosshairCollisionPoint());
		}

		grenadeThrower.UpdatePosition(GlobalPosition, cameraNode.Basis);
		if (Input.IsActionJustPressed("throw_grenade"))
		{
			grenadeThrower.Fire(cameraNode.GetCrosshairCollisionPoint());
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
			ApplyCentralForce(-vel * NoInputDeceleration * (float)delta);
			return;
		}

		if (groundCast.IsColliding()) moveVector = AdjustMoveVectorBySlope(moveVector, groundCast.GetCollisionNormal());

		if (moveVector.Length() > 1f) moveVector = moveVector.Normalized();

		if (groundCast.IsColliding())
			ApplyCentralForce(moveVector * GroundAcceleration * (float)delta);
		else
			ApplyCentralForce(moveVector * AirAcceleration * (float)delta);
	}

	private void RestrictPlayerVelocity(double delta)
	{
		Vector3 velocity = LinearVelocity;
		velocity.Y = 0; //don't restrict vertical velocity
		if (velocity.Length() > MaxVelocity) ApplyCentralForce(-velocity.Normalized() * MaxVelocityPushback * (float)delta);
	}

	private void RunFpsDebug()
	{
		
		if (Input.IsKeyPressed(Key.Key1))
		{
			GD.Print("Set FPS to 120");
			Engine.MaxFps = 122;
		}
		if (Input.IsKeyPressed(Key.Key2))
		{
			GD.Print("Set FPS to 60");
			Engine.MaxFps = 60;
		}
		if (Input.IsKeyPressed(Key.Key3))
		{
			GD.Print("Set FPS to 30");
			Engine.MaxFps = 30;
		}
		if (Input.IsKeyPressed(Key.Key4))
		{
			GD.Print("Set FPS to 165");
			Engine.MaxFps = 165;
		}
		//potato mode
		if(Input.IsKeyPressed(Key.Key5))
		{
			//set the shadow res size to 256
			RenderingServer.DirectionalShadowAtlasSetSize(256, true);
			RenderingServer.DirectionalSoftShadowFilterSetQuality(RenderingServer.ShadowQuality.Hard);
			//turns off MSAA
			GetViewport().Msaa3D = Viewport.Msaa.Disabled;
			//unlock FPS
			Engine.MaxFps = 0;
		}
	}

	private void TryJumping()
	{
		if (Input.IsActionJustPressed("jump") && groundCast.IsColliding())
		{
			ApplyCentralImpulse(new Vector3(0, 9f, 0));
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



