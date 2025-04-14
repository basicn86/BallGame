using Godot;
using System;
using BallGame.Common;

public partial class SpikeArea : Area3D
{
	[Export]
	public int Damage = 1000;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_area_entered(Area3D area)
	{
		if (area is DamageReceiver damageReceiver)
		{
			damageReceiver.ReceiveDamage(Damage, Team.Neutral);
		}
	}
}
