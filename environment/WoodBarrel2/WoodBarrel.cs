using BallGame.Common;
using Godot;
using System;

public partial class WoodBarrel : RigidBody3D
{
	[Export]
	public PackedScene ExplosionScene;

	public void _take_damage(int amount, Team team, Vector3 KnockbackForce)
	{
		if(team != Team.Neutral) return;

		Node3D Explosion = ExplosionScene.Instantiate<Node3D>();
		GetParent().AddChild(Explosion);

		Explosion.GlobalPosition = GlobalPosition;

		QueueFree();
	}
}
