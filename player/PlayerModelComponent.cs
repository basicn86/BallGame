using Godot;
using System;

public partial class PlayerModelComponent : MeshInstance3D
{
	[Export]
	public Player player;

	private Vector3 previousPos;
	private Vector3 currentPos;

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
		Rotation = player.Rotation;
		GlobalPosition = previousPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
	}

	public override void _PhysicsProcess(double delta)
	{
		previousPos = currentPos;
		currentPos = player.GlobalTransform.Origin;
	}
}
