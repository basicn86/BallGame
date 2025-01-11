using Godot;
using System;
using System.Diagnostics;

//todo: possibly move this to BallGame.DemoLevel
namespace DemoLevel;
public partial class DemoLevelExplosion : Area3D
{
	[Export]
	public float explosionForce = 10.0f;

	[Export]
	public GpuParticles3D[] particles;

	[Export]
	public MeshInstance3D burningGround;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debugf1"))
		{
			Godot.Collections.Array<Node3D> bodies = GetOverlappingBodies();
			foreach (Node3D body in bodies)
			{
				if (body is RigidBody3D)
				{
					RigidBody3D rigidBody = body as RigidBody3D;
					Vector3 vector = (rigidBody.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
					rigidBody.ApplyImpulse(vector * explosionForce);
				}
			}

			foreach (GpuParticles3D particle in particles)
			{
				particle.Emitting = true;
			}
		}
	}
}
