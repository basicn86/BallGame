using Godot;
using System;

namespace UIControls
{
	public partial class UIScaleSlider : HSlider
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Value = GetWindow().ContentScaleFactor;
		}

		private void _on_drag_ended(bool value_changed)
		{
			if(!value_changed) return;
			GD.Print(Value);
			GetWindow().ContentScaleFactor = (float)Value;
		}
	}
}
