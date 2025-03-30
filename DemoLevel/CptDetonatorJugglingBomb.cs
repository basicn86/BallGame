using Godot;
using System;

public partial class CptDetonatorJugglingBomb : RigidBody3D
{
	//used to tell where the bomb is in the air so the player doesnt have to look up
	[Export]
	private RayCast3D groundRay;
	[Export]
	private MeshInstance3D groundIndicatorMesh;

	public override void _Ready()
	{
		groundIndicatorMesh.TopLevel = true;
	}

	public override void _Process(double delta)
	{
		groundRay.ForceRaycastUpdate();
		groundIndicatorMesh.GlobalPosition = groundRay.GetCollisionPoint();
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
		if (body is not Player) return;
		Player player = body as Player;

		Vector3 direction = new Vector3(Random.Shared.NextSingle(), 5f, Random.Shared.NextSingle());

		LinearVelocity = direction;
	}
}
