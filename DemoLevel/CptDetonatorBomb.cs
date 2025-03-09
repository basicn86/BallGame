using Godot;
using System;

public partial class CptDetonatorBomb : RigidBody3D
{
	[Export]
	PackedScene explosionScene;

	[Export]
	Area3D explosionArea;

	public GrassMultiMeshInstance3D grassMultiMeshInstance3D;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void DealDamage()
	{
		Godot.Collections.Array<Area3D> bodies = explosionArea.GetOverlappingAreas();
		foreach (Area3D body in bodies)
		{
			if (body is HitBoxComponent)
			{
				HitBoxComponent hitBox = body as HitBoxComponent;
				hitBox.EmitSignal("TakeDamage", 10, (int)BallGame.Common.Team.Enemy);
			}
		}
	}

	public void _on_explosion_timer_timeout()
	{
		Node3D explosion = explosionScene.Instantiate() as Node3D;
		GetParent().AddChild(explosion);
		explosion.GlobalPosition = GlobalPosition;

		grassMultiMeshInstance3D?.RemoveWithinDistance(GlobalPosition, 3.0f);

		CptDetonatorGround.Instance?.AddBurnPoint(GlobalPosition);

		DealDamage();

		QueueFree();
	}
}
