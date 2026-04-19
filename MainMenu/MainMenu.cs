using Godot;
using System;

public partial class MainMenu : VBoxContainer
{
	[Export]
	PackedScene tutorialScene;

	[Export]
	Button tutorialButton;

	public override void _Ready()
	{
		tutorialButton.GrabFocus();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("fullscreen"))
		{
			if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
			else
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
		}
	}

	public void _tutorial_pressed()
	{
		//change scene to the tutorial scene
		GetTree().ChangeSceneToPacked(tutorialScene);
	}

	public void _quit_pressed()
	{
		GetTree().Quit();
	}
}
