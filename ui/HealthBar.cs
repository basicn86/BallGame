using Godot;
using System;

//TODO: add it to the proper namespace

public partial class HealthBar : TextureProgressBar
{
	public static HealthBar Instance;

	private int targetValue;
	private float currentValue;

	public override void _Ready()
	{
		if (Instance != null)
		{
			QueueFree();
			return;
		}
		Instance = this;
		currentValue = 100f;
		targetValue = 100;
	}

	public override void _Process(double delta)
	{
		if (currentValue != targetValue)
		{
			currentValue = Mathf.Lerp(currentValue, targetValue, (float)delta * 4.0f);
			Value = currentValue;
		}
	}

	public void SetHealth(int health)
	{
		targetValue = health;
	}
}
