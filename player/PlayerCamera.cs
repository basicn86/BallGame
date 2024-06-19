using Godot;
using System;

public partial class PlayerCamera : Node3D
{
	private float sensitivity = 0.1f;

	[Export]
	public Player player;
	private Node3D playerModel;
	[Export]
	private Node3D pitch;
	[Export]
	private RayCast3D obstacleRaycast;
	[Export]
	private RayCast3D crosshairRaycast;

	public RayCast3D CrosshairRaycast
	{
		get { return crosshairRaycast; }
	}

	private Vector3 normalCameraPosition;
	private float normalCameraDistance;
	private Vector3 normalObstacleRaycastPosition;

	[Export]
	public Camera3D camera;

	public float Pitch
	{
		get { return pitch.Rotation.X; }
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerModel = player.playerModel;

		obstacleRaycast.TargetPosition = camera.Position + new Vector3(0, -0.1f, 0);
		obstacleRaycast.AddExceptionRid(player.GetRid());

		normalCameraPosition = camera.Position;
		normalCameraDistance = camera.Position.DistanceTo(new Vector3());

		normalObstacleRaycastPosition = crosshairRaycast.Position; //must be relative to the camera
		crosshairRaycast.AddExceptionRid(player.GetRid());

		//capture the mouse
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = Position.Lerp(playerModel.GlobalTransform.Origin, 20.0f * (float)delta);

		MoveCameraAwayFromEnvironment();

		ToggleFullscreen();

		if (Input.IsActionJustPressed("ui_cancel")) Input.MouseMode = Input.MouseModeEnum.Visible;

		HandleJoystickCameraRotation(delta);
	}

	private void ToggleFullscreen()
	{
		if (Input.IsActionJustPressed("fullscreen"))
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;

			//Exclusive fullscreen is needed for FreeSync/G-Sync to work
			if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
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
		RotationDegrees += new Vector3(0, -X * sensitivity, 0);
		pitch.RotationDegrees += new Vector3(-Y * sensitivity, 0, 0);

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

		if (obstacleRaycast.IsColliding())
		{
			Vector3 localCollisionPoint = obstacleRaycast.GetCollisionPoint() - obstacleRaycast.GlobalPosition;
			float localDistance = localCollisionPoint.Length();
			camera.Position = normalCameraPosition * (localDistance / normalCameraDistance);
			//move the camera up a little bit when it is colliding with the environment
			camera.Position += new Vector3(0, 0.5f * Math.Clamp((1f-(localDistance/normalCameraDistance))*2f, 0f, 1f), 0);

			crosshairRaycast.Position = normalObstacleRaycastPosition * (localDistance / normalCameraDistance);
		}
		else
		{
			camera.Position = normalCameraPosition;
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
