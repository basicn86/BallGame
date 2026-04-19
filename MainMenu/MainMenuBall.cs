using Godot;
using System;

public partial class MainMenuBall : RigidBody3D
{
	[Export]
	MeshInstance3D mesh;

	private float speed = 5.0f;

	public Vector3 TargetPosition;

	public override void _Ready()
	{
		speed = (float)GD.RandRange(3.0f, 7.0f);
		
		StandardMaterial3D mat = new StandardMaterial3D();

		mat.AlbedoColor = GetRandomColor();

		mesh.SetSurfaceOverrideMaterial(0, mat);

		LinearVelocity = new Vector3(GD.RandRange(-5, 5), 0f, GD.RandRange(-5, 5));
	}

	private Color GetRandomColor()
	{
		Color col = new Color(
			(float)GD.RandRange(0.0, 1.0),
			(float)GD.RandRange(0.0, 1.0),
			(float)GD.RandRange(0.0, 1.0),
			1.0f
		);

		//Darken one channel to ensure the color isn't too bright
		uint i = GD.Randi() % 3;
		if (i == 0)
			col.R *= 0.5f;
		else if (i == 1)
			col.G *= 0.5f;
		else
			col.B *= 0.5f;

		return col;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction = (TargetPosition - GlobalPosition);
		direction.Y = 0; // Keep movement in the horizontal plane
		direction = direction.Normalized();

		Vector3 horizontalVelocity = LinearVelocity;
		horizontalVelocity.Y = 0;

		//Multiplies the final force by a drag factor.
		float dragMultiplier = 1.0f;

		if(horizontalVelocity.Length() > speed)
		{
			float dot = horizontalVelocity.Dot(direction);
			if(dot > 0.0)
			{
				dragMultiplier = 1.0f - dot;
			}
		}

		ApplyCentralForce(direction * speed * dragMultiplier);
	}

	public void _on_timer_timeout()
	{
		mesh.SetSurfaceOverrideMaterial(0, null);
		QueueFree();
	}
}
