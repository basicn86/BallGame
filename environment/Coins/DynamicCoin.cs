using Godot;
using System;

public partial class DynamicCoin : RigidBody3D
{
	[Export]
	public float RotationSpeed = 1f;

	[Export]
	private MeshInstance3D mesh;

	[Export]
	private Area3D DetectionArea;

	private float activationTimer = 1.0f;
	private float despawnTimer = 60f;
	private Player? player = null;

	private Vector3 currentMeshPos = Vector3.Zero;
	private Vector3 prevMeshPos = Vector3.Zero;
	private bool firstFrame = true;

	public override void _Ready()
	{
		if(mesh == null) QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		prevMeshPos = currentMeshPos;
		currentMeshPos = GlobalPosition;

		despawnTimer -= (float)delta;
		if (despawnTimer < 0f)
		{
			QueueFree();
			return;
		}

		if (player == null) return;
		GlobalTranslate((player.GlobalPosition - GlobalPosition).Normalized() * 6f * (float)delta);
		if (GlobalPosition.DistanceTo(player.GlobalPosition) < 0.25f)
		{
			QueueFree();
		}
	}

	public override void _Process(double delta)
	{
		if (firstFrame)
		{
			currentMeshPos = GlobalPosition;
			prevMeshPos = GlobalPosition;
			firstFrame = false;
		}

		if(activationTimer > 0f) activationTimer -= (float)delta;

		mesh.RotateY(RotationSpeed * (float)delta);
		mesh.GlobalPosition = prevMeshPos.Lerp(currentMeshPos, (float)Engine.GetPhysicsInterpolationFraction());
	}

	private void body_entered(Node3D node)
	{
		if (activationTimer > 0f) return;
		if (node is not Player) return;
		player = node as Player;
		DetectionArea.QueueFree();
		CustomIntegrator = true;
	}
}
