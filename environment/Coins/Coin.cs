using Godot;
using System;

public partial class Coin : Node3D
{
	[Export]
	public float RotationSpeed = 1f;

	[Export]
	private MeshInstance3D mesh;

	private Player? player = null;

	[Export]
	private Area3D DetectionArea;

	public override void _Ready()
	{
		if (mesh == null) QueueFree();
		mesh.RotateY(GD.Randf());

		if(GD.Randf() > 0.5) RotationSpeed *= -1.0f;
	}

	
	public override void _Process(double delta)
	{
		//we only need to rotate the mesh, it makes no sense to rotate the entire node including the collision shape
		mesh.RotateY(RotationSpeed * (float)delta);

		if (player == null) return;
		GlobalTranslate((player.GlobalPosition - GlobalPosition).Normalized() * 9f * (float)delta);
		if (GlobalPosition.DistanceTo(player.GlobalPosition) < 0.25f)
		{
			CoinCounter.Instance.AddCoins(1);
			QueueFree();
		}
	}

	private void body_entered(Node3D node)
	{
		if (node is not Player) return;
		player = node as Player;
		DetectionArea.QueueFree();
	}
}
