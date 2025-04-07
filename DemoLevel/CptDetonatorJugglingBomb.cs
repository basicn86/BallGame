using Godot;
using System;

public partial class CptDetonatorJugglingBomb : RigidBody3D
{
	//used to tell where the bomb is in the air so the player doesnt have to look up
	[Export]
	private RayCast3D groundRay;
	[Export]
	private MeshInstance3D groundIndicatorMesh;
	private bool finished = false;

	public Node3D CptDet;

	private int hits = 0;
	public int maxHits = 1;

	private bool firstFrame = true;

	public override void _Ready()
	{
		groundIndicatorMesh.GlobalPosition = GlobalPosition;
		ResetPhysicsInterpolation();
	}

	public override void _Process(double delta)
	{
		if (finished) return;

		groundRay.ForceRaycastUpdate();
		groundIndicatorMesh.GlobalPosition = groundRay.GetCollisionPoint();

		if (firstFrame)
		{
			ResetPhysicsInterpolation();
			firstFrame = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GetContactCount() > 0)
		{
			QueueFree();
		}
	}

	public void _player_entered(Node3D body)
	{
		if (finished) return;
		if (body is not Player) return;
		Player player = body as Player;
		Vector3 direction = Vector3.Zero;

		if (hits >= maxHits)
		{
			direction = CptDet.GlobalPosition - GlobalPosition;
			float height = direction.Y;
			direction.Y = 0f;
			float dist = direction.Length();
			direction = direction.Normalized();
			direction *= 20f;
			direction.Y = (dist / 8f) - MathF.Abs(height);
			LinearVelocity = direction;

			finished = true;
			groundRay.QueueFree();
			groundIndicatorMesh.QueueFree();
			return;
		}

		direction = new Vector3(Random.Shared.NextSingle(), 5f, Random.Shared.NextSingle());
		direction.X -= 0.5f;
		direction.Z -= 0.5f;
		direction.X *= 8f;
		direction.Z *= 8f;

		LinearVelocity = direction;

		hits++;
	}
}
