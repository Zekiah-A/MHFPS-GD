using Godot;
using System;

public class IntroManager : Spatial
{
	private Timer creditsTimer;
	private AnimationPlayer cameraAnimationPlayer;
	
	public override void _Ready()
	{
		creditsTimer = GetNode<Timer>("CutsceneTimer");
		cameraAnimationPlayer = GetNode<AnimationPlayer>("Camera/CameraAnimationPlayer");

		GetNode<AnimationPlayer>("IntroUI/CreditsPanel/AnimationPlayer").Play("intro_credits");
		creditsTimer.Connect("timeout", this, nameof(PlayIntro));
		creditsTimer.Start(); //Should yield for animation to finish instead
	}

	public void PlayIntro()
	{
		GetNode<Panel>("IntroUI/CreditsPanel").Visible = false;
		GetNode<ColorRect>("IntroUI/CreditsBackground").Visible = false;
		
		GetNode<Tween>("IntroUI/CinematicBars/Tween").InterpolateProperty(
			GetNode<Panel>("IntroUI/CinematicBars"),
			"rect_scale",
			new Vector2(1f, 1.275f),
			new Vector2(1f, 1f),
			1f,
			Tween.TransitionType.Cubic,
			Tween.EaseType.Out
		);
		GetNode<Tween>("IntroUI/CinematicBars/Tween").Start();
		
		cameraAnimationPlayer.Play("intro_camera");		
	}
}
