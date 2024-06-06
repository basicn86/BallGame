using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : RigidBody3D
{
	private Node3D twist;


	[Export]
	public MeshInstance3D playerModel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		twist = GetNode<Node3D>("../TwistPivot");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 moveVector = new Vector3();
		moveVector.X = Input.GetAxis("left", "right");
		moveVector.Z = Input.GetAxis("forward", "backward");
		moveVector *= twist.Basis.Inverse();
		moveVector.Y = 0;

		moveVector = moveVector.Normalized();

		ApplyCentralForce(moveVector * 1000f * (float)delta);


		if (Input.IsActionJustPressed("ui_accept"))
		{
			ApplyCentralImpulse(new Vector3(0, 5f, 0));
		}

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
