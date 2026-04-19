using Godot;
using System;

public partial class PauseMenu : MarginContainer
{
	public override void _Ready()
	{
		Visible = false;
		//capture the mouse
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ToggleFullscreen();

		if (Input.IsActionJustPressed("ui_cancel")) Input.MouseMode = Input.MouseModeEnum.Visible;

		if (Input.IsActionJustPressed("pause"))
		{
			if (GetTree().Paused)
				ResumeGame();
			else
				PauseGame();
		}
	}
	void PauseGame()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetTree().Paused = true;
		Visible = true;
	}

	void ResumeGame()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		GetTree().Paused = false;
		Visible = false;
	}

	private void ToggleFullscreen()
	{
		//TODO: move to UI code, doesnt make sense to have it here
		if (Input.IsActionJustPressed("fullscreen"))
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;

			//Exclusive fullscreen is needed for FreeSync/G-Sync to work
			if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
			else
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
		}
	}

	private void _on_resume_button_pressed()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		GetTree().Paused = false;
		Visible = false;
	}

	private void _on_quit_button_pressed()
	{
		GetTree().Quit();
	}
}
