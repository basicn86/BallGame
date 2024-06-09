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
	private RayCast3D cameraCast;

	private Vector3 normalCameraPosition;
	private float normalCameraDistance;
	[Export]
	public Camera3D camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerModel = player.playerModel;

		cameraCast.TargetPosition = camera.Position;
		cameraCast.AddExceptionRid(player.GetRid());

		normalCameraPosition = camera.Position;
		normalCameraDistance = camera.Position.DistanceTo(new Vector3());

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
		float horizontalAxis = Input.GetAxis("look_left", "look_right") * 1500f * (float)delta;
		float verticalAxis = Input.GetAxis("look_up", "look_down") * 1500f * (float)delta;
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
	/// </summary>
	private void MoveCameraAwayFromEnvironment()
	{

		if (cameraCast.IsColliding())
		{
			Vector3 localCollisionPoint = cameraCast.GetCollisionPoint() - cameraCast.GlobalPosition;
			float localDistance = localCollisionPoint.Length();
			camera.Position = normalCameraPosition * (localDistance / normalCameraDistance) * 0.9f;
		}
		else
			camera.Position = normalCameraPosition;
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
