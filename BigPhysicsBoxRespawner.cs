using Godot;
using System;

namespace DemoLevel {
	public partial class BigPhysicsBoxRespawner : Node3D
	{
		[Export]
		public PackedScene physicsBoxScene;
		[Export]
		public Timer respawnTimer;

		private Vector3 initialSpawn = Vector3.Zero;
		public override void _Ready()
		{
			initialSpawn = GlobalPosition;
		}

		private void BoxDied()
		{
			respawnTimer.Start();
		}

		private void SpawnBox()
		{
			Node3D freshBox = (Node3D)physicsBoxScene.Instantiate();
			AddChild(freshBox);
			freshBox.GlobalPosition = initialSpawn;
			freshBox.TreeExited += BoxDied;
		}
	}
}
