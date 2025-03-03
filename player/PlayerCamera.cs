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
		FreeLook,
		EnteringLockOn,
		LockOn
	}
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

		normalCameraPosition = camera.Position;
		normalCameraRotation = camera.Rotation;
		normalCameraDistance = camera.Position.DistanceTo(new Vector3());

		normalObstacleRaycastPosition = crosshairRaycast.Position; //must be relative to the camera
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GlobalPosition.Lerp(TargetPosition, 20.0f * (float)delta);

		switch(cameraMode)
		{
			case CameraMode.FreeLook:
				HandleJoystickCameraRotation(delta);
				camera.GlobalPosition = FreeLookPedestal.GlobalPosition;
				camera.GlobalRotation = FreeLookPedestal.GlobalRotation;
				break;
			case CameraMode.EnteringLockOn:
				EnteringLockOnState(delta);
				break;
			case CameraMode.LockOn:
				LockOnState();
				break;
		}

		//TODO: reimplement this
		//MoveCameraAwayFromEnvironment();

		if (Input.IsKeyLabelPressed(Key.E))
		{
			if (cameraMode != CameraMode.FreeLook) return;
			Node3D temp = GetLockOnTarget();
			if (temp != null)
			{
				lockOnTargetInfo.Target = temp;
				cameraMode = CameraMode.EnteringLockOn;
				camera.Reparent(LockOnPedestal);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (lockOnTargetInfo.IsAlive())
		{
			lockOnTargetInfo.UpdatePosition();
		}
	}

	private void EnteringLockOnState(double delta)
	{
		if (!lockOnTargetInfo.IsAlive())
		{
			cameraMode = CameraMode.FreeLook;
			return;
		}

		Vector3 lockOnPos = lockOnTargetInfo.GetInterpolatedPos();
		LockOnPedestal.LookAt(lockOnPos);
		lockOnPos.Y = GlobalPosition.Y;
		LookAt(lockOnPos, Vector3.Up);

		camera.GlobalPosition = camera.GlobalPosition.Lerp(LockOnPedestal.GlobalPosition, 10f * (float)delta);
		Basis targetRotation = LockOnPedestal.GlobalBasis;
		targetRotation = targetRotation.Orthonormalized();
		camera.GlobalTransform = new Transform3D(
			camera.GlobalBasis.Slerp(targetRotation, MathF.Min(10f * (float)delta, 1f)),
			camera.GlobalTransform.Origin
		);

		if (camera.GlobalPosition.DistanceTo(LockOnPedestal.GlobalPosition) < 0.1f &&
			camera.GlobalBasis.Z.Dot(LockOnPedestal.GlobalBasis.Z) > 0.9f)
		{
			cameraMode = CameraMode.LockOn;
		}
	}

	private void LockOnState()
	{
		if (!lockOnTargetInfo.IsAlive())
		{
			cameraMode = CameraMode.FreeLook;
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
	private void MoveCameraAwayFromEnvironment()
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
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
		{
			if (cameraMode != CameraMode.FreeLook) return;

			RotateCamera(
				((InputEventMouseMotion)@event).Relative.X,
				((InputEventMouseMotion)@event).Relative.Y
			);
		}
	}
}
