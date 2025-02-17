using Godot;
using System;

public partial class CptDetonatorBomb : RigidBody3D
{
	[Export]
	PackedScene explosionScene;

	public GrassMultiMeshInstance3D grassMultiMeshInstance3D;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_explosion_timer_timeout()
	{
		Node3D explosion = explosionScene.Instantiate() as Node3D;
		GetParent().AddChild(explosion);
		explosion.GlobalPosition = GlobalPosition;

		grassMultiMeshInstance3D?.RemoveWithinDistance(GlobalPosition, 3.0f);

		QueueFree();
	}
}
