using Godot;
using System;

public partial class PlayerCameraManager : Node3D
{
	[Export]
	public PlayerCamera StandardCamera;
	[Export]
	public LockOnCamera LockOnCamera;

	private IPlayerCamera _currentCamera;

	private bool _isLockedOn = false;

	private Vector3 _targetPosition;
	public Vector3 TargetPosition
	{
		get { return _targetPosition; }
		set { _targetPosition = value; _currentCamera.TargetPosition = value; }
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("target_lock"))
		{
			if (_isLockedOn)
			{
				_isLockedOn = false;

				StandardCamera.Activate();
				StandardCamera.ResetPosition(LockOnCamera.camera.GlobalPosition);

				_currentCamera = StandardCamera;
			}
			else
			{
				_isLockedOn = true;

				GD.Print("Locking on");

				LockOnCamera.Activate();
				LockOnCamera.ResetPosition(StandardCamera.camera.GlobalPosition);
				_currentCamera = LockOnCamera;
			}
		}
	}

	public override void _Ready()
	{
		_currentCamera = StandardCamera;
	}

	/// <summary>
	/// Returns the current camera basis. Not to be confused with the manager's basis.
	/// </summary>
	public Basis CameraBasis
	{
		get { return _currentCamera.Basis;}
	}

	public Vector3 GetCrosshairCollisionPoint()
	{
		return _currentCamera.GetCrosshairCollisionPoint();
	}
}
