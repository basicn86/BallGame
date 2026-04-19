using Godot;
using System;

public partial class MainMenu : VBoxContainer
{
	[Export]
	PackedScene tutorialScene;
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
