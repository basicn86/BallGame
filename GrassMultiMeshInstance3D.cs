using Godot;
using System;

public partial class GrassMultiMeshInstance3D : MultiMeshInstance3D
{
	[Export]
	public Material Material { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < Multimesh.InstanceCount; i++)
		{
			Multimesh.Mesh.SurfaceSetMaterial(0, Material);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
