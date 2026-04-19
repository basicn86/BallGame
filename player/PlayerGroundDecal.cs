using Godot;
using System;

public partial class PlayerGroundDecal : Decal
{
	[Export]
	private Node3D player;
	[Export]
	private RayCast3D groundCast;
	[Export]
	private float offset;

	//These are the player's positions, for interpolating movement
	private Vector3 currentPos;
	private Vector3 prevPos;

	public override void _Ready()
	{
		if(player == null || groundCast == null)
		{
			GD.PrintErr("PlayerGroundDecal: Player or GroundCast not assigned!");
			QueueFree();
			return;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		prevPos = currentPos;
		currentPos = player.GlobalPosition;
	}

	public override void _Process(double delta)
	{
		if (!groundCast.IsColliding())
		{
			Vector3 interpolatedPos = prevPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
			GlobalPosition = interpolatedPos + new Vector3(0f, offset, 0f);
			Visible = true;
		} else
		{
			Visible = false;
		}
	}
}
