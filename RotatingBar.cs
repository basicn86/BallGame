using Godot;
using System;

namespace DemoLevel
{
	public partial class RotatingBar : AnimatableBody3D
	{
		[Export]
		public float RotationSpeed = 1f;

		public override void _PhysicsProcess(double delta)
		{
			RotateX(RotationSpeed);
		}
	}
}
