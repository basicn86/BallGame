using Godot;
using System;

public partial class GrassBladeMultiMesh : MultiMeshInstance3D
{
	[Export]
	public Material Material { get; set; }

	public override void _Ready()
	{
		Multimesh.Mesh.SurfaceSetMaterial(0, Material);

		for (int i = 0; i < Multimesh.InstanceCount; i++)
		{
			Transform3D t = Multimesh.GetInstanceTransform(i);

			Basis b = new Basis(Vector3.Up, GD.Randf());

			b = b.Scaled(t.Basis.Scale);

			t.Basis = b;

			Multimesh.SetInstanceTransform(i, t);
		}
	}
}
