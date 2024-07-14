using Godot;
using System;

public partial class MovingPlatform : AnimatableBody3D
{
	[Export]
	public PathFollow3D PathFollow;
	private float direction = 1f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (PathFollow == null) QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (PathFollow.ProgressRatio >= 1f || PathFollow.ProgressRatio <= 0f) direction *= -1f;
		PathFollow.Progress += (float)delta * direction;
		//While this doesn't make any sense, this line is required to update all the children's positions. PathFollow3D doesn't update the children's positions automatically. See issue #58269 and #63140. Omitting this line will cause the collision shapes to not move with the platform.
		GlobalPosition = GlobalPosition;
	}
}
