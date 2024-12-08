using BallUI;
using Godot;
using System;

public partial class Hint : Node3D
{
	string HintText = "Tutorial: blah blah blah";

	private void body_entered(Node3D node)
	{
		HintPanel.Instance.HintEntered(HintText);
	}

	private void body_exited(Node3D node)
	{
		HintPanel.Instance.HintExited();
	}
}
