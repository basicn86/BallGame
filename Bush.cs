using Godot;
using System;

public partial class Bush : Node3D
{
	[Export]
	GpuParticles3D particles;

	[Export]
	AudioStreamPlayer3D AudioPlayer;

	public void _on_player_detection_area_body_entered(Node3D body)
	{
		particles.Restart();
		particles.Emitting = true;
		AudioPlayer.Play(); //rustle
	}
}
