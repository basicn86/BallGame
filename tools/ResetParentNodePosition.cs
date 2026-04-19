using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BallGame.Tools;

[Tool]
public partial class ResetParentNodePosition : Node3D
{
	[Export] public bool ResetPosition = false;

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint() && ResetPosition)
		{
			Godot.Collections.Array<Node> children = GetChildren();
			List<Node3D> children3D = (from c in children where c is Node3D select c as Node3D).ToList();
			List<Transform3D> transforms = (from c in children3D select c.GlobalTransform).ToList();

			GlobalTransform = Transform3D.Identity;

			for(int i = 0; i < children3D.Count; i++)
			{
				children3D[i].GlobalTransform = transforms[i];
			}

			ResetPosition = false;
		}
	}
}
