using Godot;
using System;

namespace BallUI;


/// <summary>
/// A panel that displays a hint when the mouse enters a node.
/// </summary>
/// <remarks>
/// Singleton Class: Only one instance is allowed.
/// </remarks>
public partial class HintPanel : PanelContainer
{

	[Export]
	RichTextLabel hintLabel;

	public static HintPanel Instance { get; private set; }

	public override void _Ready()
	{
		if(Instance != null) QueueFree(); //guarantee only one instance
		Instance = this;
		Visible = false;
	}

	
	public override void _Process(double delta)
	{
	}

	public void HintEntered(string text)
	{
		hintLabel.Text = text;
		Visible = true;
	}

	public void HintExited()
	{
		Visible = false;
	}
}
