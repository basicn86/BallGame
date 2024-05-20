using Godot;
using System;

public partial class jetpack : MeshInstance3D
{
	private RigidBody3D player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<RigidBody3D>("../Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = player.Position + new Vector3(0, 0, 0.8f);
	}
}
