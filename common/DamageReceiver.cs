using Godot;
using System;
using BallGame.Common;

public partial class DamageReceiver : Area3D
{
    //Used to indicate to external damage senders if this is a player or not
    [Export]
	public bool IsPlayer = false;

	public void ReceiveDamage(int amount, Team team = Team.Neutral, Vector3 KnockbackForce = default) {
		EmitSignal(SignalName.TakeDamage, amount, (int)team, KnockbackForce);
    }

    [Signal]
	public delegate void TakeDamageEventHandler(int amount, Team team, Vector3 KnockbackForce);
}
