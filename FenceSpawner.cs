using Godot;
using System;

[Tool]
public partial class FenceSpawner : Node3D
{
	[Export] public Mesh FenceMesh;            // The fence segment mesh
	[Export] public Material FenceMaterial;        // The fence segment material
	[Export] public int Count = 10;             // How many segments
	[Export] public Vector3 Offset = Vector3.Zero; // Optional offset per segment
	[Export] public float Rotation = 0.0f;      // Optional rotation per segment
	[Export] public bool Rebuild = false;        // Button to manually rebuild

	private MultiMeshInstance3D _multiMeshInstance;

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint() && Rebuild)
		{
			RebuildFence();
			Rebuild = false;
		}

		if(!Engine.IsEditorHint())
		{
			RebuildFence();
			ProcessMode = ProcessModeEnum.Disabled;
			return;
		}
	}

	private void RebuildFence()
	{
		// Remove old MultiMeshInstance if exists
		if (_multiMeshInstance != null)
		{
			_multiMeshInstance.QueueFree();
		}

		_multiMeshInstance = new MultiMeshInstance3D();
		AddChild(_multiMeshInstance);

		MultiMesh multiMesh = new MultiMesh
		{
			Mesh = FenceMesh,
			TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
			InstanceCount = Count
		};
		multiMesh.Mesh.SurfaceSetMaterial(0, FenceMaterial);

		_multiMeshInstance.Multimesh = multiMesh;

		for (int i = 0; i < Count; i++)
		{
			Vector3 position = Offset * i;
			Transform3D xform = Transform3D.Identity.Translated(position);
			xform = xform.RotatedLocal(Vector3.Up, Mathf.DegToRad(Rotation));
			multiMesh.SetInstanceTransform(i, xform);
		}
	}
}
