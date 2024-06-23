using Godot;
using System;

public partial class LaserProjectileInterpolator : MeshInstance3D
{
	[Export]
	private RigidBody3D projectile;

    private Vector3 previousPos;
    private Vector3 currentPos;

    public override void _Ready()
    {
        if (projectile == null) QueueFree();
        //we need to initialize these values to avoid the projectile spawning at the world origin (0,0,0)
        previousPos = projectile.GlobalTransform.Origin;
        currentPos = projectile.GlobalTransform.Origin;
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
