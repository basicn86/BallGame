using Godot;
using System;

public partial class CptDetonatorCutscene : AnimationPlayer
{
	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debugf1"))
		{
			Play("CptDetonatorIntro");
		}
	}
}
