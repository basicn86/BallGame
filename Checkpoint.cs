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
	private Material capturedMaterial;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_area_3d_body_entered(Node body)
	{
		particles.Emitting = false;
		mesh.MaterialOverride = capturedMaterial;
		lightNode.QueueFree();
	}
}
