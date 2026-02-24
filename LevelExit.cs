using Godot;
using System;

public partial class LevelExit : Node3D
{
	[Export]
	public Node3D ExitSign; //Spin this around.

	private float rotationSpeed = 90f;

	public override void _Ready()
	{
		if (ExitSign == null)
		{
			GD.PrintErr("LevelExit: ExitSign not assigned!");
			QueueFree();
			return;
		}
	}

	public override void _Process(double delta)
	{
		ExitSign.RotateY(Mathf.DegToRad(rotationSpeed * (float)delta));
	}

	public void _on_player_detection_area_body_entered(Node3D body)
	{
		if (body is not Player) return;
		GD.Print("Exit reached.");
		QueueFree();
	}
}
