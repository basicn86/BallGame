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

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void UpdatePosition(Vector3 position, Basis basis)
	{
		GlobalPosition = position + Offset * basis.Inverse();
		Rotation = basis.GetEuler();
	}

	public void Fire(Vector3 targetPos)
	{
		Grenade grenade = (Grenade)grenadeScene.Instantiate();

		GetParent().AddChild(grenade);

		grenade.GlobalPosition = GlobalPosition + (Offset * Basis.Inverse());

		//Dumb hack: We need to reset the model interpolator so it doesn't spawn at the world origin
		grenade.ResetInterpolator();

		grenade.ApplyImpulse(throwForce * (targetPos - GlobalPosition).Normalized() + new Vector3(0f,throwHeight,0f));
	}
}
