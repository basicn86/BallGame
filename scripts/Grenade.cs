using Godot;
using System;
using System.Diagnostics;

public partial class Grenade : RigidBody3D
{
	[ExportGroup("References")]
	[Export]
	Area3D explosionArea;
	[Export]
	PackedScene explosionScene;
	[Export]
	ModelInterpolator interpolator;

	[ExportGroup("Forces")]
	[Export]
	public CurveTexture forceCurve;
	[Export]
	public float explosionForce = 1f;
	[Export]
	public float explosionRadius = 3f;

	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void ResetInterpolator()
	{
		interpolator.GlobalPosition = GlobalPosition;
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
				float finalForce = forceCurve.Curve.Sample(forceDirection.Length() / explosionRadius);

				i.ApplyCentralImpulse(forceDirection.Normalized() * finalForce * explosionForce);

				//for debug output, print the name of the object that was hit
				GD.Print(i.Name);
			}
		}

		Node3D _explosionScene = (Node3D)explosionScene.Instantiate();
		GetParent().AddChild(_explosionScene);
		_explosionScene.GlobalPosition = GlobalPosition;
		QueueFree();
		
		//stopwatch stop
		sw.Stop();
		GD.Print("Time elapsed to explode: " + sw.ElapsedMilliseconds + "ms");
	}
}
