using Godot;
using System;

[Tool]
public partial class FanBlades : MeshInstance3D
{
	[Export]
	float speed = 1.0f;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RotateY(speed * (float)delta);
	}
}
