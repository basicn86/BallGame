using Godot;
using System;

namespace DemoLevel
{
	public partial class RotatingBar : AnimatableBody3D
	{
		[Export]
		public float RotationSpeed = 1f;
		private float _currentX = 0f;
		private float _currentY = 0f;
		private float _currentZ = 0f;

		public override void _Ready()
		{
			_currentX = Rotation.X;
			_currentY = Rotation.Y;
			_currentZ = Rotation.Z;
		}

		public override void _PhysicsProcess(double delta)
		{
			_currentX += RotationSpeed;

			Rotation = new Vector3(_currentX, _currentY, _currentZ);
		}
	}
}
