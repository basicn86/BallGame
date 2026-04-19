using BallUI;
using Godot;
using System;

public partial class Hint : Node3D
{
	[Export]
	string HintText = "Tutorial: blah blah blah";

	[Export]
	MeshInstance3D Mesh;

	private void body_entered(Node3D node)
	{
		HintPanel.Instance.HintEntered(HintText);
	}

	private void body_exited(Node3D node)
	{
		HintPanel.Instance.HintExited();
	}

	public override void _Process(double delta)
	{
		Mesh.RotateY(1.0f * (float)delta);
	}
}
