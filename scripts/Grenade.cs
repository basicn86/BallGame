using Godot;
using System;

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
			}
		}

		Node3D _explosionScene = (Node3D)explosionScene.Instantiate();
		GetParent().AddChild(_explosionScene);
		_explosionScene.GlobalPosition = GlobalPosition;
		QueueFree();
	}
}
