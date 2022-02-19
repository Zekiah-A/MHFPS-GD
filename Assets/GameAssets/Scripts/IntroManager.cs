using Godot;
using System;

public class IntroManager : Spatial
{
	private Timer creditsTimer;
	private AnimationPlayer cameraAnimationPlayer;
	
	public override async void _Ready()
	{
		creditsTimer = GetNode<Timer>("CutsceneTimer");
		cameraAnimationPlayer = GetNode<AnimationPlayer>("Camera/CameraAnimationPlayer");

		var creditsAnimationPlayer = GetNode<AnimationPlayer>("IntroUI/CreditsPanel/AnimationPlayer");
		creditsAnimationPlayer.Play("intro_credits");

		await ToSignal(creditsAnimationPlayer, "animation_finished");
		PlayIntro();
	}

	private void PlayIntro()
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
