using Godot;
using System;
using System.Diagnostics;

public partial class Grenade : RigidBody3D
{
	[Export]
	Area3D explosionArea;
	[Export]
	PackedScene explosionScene;
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _on_timer_timeout()
	{
		//stopwatch start
		Stopwatch sw = new Stopwatch();
		sw.Start();

		var bodies = explosionArea.GetOverlappingBodies();

		foreach (var body in bodies)
		{
			if (body.GetInstanceId() == GetInstanceId()) continue;

			if (body is RigidBody3D)
			{
				RigidBody3D i = (RigidBody3D)body;

				Vector3 forceDirection = i.GlobalTransform.Origin - GlobalTransform.Origin;
				float distance = forceDirection.Length();

				forceDirection = forceDirection.Normalized();

				i.ApplyCentralForce(forceDirection * 100f / distance);

				//for debug output, print the name of the object that was hit
				GD.Print(i.Name);
			}
		}

		Node3D _explosionScene = (Node3D)explosionScene.Instantiate();
		_explosionScene.Position = GlobalTransform.Origin;
		GetParent().AddChild(_explosionScene);
		QueueFree();
		
		//stopwatch stop
		sw.Stop();
		GD.Print("Time elapsed to explode: " + sw.ElapsedMilliseconds + "ms");
	}
}
