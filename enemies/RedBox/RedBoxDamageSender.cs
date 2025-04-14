using Godot;
using System;

using BallGame.Common;

public partial class RedBoxDamageSender : Area3D
{
	[Export]
	public int Damage = 1000;

	private void _on_area_entered(Area3D area)
	{
		if (area is DamageReceiver damageReceiver)
		{
			if (damageReceiver.IsPlayer) {
				Vector3 directionToPlayer = (damageReceiver.GlobalPosition - GlobalPosition).Normalized();
				if (Vector3.Up.Dot(directionToPlayer) > 0.707f)
				{
					EmitSignal(SignalName.JumpedByPlayer);
					return;
				}
			}
			damageReceiver.ReceiveDamage(Damage, Team.Enemy);
		}
	}

	[Signal]
	public delegate void JumpedByPlayerEventHandler();
}
