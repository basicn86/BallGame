using Godot;
using System;

public partial class VSyncToggle : CheckBox
{
	public override void _Ready()
	{
		ButtonPressed = true;
	}

	public void _on_toggled(bool toggled)
	{
		if (toggled)
		{
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
			GD.Print("VSync enabled");
		}
		else
		{
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
			GD.Print("VSync disabled");
		}
	}
}
