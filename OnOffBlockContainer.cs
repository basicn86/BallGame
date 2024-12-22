using Godot;
using System;

public partial class OnOffBlockContainer : Node3D
{
	[Export]
	public Node3D OnBlock;
	[Export]
	public Node3D OffBlock;

	public override void _Ready()
	{
		if (OnOffBlockSensor.IsOn)
		{
			OnBlock.Visible = true;
			OffBlock.Visible = false;
		}
		else
		{
			OnBlock.Visible = false;
			OffBlock.Visible = true;
		}
	}

	
	public void OnOffToggle(bool isOn)
	{
		if (isOn)
		{
			OnBlock.Visible = true;
			OffBlock.Visible = false;
		}
		else
		{
			OnBlock.Visible = false;
			OffBlock.Visible = true;
		}
	}
}
