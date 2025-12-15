using Godot;
using System;

[Tool]
public partial class FenceSpawnerPath : Node3D
{
	[Export] public Mesh FenceMesh;
	[Export] public Material FenceMaterial;
	[Export] public Shape3D FenceCollisionShape;
	[Export] public int Count = 10;
	[Export] public float Rotation = 0.0f;
	[Export] public float Scale = 1.0f;
	[Export] public bool Rebuild = false;

	[Export] public NodePath PathNode; // The Path3D node

	private MultiMeshInstance3D _multiMeshInstance;


	private bool rebuilt = false;

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint() && Rebuild)
		{
			RebuildFence();
			Rebuild = false;
			GlobalPosition = GetNodeOrNull<Path3D>(PathNode).GlobalPosition;
		}

		if (!Engine.IsEditorHint() && !rebuilt)
		{
			RebuildFence();
			rebuilt = true;
			return;
		}
	}

	private void AddCollision(Vector3 position, Vector3 direction)
	{
		if (Engine.IsEditorHint()) return;
		StaticBody3D staticBody = new StaticBody3D();
		AddChild(staticBody);
		staticBody.GlobalPosition = position;
		staticBody.LookAt(position + direction, Vector3.Up);

		CollisionShape3D collisionShape = new CollisionShape3D();
		collisionShape.Shape = FenceCollisionShape;
		staticBody.AddChild(collisionShape);
		collisionShape.Position = Vector3.Zero;

		GD.Print("Added collision at: " + collisionShape.GlobalPosition);
	}

	private void RebuildFence()
	{
		if (_multiMeshInstance != null)
			_multiMeshInstance.QueueFree();

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

		// Get the Path3D and its Curve3D
		Path3D path = GetNodeOrNull<Path3D>(PathNode);
		if (path == null) return;
		Curve3D curve = path.Curve;
		if (curve == null) return;

		// Place each segment along the curve
		float totalLength = curve.GetBakedLength();
		for (int i = 0; i < Count; i++)
		{
			float t = (float)i / (Count - 1);
			Vector3 currentPos = curve.SampleBaked(t * totalLength);
			Vector3 nextPos;

			if (i < Count - 1)
			{
				// Point to the next segment
				float tNext = (float)(i + 1) / (Count - 1);
				nextPos = curve.SampleBaked(tNext * totalLength);
			}
			else
			{
				// Last segment: point from previous to current
				float tPrev = (float)(i - 1) / (Count - 1);
				nextPos = currentPos + (currentPos - curve.SampleBaked(tPrev * totalLength));
			}

			Vector3 direction = (nextPos - currentPos).Normalized();

			Transform3D xform = Transform3D.Identity;
			xform.Origin = currentPos;

			// Align along the segment direction
			if (!direction.IsZeroApprox())
				xform = xform.LookingAt(currentPos + direction, Vector3.Up);

			// Optional rotation and scale
			xform = xform.RotatedLocal(Vector3.Up, Mathf.DegToRad(Rotation));
			xform = xform.ScaledLocal(new Vector3(Scale, Scale, Scale));

			AddCollision(currentPos + GlobalPosition, direction);

			multiMesh.SetInstanceTransform(i, xform);
		}
	}
}
