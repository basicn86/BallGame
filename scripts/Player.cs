using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : RigidBody3D
{
	private Node3D twist;


	[Export]
	public MeshInstance3D playerModel;
	[Export]
	public RayCast3D groundCast;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		twist = GetNode<Node3D>("../TwistPivot");
		groundCast.TopLevel = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateGroundCast();

		Jump();

		Vector3 moveVector = new Vector3();
		moveVector.X = Input.GetAxis("left", "right");
		moveVector.Z = Input.GetAxis("forward", "backward");
		moveVector *= twist.Basis.Inverse();
		moveVector.Y = 0;

		moveVector = moveVector.Normalized();

		ApplyCentralForce(moveVector * 1000f * (float)delta);

		SmoothPlayerMotion(delta);

		

		if (Input.IsKeyPressed(Key.Key1))
		{
			GD.Print("Set FPS to 120");
			Engine.MaxFps = 122;
		}
		if (Input.IsKeyPressed(Key.Key2))
		{
			GD.Print("Set FPS to 60");
			Engine.MaxFps = 60;
		}
		if (Input.IsKeyPressed(Key.Key3))
		{
			GD.Print("Set FPS to 30");
			Engine.MaxFps = 30;
		}
		if(Input.IsKeyPressed(Key.Key4))
		{
			GD.Print("Set FPS to 1000");
			Engine.MaxFps = 1000;
		}
	}

	private Vector3 previousPos;
	private Vector3 currentPos;
	private void SmoothPlayerMotion(double delta)
	{
		playerModel.TopLevel = true;
		playerModel.Rotation = Rotation;
		playerModel.GlobalPosition = previousPos.Lerp(currentPos, (float)Engine.GetPhysicsInterpolationFraction());
	}

	private void Jump()
	{
		if (Input.IsActionJustPressed("ui_accept") && groundCast.IsColliding())
		{
			ApplyCentralImpulse(new Vector3(0, 10f, 0));
		}

		//Let the player jump higher if the jump button is held down
		if (!Input.IsActionPressed("ui_accept") && !groundCast.IsColliding())
		{
			GravityScale = 2f;
		}
		else
		{
			GravityScale = 1f;
		}
	}

	private void UpdateGroundCast()
	{
		groundCast.Position = GlobalTransform.Origin;
	}

	public override void _PhysicsProcess(double delta)
	{
		previousPos = currentPos;
		currentPos = GlobalTransform.Origin;
	}

	//version 1, save it for later
	/*private void SmoothPlayerMotion(double delta)
	{
		float fps = 1f / (float)delta;

		if (fps > Engine.PhysicsTicksPerSecond)
		{
			playerModel.TopLevel = true;
			playerModel.Rotation = playerModel.Rotation.Lerp(Rotation, 40f * (float)delta);
			playerModel.GlobalPosition = playerModel.GlobalPosition.Lerp(GlobalTransform.Origin, 40f * (float)delta);
			GD.Print("Smoothing");
		}
		else
		{
			playerModel.GlobalPosition = GlobalTransform.Origin;
			playerModel.TopLevel = false;
			GD.Print("Not Smoothing");
		}
	}*/
}
