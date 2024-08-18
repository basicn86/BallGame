using Godot;
using System;

public partial class PlayerRespawner : Node3D
{
	[Export]
	private Timer RespawnTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Disabled;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_player_player_died()
	{
		ProcessMode = ProcessModeEnum.Inherit;
		RespawnTimer.Start();
	}

	private void _on_respawn_delay_timeout()
	{
		EmitSignal("RespawnNow");
	}

	[Signal]
	public delegate void RespawnNowEventHandler();
}
