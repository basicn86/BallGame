using Godot;
using System;
using System.Diagnostics;

public partial class OnOffBlockSensor : Area3D
{
	public static bool IsOn = false;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	private void _on_body_entered(Node body)
	{
		if (body is not Player) return;
		Player player = (Player)body;

		IsOn = !IsOn;

		GD.Print("OnOffBlockSensor: " + IsOn);

		GetTree().CallGroup("OnOffBlock", "OnOffToggle", IsOn);
	}
}
