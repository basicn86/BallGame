using Godot;
using System;

public partial class Coin : Node3D
{
	[Export]
	public float RotationSpeed = 1f;

	[Export]
	private MeshInstance3D mesh;

	public override void _Ready()
	{
		if(mesh == null) QueueFree();
	}

	
	public override void _Process(double delta)
	{
		//we only need to rotate the mesh, it makes no sense to rotate the entire node including the collision shape
		mesh.RotateY(RotationSpeed * (float)delta);
	}

	private void body_entered(Node3D node)
	{
		GD.Print("Coin body entered");
		//TODO: actually implement this. We are queueing free for debugging purposes
		if (node is Player) QueueFree();
	}
}
