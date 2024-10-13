using Godot;
using System;

public partial class LaserProjectileInterpolator : MeshInstance3D
{
	[Export]
	private RigidBody3D projectile;

    public Vector3 previousPos = Vector3.Zero;
    public Vector3 currentPos = Vector3.Zero;

    public override void _Ready()
    {
        if (projectile == null) QueueFree();
    }

    public void ResetInterpolator()
    {
        GlobalPosition = projectile.GlobalPosition;
        currentPos = projectile.GlobalPosition;
        previousPos = projectile.GlobalPosition;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        SmoothPlayerMotion(delta);
    }

    private void SmoothPlayerMotion(double delta)
    {
        TopLevel = true;
        GlobalPosition = previousPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
    }

    public override void _PhysicsProcess(double delta)
    {
        previousPos = currentPos;
        currentPos = projectile.GlobalTransform.Origin;
    }
}
