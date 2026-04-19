using Godot;
using System;

public partial class OnOffPlatform : Node3D
{
	[Export]
	Node3D OnNode;

	[Export]
	Node3D OffNode;

	public override void _Ready()
	{
		if(OnNode == null || OffNode == null)
		{
			GD.Print("OnOffPlatform: OnNode or OffNode is null");
			QueueFree();
			return;
		}

		if(OnOffBlockSensor.IsOn)
		{
			OnNode.ProcessMode = ProcessModeEnum.Inherit;
			OnNode.Visible = true;
			OffNode.ProcessMode = ProcessModeEnum.Disabled;
			OffNode.Visible = false;
		}
		else
		{
			OnNode.ProcessMode = ProcessModeEnum.Disabled;
			OnNode.Visible = false;
			OffNode.ProcessMode = ProcessModeEnum.Inherit;
			OffNode.Visible = true;
		}
	}

	public void OnOffToggle(bool isOn)
	{
		if (isOn)
		{
			OnNode.ProcessMode = ProcessModeEnum.Inherit;
			OnNode.Visible = true;
			OffNode.ProcessMode = ProcessModeEnum.Disabled;
			OffNode.Visible = false;
		}
		else
		{
			OnNode.ProcessMode = ProcessModeEnum.Disabled;
			OnNode.Visible = false;
			OffNode.ProcessMode = ProcessModeEnum.Inherit;
			OffNode.Visible = true;
		}
	}
}
