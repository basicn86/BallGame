using Godot;
using System;

public partial class CptDetonatorRocket : RigidBody3D
{
	[Export]
	public float RocketSpeed = 20f;

	Player player;
	private Vector3 travelDirection = Vector3.Forward;
	public Vector3 TravelDirection
	{
		get => travelDirection;
		set => travelDirection = value.Normalized();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = Player.Instance;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if(GetContactCount() > 0) QueueFree();
		if (LinearVelocity.IsZeroApprox()) return;
		LookAt(GlobalPosition + LinearVelocity.Normalized(), Vector3.Up);
	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		if (player == null) return;
		Vector3 directionToPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
		
		if (travelDirection.Dot(-directionToPlayer) < 0.97f && travelDirection.Dot(-directionToPlayer) > 0.0f)
		{
			Vector3 rotationAxis = directionToPlayer.Cross(travelDirection.Normalized()).Normalized();

			travelDirection = travelDirection.Rotated(rotationAxis, 0.03f).Normalized();
		}

		LinearVelocity = -travelDirection * RocketSpeed;
	}

	public void _on_timer_timeout()
	{
		QueueFree();
	}
}
