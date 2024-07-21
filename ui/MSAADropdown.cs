using Godot;
using System;

namespace UIControls
{
	public partial class MSAADropdown : OptionButton
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Select((int)GetViewport().Msaa3D);
		}

		private void _on_item_selected(long index)
		{
			if (index < (long)Viewport.Msaa.Disabled) return;
			if (index > (long)Viewport.Msaa.Max) return;

			GetViewport().Msaa3D = (Viewport.Msaa)index;
		}
	}
}
