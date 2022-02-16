using Godot;
using System;

public class Intro : Spatial
{
	
	
	public override void _Ready()
	{
		GetNode<Tween>("Control/CinematicBars/Tween").InterpolateProperty(
			GetNode<Panel>("Control/CinematicBars"),
			"rect_scale",
			new Vector2(1f, 1.275f),
			new Vector2(1f, 1f),
			1f,
			Tween.TransitionType.Cubic,
			Tween.EaseType.Out
		);
		GetNode<Tween>("Control/CinematicBars/Tween").Start();
	}

	public override void _Process(float delta)
	{
		
	}
}
