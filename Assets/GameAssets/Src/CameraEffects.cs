using System;
using Godot;

public partial class CameraEffects : Camera3D
{
	private Timer cameraTimer;
	private int incrementX = 98043, incrementY = 62356;
	
	[Export] public float Damping = 10f;
	[Export] public bool Inverse;
	[Export] public float LerpStrength = 0.1f;

	private float posx, posy;

	private Random random;

	//private Camera3D camera;
	[Export] public bool Random;
	//OpenSimplexNoise noise = new OpenSimplexNoise();


	public override void _Ready()
	{
		if (!Random) return;

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

	public override void _Process(double delta) //input tween
	{
		if (!Random)
		{
			var (x, y) = GetViewport().GetMousePosition();
			posx = (x - GetViewport().GetVisibleRect().Size.X / 2) / GetViewport().GetVisibleRect().Size.X;
			posy = (y - GetViewport().GetVisibleRect().Size.Y / 2) / GetViewport().GetVisibleRect().Size.Y;
		}

		Rotation = new Vector3(
			Mathf.Lerp(Rotation.Y, Inverse ? posy / Damping : -posy / Damping, LerpStrength),
			Mathf.Lerp(Rotation.Y, Inverse ? posx / Damping : -posx / Damping, LerpStrength),
			Rotation.Z
		);
	}

	private void OnTimerComplete()
	{
		///<note> Value must be between -0.5 and 0.5 </note>
		incrementX++;
		incrementY++;
		//posx = noise.GetNoise1d(incrementx) / 2;
		//posy = noise.GetNoise1d(-incrementx) / 2;
	}
}
