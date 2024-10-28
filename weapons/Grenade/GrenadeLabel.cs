using Godot;
using System;

public partial class GrenadeLabel : Label3D
{
	[Export]
	private MeshInstance3D mainModel;

	private Vector3 offset = Vector3.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		offset = Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = mainModel.GlobalPosition + offset;
	}
}
