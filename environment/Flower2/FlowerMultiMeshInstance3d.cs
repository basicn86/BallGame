using Godot;
using System;

public partial class FlowerMultiMeshInstance3d : MultiMeshInstance3D
{
	[Export]
	public Material PetalMaterial;
	[Export]
	public Material StemMaterial;
	[Export]
	public Material PistilMaterial;
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
		Multimesh.Mesh.SurfaceSetMaterial(0, PetalMaterial);
		Multimesh.Mesh.SurfaceSetMaterial(1, PistilMaterial);
		Multimesh.Mesh.SurfaceSetMaterial(2, PetalMaterial);
		Multimesh.Mesh.SurfaceSetMaterial(3, PetalMaterial);
		Multimesh.Mesh.SurfaceSetMaterial(4, PetalMaterial);
		Multimesh.Mesh.SurfaceSetMaterial(5, PetalMaterial);
		Multimesh.Mesh.SurfaceSetMaterial(6, PetalMaterial);
		Multimesh.Mesh.SurfaceSetMaterial(7, StemMaterial);

		for(int i = 0; i < Multimesh.InstanceCount; i++)
		{
			Transform3D transform = Multimesh.GetInstanceTransform(i);

			transform = transform.LookingAt(transform.Origin + new Vector3(1,0,-1), new Vector3(0, 1, 0));
			transform = transform.ScaledLocal(new Vector3(0.25f, 0.25f, 0.25f));
			transform = transform.Translated(new Vector3(0, 0.7f, 0));


			Multimesh.SetInstanceTransform(i, transform);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
