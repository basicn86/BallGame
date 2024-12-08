using Godot;
using System;

public partial class PlayerModelComponent : MeshInstance3D
{
	[Export]
	public Player player;
	[Export]
	public Timer InvisibilityTimer;

	private Vector3 previousPos;
	private Vector3 currentPos;

	//we need to use quaternions for rotation interpolation, euler angles cause snapping when the rotation overflows from 360 to 0
	private Quaternion previousRotation;
	private Quaternion currentRotation;

	public override void _Ready()
	{
		if(player == null) QueueFree();
		TopLevel = true;
		previousRotation = player.Quaternion;
		currentRotation = player.Quaternion;
		currentPos = player.GlobalPosition;
		previousPos = player.GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		SmoothPlayerMotion(delta);
	}

	private void SmoothPlayerMotion(double delta)
	{
		Quaternion = previousRotation.Slerp(currentRotation, (float)Engine.GetPhysicsInterpolationFraction());
		GlobalPosition = previousPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
	}

	public override void _PhysicsProcess(double delta)
	{
		previousPos = currentPos;
		currentPos = player.GlobalTransform.Origin;

		previousRotation = currentRotation;
		currentRotation = player.Quaternion;
	}

	private void _on_player_player_died()
	{
		InvisibilityTimer.Start();
		Visible = false;
	}

	private void _on_invisibility_timer_timeout()
	{
		Visible = true;
	}
}
