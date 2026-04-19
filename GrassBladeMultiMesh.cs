using Godot;
using System;

public partial class GrassBladeMultiMesh : MultiMeshInstance3D
{
	[Export]
	public Material Material { get; set; }

	public override void _Ready()
	{
		Multimesh.Mesh.SurfaceSetMaterial(0, Material);
	}
}
