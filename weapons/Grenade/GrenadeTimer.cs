using Godot;
using System;

public partial class GrenadeTimer : Timer
{
	[Export]
	private Label3D timerLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		timerLabel.Text = TimeLeft.ToString("0.0");
	}
}
