using Godot;
using System;

public partial class Crosshair : CenterContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		QueueRedraw();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Draw()
	{
		DrawCircle(new Vector2(Size.X / 2f, Size.Y / 2f), 6.0f, new Color(0f, 0.0f, 0f));
		DrawCircle(new Vector2(Size.X / 2f, Size.Y / 2f), 4.0f, new Color(0.0f, 1.0f, 0.0f));
	}
}
