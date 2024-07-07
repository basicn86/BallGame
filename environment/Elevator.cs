using Godot;
using System;

public partial class Elevator : AnimatableBody3D
{

	private Vector3 spawnPosition;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spawnPosition = GlobalTransform.Origin;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition = spawnPosition + new Vector3(0f, Mathf.Sin(Engine.GetPhysicsFrames() / 15f) * 2f,0f);
	}
}
