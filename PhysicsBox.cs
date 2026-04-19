using Godot;
using System;
////
public partial class PhysicsBox : RigidBody3D
{
	[Export]
	PackedScene explosionScene;

	[Export]
	AudioStreamPlayer3D MovementRustle;

	private Vector3 previousVelocity;
	private Vector3 currentVelocity;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		previousVelocity = Vector3.Zero;
		currentVelocity = Vector3.Zero;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if ((currentVelocity - previousVelocity).Length() > 3.0f)
		{
			MovementRustle.PitchScale = (float)GD.RandRange(0.95f, 1.0f);
			MovementRustle.Play();
		}

		previousVelocity = currentVelocity;
		currentVelocity = LinearVelocity;
	}

	private void _on_area_3d_take_damage(int amount, int team, Vector3 knockback)
	{
		Node3D explosion = (Node3D)explosionScene.Instantiate();
		GetParent().AddChild(explosion);
		explosion.GlobalPosition = GlobalPosition;

		//GD.Print("Ouch, I took: " + amount + " damage!");
		QueueFree();
	}
}
