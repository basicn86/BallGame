using Godot;
using System;

public partial class CoinCounter : Label
{
	public static CoinCounter Instance { get; private set; }
	private int _coinCount = 0;

	[Export]
	private AudioStreamPlayer CoinSound;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
			UpdateCoinDisplay();
		}
		else
		{
			GD.PrintErr("CoinCounter instance already exists!");
			QueueFree();
			return;
		}
	}

	public void AddCoins(int amount)
	{
		if(CoinSound.GetPlaybackPosition() > 0.05f || !CoinSound.Playing)
		{
			CoinSound.PitchScale = (float)GD.RandRange(0.98f, 1.0f);
			CoinSound.Play(0.02f);
		}
		_coinCount += amount;
		UpdateCoinDisplay();
	}

	public int GetCoinCount()
	{
		return _coinCount;
	}

	public void RemoveCoins(int amount)
	{
		_coinCount -= amount;
		if (_coinCount < 0) _coinCount = 0; // Prevent negative coin count
		UpdateCoinDisplay();
	}

	public void UpdateCoinDisplay()
	{
		Text = "Coins: " + _coinCount.ToString();
	}
}
