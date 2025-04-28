using Godot;
using System;

[Tool]
public partial class FenceSpawner : Node3D
{
	[Export] public Mesh FenceMesh;            // The fence segment mesh
	[Export] public int Count = 10;             // How many segments
	[Export] public float Spacing = 2.0f;        // Spacing between segments
	[Export] public Vector3 Offset = Vector3.Zero; // Optional offset per segment
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

		_multiMeshInstance.Multimesh = multiMesh;

		for (int i = 0; i < Count; i++)
		{
			Vector3 position = new Vector3(i * Spacing, 0, 0) + Offset * i;
			Transform3D xform = Transform3D.Identity.Translated(position);
			multiMesh.SetInstanceTransform(i, xform);
		}
	}
}
