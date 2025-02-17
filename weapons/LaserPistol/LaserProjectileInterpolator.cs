using Godot;
using System;

public partial class LaserProjectileInterpolator : MeshInstance3D
{
	[Export]
	private RigidBody3D projectile;

    public Vector3 previousPos = Vector3.Zero;
    public Vector3 currentPos = Vector3.Zero;

    private bool firstFrame = true;

    public override void _Ready()
    {
        if (projectile == null) QueueFree();
        TopLevel = true;
    }

    public override void _Process(double delta)
    {
        if (firstFrame)
        {
            previousPos = projectile.GlobalPosition;
            currentPos = projectile.GlobalPosition;
            GlobalPosition = projectile.GlobalPosition;
            GlobalRotation = projectile.GlobalRotation;
            RotateObjectLocal(Vector3.Right, Mathf.Pi / 2);
            firstFrame = false;
            return;
        }
        GlobalPosition = previousPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
    }

    public override void _PhysicsProcess(double delta)
    {
        previousPos = currentPos;
        currentPos = projectile.GlobalTransform.Origin;
    }
}
