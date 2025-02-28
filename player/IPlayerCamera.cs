using Godot;

internal interface IPlayerCamera
{
    public Vector3 TargetPosition { set; get; }
    public Vector3 GetCrosshairCollisionPoint();
    public Basis Basis { get; set; }

    public void Activate();
}
