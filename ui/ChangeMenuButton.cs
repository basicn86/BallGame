using Godot;
using System;

namespace UIControls
{
	public partial class ChangeMenuButton : Button
	{
		[Export]
		Control ControlToShow;
		[Export]
		Control ControlToHide;

		private void _on_pressed()
		{
			ControlToShow.Visible = true;
			ControlToHide.Visible = false;
		}
	}
}
