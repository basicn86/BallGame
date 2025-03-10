using Godot;
using System;
using System.Linq;

public partial class ExploderComponent : Area3D
{
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _on_timer_timeout()
	{
		Explode();
	}

	public void Explode()
	{
		ulong parentInstanceId = GetParent().GetInstanceId();

		foreach (var item in GetOverlappingBodies())
		{
			//skip the parent object
			if (item.GetInstanceId() == parentInstanceId) continue;

			if(item is RigidBody3D)
			{
				RigidBody3D i = (RigidBody3D)item;

				Vector3 forceDirection = i.GlobalTransform.Origin - GlobalTransform.Origin;
				float distance = forceDirection.Length();

				forceDirection = forceDirection.Normalized();

				i.ApplyCentralForce(forceDirection * 1000f / distance);
			}
		}
	}
}
