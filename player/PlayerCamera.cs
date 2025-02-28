using Godot;
using Godot.Collections;
using System;

public partial class PlayerCamera : Node3D, IPlayerCamera
{
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
	private float normalCameraDistance;
	private Vector3 normalObstacleRaycastPosition;
	private Vector3 desiredCameraLocalPosition;

	[Export]
	public Camera3D camera;

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
	#endregion

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		lockOnRaycast.Enabled = false;

		obstacleRaycast.TargetPosition = camera.Position + new Vector3(0, -0.1f, 0);

		normalCameraPosition = camera.Position;
		normalCameraDistance = camera.Position.DistanceTo(new Vector3());

		normalObstacleRaycastPosition = crosshairRaycast.Position; //must be relative to the camera

		//capture the mouse
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GlobalPosition.Lerp(TargetPosition, 20.0f * (float)delta);

		MoveCameraAwayFromEnvironment();
		camera.Position = camera.Position.Lerp(desiredCameraLocalPosition, 20.0f * (float)delta);

		ToggleFullscreen();

		if (Input.IsActionJustPressed("ui_cancel")) Input.MouseMode = Input.MouseModeEnum.Visible;

		HandleJoystickCameraRotation(delta);

		if (Input.IsKeyLabelPressed(Key.E))
		{
			Node3D lockOnTarget = GetLockOnTarget();
			if (lockOnTarget != null) GD.Print("Locking on to " + lockOnTarget.Name);
		}
	}

	private void ToggleFullscreen()
	{
		//TODO: move to UI code, doesnt make sense to have it here
		if (Input.IsActionJustPressed("fullscreen"))
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;

			//Exclusive fullscreen is needed for FreeSync/G-Sync to work
			if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
			else
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
		}
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
			RotateCamera(
				((InputEventMouseMotion)@event).Relative.X,
				((InputEventMouseMotion)@event).Relative.Y
			);
		}
	}
}
