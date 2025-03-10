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
	enum CameraMode
	{
		EnteringFreeLook,
		FreeLook,
		EnteringLockOn,
		LockOn
	}

	private double _stateTransitionTimer = 0;
	private class LockOnTargetInfo
	{
		private Node3D target;
		public Node3D Target
		{
			get { return target; }
			set
			{
				target = value;
				prevPos = target.GlobalPosition;
				currentPos = target.GlobalPosition;
			}
		}
		private Vector3 prevPos;
		private Vector3 currentPos;

		public bool IsAlive()
		{
			return IsInstanceValid(Target);
		}
		public void UpdatePosition()
		{
			prevPos = currentPos;
			currentPos = Target.GlobalPosition;
		}
		public Vector3 GetInterpolatedPos()
		{
			return prevPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
		}
	}
	CameraMode cameraMode = CameraMode.FreeLook;

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
	private Vector3 normalCameraRotation;
	private float normalCameraDistance;
	private Vector3 normalObstacleRaycastPosition;
	private Vector3 desiredCameraLocalPosition;
	private LockOnTargetInfo lockOnTargetInfo = new LockOnTargetInfo();

	[Export]
	public Camera3D camera;

	//These are nodes that the camera will sit on.
	[ExportCategory("Pedestals")]
	[Export]
	private Node3D FreeLookPedestal;
	[Export]
	private Node3D LockOnPedestal;

	#region Publicly accessible properties
	public float Pitch
	{
		get { return pitch.Rotation.X; }
	}
	/// <summary>
	/// Gets the collision point of the crosshair raycast if it is colliding with something. If it is not colliding with anything, it returns the ending point of the crosshair raycast. This allows us to get the point where the player is aiming at, even if the crosshair is not colliding with anything.
	/// </summary>
	public Vector3 GetCrosshairCollisionPoint()
	{
		if (crosshairRaycast.IsColliding())
		{
			return crosshairRaycast.GetCollisionPoint();
		}
		else
		{
			return crosshairNoncollidingPoint.GlobalTransform.Origin;
		}
	}

	public Node3D GetLockOnTarget()
	{
		//This is to prevent locking on through walls
		Vector3 targetPosition = GetCrosshairCollisionPoint();
		lockOnRaycast.TargetPosition = new Vector3(0f,0f, -((targetPosition - crosshairRaycast.GlobalPosition).LengthSquared()));

		lockOnRaycast.ForceRaycastUpdate();
		if (!lockOnRaycast.IsColliding()) return null;
		return lockOnRaycast.GetCollider() as Node3D;
	}
	public RayCast3D CrosshairRaycast
	{
		get { return crosshairRaycast; }
	}

	public void Activate()
	{
		camera.Current = true;
	}

	public void ResetPosition(Vector3 targetPosition)
	{
		camera.GlobalPosition = targetPosition;
	}
	public void ResetRotation(Vector3 rotation)
	{
		camera.GlobalRotation = rotation;
	}
	#endregion

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		lockOnRaycast.Enabled = false;

		obstacleRaycast.TargetPosition = camera.Position + new Vector3(0, -0.1f, 0);

		normalCameraPosition = FreeLookPedestal.Position;
		normalCameraRotation = FreeLookPedestal.Rotation;
		normalCameraDistance = FreeLookPedestal.Position.DistanceTo(new Vector3());

		normalObstacleRaycastPosition = crosshairRaycast.Position; //must be relative to the camera
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
		}

		if (Input.IsActionJustPressed("target_lock"))
		{
			if (cameraMode == CameraMode.FreeLook) {
				Node3D temp = GetLockOnTarget();
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
	}

	private void LockOn(Node3D target)
	{
		lockOnTargetInfo.Target = target;
		_stateTransitionTimer = 0;
		cameraMode = CameraMode.EnteringLockOn;
		camera.Reparent(LockOnPedestal);

		if (target is LockOnArea lockOnArea)
		{
			lockOnArea.TargetLocked();
		}
	}

	private void StopLockOn()
	{
		if (lockOnTargetInfo.Target is LockOnArea lockOnArea && IsInstanceValid(lockOnArea))
		{
			lockOnArea.TargetUnlocked();
		}
		cameraMode = CameraMode.EnteringFreeLook;
		_stateTransitionTimer = 0;
		camera.Reparent(FreeLookPedestal);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (lockOnTargetInfo.IsAlive())
		{
			lockOnTargetInfo.UpdatePosition();
		}
	}

	private void EnteringFreeLookState(double delta)
	{
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
		if (!lockOnTargetInfo.IsAlive())
		{
			cameraMode = CameraMode.FreeLook;
			_stateTransitionTimer = 0;
			return;
		}

		_stateTransitionTimer += delta;

		Vector3 lockOnPos = lockOnTargetInfo.GetInterpolatedPos();
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
		if (!lockOnTargetInfo.IsAlive())
		{
			cameraMode = CameraMode.EnteringFreeLook;
			_stateTransitionTimer = 0;
			return;
		}
		Vector3 lockOnPos = lockOnTargetInfo.GetInterpolatedPos();
		LockOnPedestal.LookAt(lockOnPos);
		lockOnPos.Y = pitch.GlobalPosition.Y;
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
		RotationDegrees += new Vector3(0, -X * sensitivity * UIScale, 0);
		pitch.RotationDegrees += new Vector3(-Y * sensitivity * UIScale, 0, 0);

		pitch.RotationDegrees = new Vector3(
			Mathf.Clamp(pitch.RotationDegrees.X, -80f, 80f),
			pitch.RotationDegrees.Y,
			pitch.RotationDegrees.Z
	   );
	}

	/// <summary>
	/// Adjusts the camera's position to avoid clipping into the environment.
	/// If the camera is colliding with the environment, it calculates the distance to the collision point
	/// and moves the camera closer to the player. If the camera is not colliding with anything, it sets the camera's position
	/// to its normal position.
	/// We also need to update the crosshairRaycast position to match the camera position when the camera is colliding with the environment. When it is not colliding with the environment, we set the crosshairRaycast position to its normal position, which is usually a little bit ahead of the player. This prevents entities from being selected when an entity walks in between the player and the camera, and prevents the player from attacking backwards.
	/// </summary>
	private void MoveCameraAwayFromEnvironment(double delta)
	{
		obstacleRaycast.ForceRaycastUpdate();
		if (obstacleRaycast.IsColliding())
		{
			Vector3 localCollisionPoint = obstacleRaycast.GetCollisionPoint() - obstacleRaycast.GlobalPosition;
			float localDistance = localCollisionPoint.Length();
			desiredCameraLocalPosition = normalCameraPosition * (localDistance / normalCameraDistance);
			//move the camera up a little bit when it is colliding with the environment
			//TODO: possibly replace this with a curve rather than an equation
			desiredCameraLocalPosition += new Vector3(0, 0.5f * Math.Clamp((1f-(localDistance/normalCameraDistance))*2f, 0f, 1f), 0);

			crosshairRaycast.Position = normalObstacleRaycastPosition * (localDistance / normalCameraDistance);
		}
		else
		{
			desiredCameraLocalPosition = normalCameraPosition;
			crosshairRaycast.Position = normalObstacleRaycastPosition;
		}
		FreeLookPedestal.Position = FreeLookPedestal.Position.Lerp(desiredCameraLocalPosition, 20f * (float)delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
		{
			switch (cameraMode)
			{
				case CameraMode.FreeLook:
				case CameraMode.EnteringFreeLook:
					RotateCamera(
						((InputEventMouseMotion)@event).Relative.X,
						((InputEventMouseMotion)@event).Relative.Y
					);
					break;
				default:
					break;
			}
		}
	}
}
