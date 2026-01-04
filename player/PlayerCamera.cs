using Godot;
using Godot.Collections;
using System;

//Camera Requirements:
//The camera should lerp to the player's position
//When in free look mode, the rotation should have no lerping
//When locking onto an enemy, the camera should lerp to its lock-on position
//The camera should not clip into the environment, when it does, lerp the camera towards the player
public partial class PlayerCamera : Node3D
{
	public static PlayerCamera Instance = null;

	enum CameraMode
	{
		EnteringFreeLook,
		FreeLook,
		EnteringLockOn,
		LockOn,
		Automatic
	}

	private double _stateTransitionTimer = 0;

	CameraMode cameraMode = CameraMode.Automatic;

	private float sensitivity = 0.1f;

	public Vector3 TargetPosition { get; set; }
	[Export]
	private Node3D pitch;
	[Export]
	private RayCast3D obstacleRaycast;
	[Export]
	private RayCast3D crosshairRaycast;
	[Export]
	private RayCast3D lockOnRaycast;
	[Export]
	private Node3D crosshairNoncollidingPoint;

	//default camera position and distance
	private Vector3 normalCameraPosition;
	private LockOnArea? lockOnTarget;

	[Export]
	public Camera3D camera;

	//These are nodes that the camera will sit on.
	[ExportCategory("Pedestals")]
	[Export]
	private Node3D FreeLookPedestal;
	[Export]
	private Node3D LockOnPedestal;
	[Export]
	private Node3D AutomaticPedestal;

	public Vector3 RespawnPosition;

	#region Publicly accessible properties
	/// <summary>
	/// Gets the collision point of the crosshair raycast if it is colliding with something. If it is not colliding with anything, it returns the ending point of the crosshair raycast. This allows us to get the point where the player is aiming at, even if the crosshair is not colliding with anything.
	/// </summary>
	public Vector3 GetCrosshairCollisionPoint()
	{
		if (cameraMode == CameraMode.LockOn || cameraMode == CameraMode.EnteringLockOn)
		{
			return lockOnTarget.GetInterpolatedPos();
		}
		else if (crosshairRaycast.IsColliding())
		{
			return crosshairRaycast.GetCollisionPoint();
		}
		else
		{
			return crosshairNoncollidingPoint.GlobalTransform.Origin;
		}
	}

	public LockOnArea GetLockOnTarget()
	{
		//This is to prevent locking on through walls
		Vector3 targetPosition = GetCrosshairCollisionPoint();
		lockOnRaycast.GlobalPosition = crosshairRaycast.GlobalPosition;
		lockOnRaycast.TargetPosition = targetPosition - lockOnRaycast.GlobalPosition;

		lockOnRaycast.ForceRaycastUpdate();
		if (!lockOnRaycast.IsColliding()) return null;
		if (lockOnRaycast.GetCollider() is not LockOnArea) return null;
		return lockOnRaycast.GetCollider() as LockOnArea;
	}
	
	public Basis GetCameraRotation()
	{
		Basis cameraRotation = new Basis(Vector3.Up, camera.GlobalRotation.Y);
		return cameraRotation;
	}
	#endregion

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(Instance != null)
		{
			GD.PrintErr("Multiple PlayerCamera instances detected! Deleting extra instance.");
			QueueFree();
			return;
		}

		Instance = this;

		camera.Current = true;

		lockOnRaycast.Enabled = false;

		obstacleRaycast.TargetPosition = camera.Position - obstacleRaycast.Position;

		normalCameraPosition = FreeLookPedestal.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GlobalPosition.Lerp(TargetPosition, 20.0f * (float)delta);

		switch(cameraMode)
		{
			case CameraMode.EnteringFreeLook:
				EnteringFreeLookState(delta);
				break;
			case CameraMode.FreeLook:
				FreeLookState(delta);
				break;
			case CameraMode.EnteringLockOn:
				EnteringLockOnState(delta);
				break;
			case CameraMode.LockOn:
				LockOnState();
				break;
			case CameraMode.Automatic:
				Automatic(delta);
				break;
			default:
				break;
		}

		if (Input.IsActionJustPressed("target_lock"))
		{
			if (cameraMode == CameraMode.FreeLook || cameraMode == CameraMode.Automatic) {
				LockOnArea temp = GetLockOnTarget();
				if (temp != null)
				{
					LockOn(temp);
				}
			}
			else if (cameraMode == CameraMode.LockOn)
			{
				StopLockOn();
			}
		}

		if (Input.IsActionJustPressed("debugf2"))
		{
			if(cameraMode == CameraMode.Automatic)
			{
				_stateTransitionTimer = 0;
				cameraMode = CameraMode.EnteringFreeLook;
			} else if (cameraMode == CameraMode.FreeLook)
			{
				AutomaticPedestal.GlobalPosition = LockOnPedestal.GlobalPosition;
				cameraMode = CameraMode.Automatic;
			}
		}
	}

	private void Automatic(double delta)
	{
		HandleJoystickCameraRotation(delta);

		float distance = AutomaticPedestal.GlobalPosition.DistanceTo(TargetPosition + new Vector3(0, 2, 0));
		float maxDistance = 6.0f;

		if (distance > maxDistance)
		{
			float speedRatio = distance - maxDistance;
			speedRatio = Mathf.Clamp(speedRatio, 0.0f, 1.0f);

			AutomaticPedestal.GlobalPosition = AutomaticPedestal.GlobalPosition.Lerp(TargetPosition + new Vector3(0, 2, 0), 2f * (float)(delta * speedRatio));

		} else if (distance < 3.5f)
		{
			float speedRatio = 3.5f - distance;
			Vector3 directionToTarget = (AutomaticPedestal.GlobalPosition - TargetPosition);
			directionToTarget.Y = 0;
			directionToTarget = directionToTarget.Normalized();
			AutomaticPedestal.GlobalPosition += directionToTarget * 6f * speedRatio * (float)delta;
		}

		camera.GlobalPosition = AutomaticPedestal.GlobalPosition;
		Vector3 lookAtPos = TargetPosition;
		lookAtPos.Y = camera.GlobalPosition.Y + (TargetPosition.Y - camera.GlobalPosition.Y) * 0.5f;
		camera.LookAt(lookAtPos);
	}

	private void LockOn(LockOnArea target)
	{
		lockOnTarget = target;
		_stateTransitionTimer = 0;
		cameraMode = CameraMode.EnteringLockOn;
		target.TargetLocked();
	}

	private void StopLockOn()
	{
		if (IsInstanceValid(lockOnTarget))
		{
			lockOnTarget.TargetUnlocked();
		}
		cameraMode = CameraMode.Automatic;
		_stateTransitionTimer = 0;
	}

	private void EnteringFreeLookState(double delta)
	{
		Crosshair.Instance.Visible = true;

		pitch.RotationDegrees = new Vector3(LockOnPedestal.GlobalRotationDegrees.X + 10f, 0.0f, 0.0f);
		_stateTransitionTimer += delta * 2.0;
		camera.GlobalPosition = camera.GlobalPosition.Lerp(FreeLookPedestal.GlobalPosition, (float)_stateTransitionTimer);

		Basis targetBasis = FreeLookPedestal.GlobalBasis;
		targetBasis = targetBasis.Orthonormalized();
		camera.GlobalTransform = new Transform3D(
			camera.GlobalBasis.Slerp(targetBasis, (float)_stateTransitionTimer),
			camera.GlobalTransform.Origin
		);

		HandleJoystickCameraRotation(delta);

		if (_stateTransitionTimer > 0.5)
		{
			cameraMode = CameraMode.FreeLook;
		}
	}

	private void FreeLookState(double delta)
	{
		HandleJoystickCameraRotation(delta);
		camera.GlobalPosition = FreeLookPedestal.GlobalPosition;
		camera.GlobalRotation = FreeLookPedestal.GlobalRotation;
		MoveCameraAwayFromEnvironment(delta);
	}

	private void EnteringLockOnState(double delta)
	{
		Crosshair.Instance.Visible = false;

		if (!lockOnTarget.CanProcess())
		{
			StopLockOn();
			return;
		}

		_stateTransitionTimer += delta;

		Vector3 lockOnPos = lockOnTarget.GetInterpolatedPos();
		LockOnPedestal.LookAt(lockOnPos);
		lockOnPos.Y = GlobalPosition.Y;
		LookAt(lockOnPos, Vector3.Up);

		camera.GlobalPosition = camera.GlobalPosition.Lerp(LockOnPedestal.GlobalPosition, (float)_stateTransitionTimer);
		Basis targetRotation = LockOnPedestal.GlobalBasis;
		targetRotation = targetRotation.Orthonormalized();
		camera.GlobalTransform = new Transform3D(
			camera.GlobalBasis.Slerp(targetRotation, (float)_stateTransitionTimer),
			camera.GlobalTransform.Origin
		);

		if (_stateTransitionTimer >= 0.5)
		{
			_stateTransitionTimer = 0;
			cameraMode = CameraMode.LockOn;
		}
	}

	private void LockOnState()
	{
		if (!lockOnTarget.CanProcess())
		{
			StopLockOn();
			return;
		}
		Vector3 lockOnPos = lockOnTarget.GetInterpolatedPos();
		LockOnPedestal.LookAt(lockOnPos);
		lockOnPos.Y = GlobalPosition.Y;
		LookAt(lockOnPos, Vector3.Up);
		camera.GlobalPosition = LockOnPedestal.GlobalPosition;
		camera.GlobalRotation = LockOnPedestal.GlobalRotation;
	}

	private void HandleJoystickCameraRotation(double delta)
	{
		float horizontalAxis = Input.GetAxis("look_left", "look_right") * 2000f * (float)delta;
		float verticalAxis = Input.GetAxis("look_up", "look_down") * 2000f * (float)delta;
		RotateCamera(horizontalAxis, verticalAxis);
	}

	private void RotateCamera(float X, float Y)
	{
		//this is stupid, but the UI scaling also affects the mouse sensitivity, so we need to do this to keep the mouse sensitivity consistent across different UI scales
		float UIScale = GetWindow().ContentScaleFactor;
		float MinCamDistance = 2.5f;

		switch (cameraMode)
		{
			case CameraMode.EnteringFreeLook:
			case CameraMode.FreeLook:
				RotationDegrees += new Vector3(0, -X * sensitivity * UIScale, 0);
				pitch.RotationDegrees += new Vector3(-Y * sensitivity * UIScale, 0, 0);

				pitch.RotationDegrees = new Vector3(
					Mathf.Clamp(pitch.RotationDegrees.X, -80f, 80f),
					pitch.RotationDegrees.Y,
					pitch.RotationDegrees.Z
			   );
				break;
			case CameraMode.Automatic:
				Vector3 rightVector = -GetCameraRotation().X;
				Vector3 directionToPlayer = (AutomaticPedestal.GlobalPosition - TargetPosition);
				AutomaticPedestal.GlobalPosition += rightVector * X * sensitivity * UIScale * 0.1f;
				if (Y > 0.0f) //camera goes towards player
				{
					AutomaticPedestal.GlobalPosition += directionToPlayer.Normalized() * Y * sensitivity * UIScale * 0.04f;
				} else if (Y < 0.0f) //camera goes away from player
				{
					AutomaticPedestal.GlobalPosition += directionToPlayer.Normalized() * Y * sensitivity * UIScale * 0.04f * Mathf.Clamp((directionToPlayer.Length() - MinCamDistance), 0.0f, 1.0f);
				}
				break;
			default:
				break;
		}
	}

	public void ResetCameraPosition()
	{
		switch (cameraMode)
		{
			case CameraMode.FreeLook:
				break;
			case CameraMode.Automatic:
				AutomaticPedestal.GlobalPosition = RespawnPosition;
				break;
			default:
				break;
		}
	}

	private void MoveCameraAwayFromEnvironment(double delta)
	{
		obstacleRaycast.ForceRaycastUpdate();
		if (obstacleRaycast.IsColliding())
		{
			Vector3 desiredPosition = obstacleRaycast.GetCollisionPoint();
			desiredPosition -= (desiredPosition - obstacleRaycast.GlobalPosition).Normalized() * 0.2f;
			FreeLookPedestal.GlobalPosition = FreeLookPedestal.GlobalPosition.Lerp(desiredPosition, 20f * (float)delta);
		} else
		{
			FreeLookPedestal.Position = FreeLookPedestal.Position.Lerp(normalCameraPosition, 20f * (float)delta);
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
		{
			RotateCamera(
				((InputEventMouseMotion)@event).Relative.X,
				((InputEventMouseMotion)@event).Relative.Y
			);
		}
		@event.Dispose();
	}
}
