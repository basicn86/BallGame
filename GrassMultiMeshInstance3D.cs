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
	private List<Transform3D> transforms;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < Multimesh.InstanceCount; i++)
		{
			Multimesh.Mesh.SurfaceSetMaterial(0, Material);
		}
		Multimesh.VisibleInstanceCount = Multimesh.InstanceCount;

		transforms = new List<Transform3D>(Multimesh.InstanceCount);
		for (int i = 0; i < Multimesh.InstanceCount; i++)
		{
			transforms.Add(Multimesh.GetInstanceTransform(i));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//TODO: possibly move this to its own class, may cause clutter.
	//TODO 2: This function takes 10 ms on 30k instances. Multithreading may be needed.
	//NOTE: This does scale with the number of instances available, as in it operates in O(n) time, where n is the number of instances.
	public void RemoveWithinDistance(Vector3 position, float distance)
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		int[] instanceIndexes = new int[Multimesh.VisibleInstanceCount];
		float distanceSquared = distance * distance;
		int instanceCount = 0;

		for (int i = 0; i < Multimesh.VisibleInstanceCount; i++)
		{
			if (transforms[i].Origin.DistanceSquaredTo(position) < distanceSquared)
			{
				instanceIndexes[instanceCount] = i;
				instanceCount++;
			}
		}

		for (int i = 0, last = Multimesh.VisibleInstanceCount - 1; i < instanceCount; i++)
		{
			while (instanceIndexes.Contains(last) && last > 0)
			{
				last--;
			}
			if (last > 0) // Ensure last is within valid range
			{
				Multimesh.SetInstanceTransform(instanceIndexes[i], Multimesh.GetInstanceTransform(last));
				transforms[instanceIndexes[i]] = transforms[last];
				last--;
			}
		}
		Multimesh.VisibleInstanceCount = Multimesh.VisibleInstanceCount - instanceCount;
		stopwatch.Stop();
		GD.Print("RemoveWithinDistance took " + stopwatch.ElapsedMilliseconds + " ms");
	}
}
