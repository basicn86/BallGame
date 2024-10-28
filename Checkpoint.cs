using Godot;
using System;

public partial class Checkpoint : Node3D
{
	[Export]
	private GpuParticles3D particles;
	[Export]
	private MeshInstance3D mesh;
	[Export]
	private Node3D lightNode;

	[Export]
	public float RotationSpeed = 2.0f;

	[Export]
	public float PlayerNearbyRotationSpeed = 10.0f;

	private bool playerNearby = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (playerNearby)
		{
			mesh.RotateY((float)delta * PlayerNearbyRotationSpeed);
		}
		else
		{
			mesh.RotateY((float)delta * RotationSpeed);
		}
	}

	private void _on_area_3d_body_entered(Node body)
	{
		if (body is not Player) return;
		Player player = (Player)body;
		player.UpdateRespawnPos(GlobalPosition);
		playerNearby = true;
		particles.Emitting = false;
		if(IsInstanceValid(lightNode)) lightNode.QueueFree();
	}

	private void _on_area_3d_body_exited(Node body)
	{
		if (body is not Player) return;
		playerNearby = false;
	}
}
