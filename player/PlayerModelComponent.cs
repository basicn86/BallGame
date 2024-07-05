using Godot;
using System;

public partial class PlayerModelComponent : MeshInstance3D
{
	[Export]
	public Player player;

	private Vector3 previousPos;
	private Vector3 currentPos;

	private Quaternion previousRotation;
	private Quaternion currentRotation;

	public override void _Ready()
	{
		if(player == null) QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		SmoothPlayerMotion(delta);
	}

	private void SmoothPlayerMotion(double delta)
	{
		TopLevel = true;
		//just to test the interpolation to see if it works
		if (Input.IsKeyPressed(Key.E)) {
			Quaternion = previousRotation.Slerp(currentRotation, (float)Engine.GetPhysicsInterpolationFraction());
		}
		else
		{
			Quaternion = currentRotation;
		}
		
		GlobalPosition = previousPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
	}

	public override void _PhysicsProcess(double delta)
	{
		previousPos = currentPos;
		currentPos = player.GlobalTransform.Origin;

		previousRotation = currentRotation;
		currentRotation = player.Quaternion;
		GD.Print(currentRotation.IsNormalized());
	}
}
