using Godot;
using System;

public partial class GrenadeThrower : Node3D
{

	[Export]
	Vector3 Offset;
	[Export]
	Vector3 Direction;
	[Export]
	PackedScene grenadeScene;

	[Export]
	public float throwForce = 10f;
	[Export]
	public float throwHeight = 2.5f;

	private Grenade grenade;
	private bool isHeld = false;

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInstanceValid(grenade)) isHeld = false;

		if (isHeld)
		{
			// Update the position of the grenade to be at the player's hand
			grenade.GlobalPosition = GlobalPosition + (Offset * Basis.Inverse());
			//The grenade still accelerates while held, so we need to stop it to prevent it from flying into the ground
			grenade.LinearVelocity = Vector3.Zero;
		}
	}

	public void UpdatePosition(Vector3 position, Basis basis)
	{
		GlobalPosition = position + Offset * basis.Inverse();
		Rotation = basis.GetEuler();
	}

	public void Fire()
	{
		grenade = (Grenade)grenadeScene.Instantiate();

		GetParent().AddChild(grenade);

		grenade.GlobalPosition = GlobalPosition + (Offset * Basis.Inverse());

		//Dumb hack: We need to reset the model interpolator so it doesn't spawn at the world origin
		grenade.ResetInterpolator();

		isHeld = true;
	}

	public void Release(Vector3 targetPos)
	{
		if (!isHeld) return;
		grenade.ApplyImpulse(throwForce * (targetPos - GlobalPosition).Normalized() + new Vector3(0f, throwHeight, 0f));
		isHeld = false;
	}
}
