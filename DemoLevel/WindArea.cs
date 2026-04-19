using Godot;
using System;
using System.Collections.Generic;

namespace DemoLevel;
public partial class WindArea : Area3D
{
	private List<RigidBody3D> rigidBodies = new List<RigidBody3D>();

	[Export]
	public Vector3 windForce = Vector3.Zero;

	public override void _Ready()
	{
		BodyEntered += _on_body_entered;
		BodyExited += _on_body_exited;
	}

	public override void _PhysicsProcess(double delta)
	{
		foreach (var body in rigidBodies)
		{
			body.ApplyCentralForce(windForce);
		}
	}

	private void _on_body_entered(Node3D body)
	{
		GD.Print("Body entered");
		if (body is RigidBody3D)
		{
			rigidBodies.Add((RigidBody3D)body);
		}
	}

	private void _on_body_exited(Node3D body)
	{
		if (body is RigidBody3D)
		{
			rigidBodies.Remove((RigidBody3D)body);
		}
	}
}
