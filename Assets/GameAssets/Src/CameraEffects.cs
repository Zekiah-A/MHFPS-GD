using System;
using Godot;

public partial class CameraEffects : Camera3D
{
	private Timer cameraTimer;
	private int seedX = 98043;
	private int seedY = 62356;
	private float posX;
	private float posY;
	private Random random;
	private FastNoiseLite noise = new FastNoiseLite();

	[Export] public bool Random;
	[Export] public float Damping = 10f;
	[Export] public bool Inverse;
	[Export] public float LerpStrength = 0.1f;

	public override void _Ready()
	{
		if (Random) 
		{
			random = new Random();
			try
			{
				cameraTimer = GetNode<Timer>("Timer");
				cameraTimer.Connect("timeout", new Callable(this, nameof(OnTimerComplete)));
				cameraTimer.Start();
			}
			catch (Exception e)
			{
				GD.PrintErr($"Random camera movement requires a timer with autostart enabled as a child.\n{e}");
			}
		}
	}

	public override void _Process(double delta) //input tween
	{
		if (!Random)
		{
			var (x, y) = GetViewport().GetMousePosition();
			posX = (x - GetViewport().GetVisibleRect().Size.X / 2) / GetViewport().GetVisibleRect().Size.X;
			posY = (y - GetViewport().GetVisibleRect().Size.Y / 2) / GetViewport().GetVisibleRect().Size.Y;
		}

		Rotation = new Vector3
		(
			Mathf.Lerp(Rotation.X, Inverse ? posY / Damping : -posY / Damping, LerpStrength),
			Mathf.Lerp(Rotation.Y, Inverse ? posX / Damping : -posX / Damping, LerpStrength),
			Rotation.Z
		);
	}

	private void OnTimerComplete()
	{
		// Value must be between -0.5 and 0.5
		seedX++;
		seedY++;
		posX = noise.GetNoise1D(seedX) / 2;
		posY = noise.GetNoise1D(-seedX) / 2;
	}
}
