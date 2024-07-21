using Godot;
using System;

public partial class PauseMenu : MarginContainer
{
	public override void _Ready()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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

