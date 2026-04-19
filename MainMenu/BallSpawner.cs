using Godot;
using System;

public partial class BallSpawner : Node3D
{
	[Export]
	PackedScene ballScene;

	[Export]
	Node3D TargetNode;

	[Export]
	float SpawnInterval = 1.0f;
	float spawnTimer = 0.0f;

	public override void _PhysicsProcess(double delta)
	{
		spawnTimer += (float)delta;

		if (spawnTimer >= SpawnInterval)
		{
			spawnTimer = 0.0f;
			SpawnBall();
		}
	}

	public void SpawnBall()
	{
		MainMenuBall ball = (MainMenuBall)ballScene.Instantiate();
		GetParent().AddChild(ball);
		ball.GlobalPosition = GlobalPosition;
		ball.ResetPhysicsInterpolation();

		ball.TargetPosition = TargetNode.GlobalPosition;
	}

	public override void _ExitTree()
	{
		GC.Collect();
	}
}
