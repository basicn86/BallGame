using Godot;
using System;

public partial class PlayerCamera : Node3D
{
	private float sensitivity = 0.1f;
	private Node3D player;
	private Node3D pitch;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Node3D>("../Player/MeshInstance3D");
		pitch = GetNode<Node3D>("PitchPivot");
		//capture the mouse
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = Position.Lerp(player.GlobalTransform.Origin, 10.0f * (float)delta);

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
		{
			RotationDegrees += new Vector3(0, -((InputEventMouseMotion)@event).Relative.X * sensitivity, 0);
			pitch.RotationDegrees += new Vector3(-((InputEventMouseMotion)@event).Relative.Y * sensitivity, 0, 0);

			pitch.RotationDegrees = new Vector3(
				Mathf.Clamp(pitch.RotationDegrees.X, -80f, 80f),
				pitch.RotationDegrees.Y,
				pitch.RotationDegrees.Z
		   );
		}
	}
}
