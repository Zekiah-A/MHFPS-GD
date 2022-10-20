using Godot;
using System;

public partial class CameraEffects : Camera3D
{
	//private Camera3D camera;
	[Export] public bool Random = false;
	[Export] public bool Inverse = false;
	[Export] public float Damping = 10f;
	[Export] public float LerpStrength = 0.1f;

	private float posx = 0, posy = 0;
	private int incrementx = 98043, incrementy = 62356;

	private Random random;
	private Timer cameraTimer;
	//OpenSimplexNoise noise = new OpenSimplexNoise();


	public override void _Ready()
	{
		if (!Random) return;
		
		random = new Random();
		try
		{
			cameraTimer = GetNode<Timer>("Timer");
			cameraTimer.Connect("timeout",new Callable(this,nameof(OnTimerComplete)));
			cameraTimer.Start();
		}
		catch (Exception e)
		{
			GD.Print($"Random camera movement requires a timer with autostart enabled as a child.\n{e}");
		}
	}

	public override void _Process(double delta) //input tween
	{
		if (!Random)
		{
			var (x, y) = GetViewport().GetMousePosition();
			posx = (x  - GetViewport().GetVisibleRect().Size.x / 2) / GetViewport().GetVisibleRect().Size.x;
			posy = (y  - GetViewport().GetVisibleRect().Size.y / 2) / GetViewport().GetVisibleRect().Size.y;
		}

		Rotation = new Vector3 (
			Mathf.Lerp(Rotation.x, Inverse ? posy / Damping : -posy / Damping, LerpStrength),
			Mathf.Lerp(Rotation.y, Inverse ? posx / Damping : -posx / Damping, LerpStrength),
			Rotation.z
		);
	}
	
	private void OnTimerComplete()
	{
		///<note> Value must be between -0.5 and 0.5 </note>
		incrementx++; incrementy++;
		//posx = noise.GetNoise1d(incrementx) / 2;
		//posy = noise.GetNoise1d(-incrementx) / 2;
	}
}
