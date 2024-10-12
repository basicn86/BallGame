using Godot;
using System;

namespace BallUI
{
	public partial class FPSLimiter : LineEdit
	{
		[Export]
		private int minValue = 30;
		[Export]
		private int maxValue = 999;
		[Export]
		private int defaultValue = 60;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Text = defaultValue.ToString();
			Engine.MaxFps = defaultValue;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public void _on_text_submitted(string text)
		{
			SetFPS(text);
		}

		public void _on_focus_exited()
		{
			SetFPS(Text);
		}

		public void SetFPS(string fpsString)
		{
			bool success = Int32.TryParse(fpsString, out int fps);
			if (success)
			{
				if (fps < minValue) fps = minValue;
				if (fps > maxValue) fps = maxValue;
				Engine.MaxFps = fps;
				Clear();
				InsertTextAtCaret(fps.ToString());
			}
			else
			{
				Engine.MaxFps = defaultValue;
				Clear();
				InsertTextAtCaret(defaultValue.ToString());
			}
		}
	}
}
