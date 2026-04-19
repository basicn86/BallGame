using Godot;
using System;

public partial class CptDetonatorRocketParticlesModelInterpolator : GpuParticles3D
{
	[Export]
	public Node3D target;

	Vector3 lastPosition = Vector3.Zero;
	Vector3 currentPosition = Vector3.Zero;

	private bool firstFrame = true;

	public override void _Ready()
	{
		lastPosition = target.GlobalPosition;
		currentPosition = target.GlobalPosition;
		TopLevel = true;
	}
	public override void _Process(double delta)
	{
		if(firstFrame)
		{
			lastPosition = target.GlobalPosition;
			currentPosition = target.GlobalPosition;
			GlobalPosition = target.GlobalPosition;
			return;
		}
		GlobalPosition = lastPosition.Lerp(currentPosition, (float)Engine.GetPhysicsInterpolationFraction());
	}
	public override void _PhysicsProcess(double delta)
	{
		lastPosition = currentPosition;
		currentPosition = target.GlobalPosition;
	}
}
