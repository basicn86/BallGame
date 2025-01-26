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

	[Export]
	public AudioStreamPlayer3D explosionSound;

	[Export]
	public ShaderMaterial grassMaterial;

	[Export]
	public CurveTexture windCurve;
	[Export]
	public float windLifeTime = 1.0f;
	[Export]
	public float windStrength = 1.0f;
	private bool animateWind = false;
	private float windTime = 0.0f;

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
					rigidBody.ApplyImpulse(vector * explosionForce * rigidBody.Mass);
				}
			}

			foreach (GpuParticles3D particle in particles)
			{
				particle.Emitting = true;
			}

			explosionSound.Play();

			animateWind = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!animateWind) return;

		float netWindStrength = windCurve.Curve.Sample(windTime) * windStrength;
		windTime += (float)delta / windLifeTime;

		grassMaterial.SetShaderParameter("windStrength", netWindStrength);

		if (windTime > 1.0f)
		{
			animateWind = false;
			windTime = 0.0f;
		}
	}
}
