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

			//This is to prevent the rotation from going over 180 or -180
			//if the float isn't clamped to this range, it can cause precision errors
			if (_currentX > 180f) _currentX -= 360f;
			if(_currentX < -180f) _currentX += 360f;

			Rotation = new Vector3(_currentX, _currentY, _currentZ);
		}
	}
}
