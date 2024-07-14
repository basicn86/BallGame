using Godot;
using System;

public partial class PhysicsBox : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_area_3d_take_damage(int amount, int team)
	{
		GD.Print("Ouch, I took: " + amount + " damage!");
		QueueFree();
	}
}
