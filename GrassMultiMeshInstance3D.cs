using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class GrassMultiMeshInstance3D : MultiMeshInstance3D
{
	[Export]
	public Material Material { get; set; }

	//Caching the transforms increases performance by 2x, even if it adds more operations, since we do not need to wait for the rendering server.
	private Transform3D[] transforms;

	public override void _Ready()
	{
		Multimesh.Mesh.SurfaceSetMaterial(0, Material);
		Multimesh.VisibleInstanceCount = Multimesh.InstanceCount;

		transforms = new Transform3D[Multimesh.InstanceCount];
		for (int i = 0; i < Multimesh.InstanceCount; i++)
		{
			transforms[i] = Multimesh.GetInstanceTransform(i);
		}
	}

	public void RemoveWithinDistance(Vector3 position, float distance)
	{
		List<int> instanceIndexes = GetInstancesToRemove(position, distance, 0, Multimesh.VisibleInstanceCount);

		for (int i = 0, last = Multimesh.VisibleInstanceCount - 1; i < instanceIndexes.Count; i++)
		{
			while (instanceIndexes.Contains(last) && last > 0)
			{
				last--;
			}
			if (last > 0)
			{
				Multimesh.SetInstanceTransform(instanceIndexes[i], transforms[last]);
				transforms[instanceIndexes[i]] = transforms[last];
				last--;
			}
		}
		Multimesh.VisibleInstanceCount = Multimesh.VisibleInstanceCount - instanceIndexes.Count;
	}

	private List<int> GetInstancesToRemove(Vector3 position, float distance, int start, int end)
	{
		List<int> instancesToRemove = new List<int>(Multimesh.VisibleInstanceCount);
		float distanceSquared = distance * distance;
		for (int i = start; i < end; i++)
		{
			if (transforms[i].Origin.DistanceSquaredTo(position) < distanceSquared)
			{
				instancesToRemove.Add(i);
			}
		}
		return instancesToRemove;
	}
}
