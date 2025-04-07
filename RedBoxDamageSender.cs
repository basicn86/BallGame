using Godot;
using System;

using BallGame.Common;

public partial class RedBoxDamageSender : Area3D
{
	[Export]
	public int Damage = 1000;

	private void _on_area_entered(Area3D area)
	{
		if (area is HitBoxComponent hitBoxComponent)
		{
			//we don't want to hit the player if they are above the red box
			Vector3 directionToPlayer = hitBoxComponent.GlobalPosition - GlobalPosition;
			if (directionToPlayer.Dot(Vector3.Up) > 0.75)
			{
				EmitSignal("JumpedByPlayer");
			}
			else
			{
				hitBoxComponent.EmitSignal("TakeDamage", Damage, (int)Team.Enemy);
			}
		}
	}

	[Signal]
	public delegate void JumpedByPlayerEventHandler();
}
