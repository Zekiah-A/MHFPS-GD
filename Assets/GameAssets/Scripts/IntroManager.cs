using Godot;
using System;
using DiscordRPC.Exceptions;

public class IntroManager : Spatial
{
	private Timer creditsTimer;
	private AnimationPlayer cameraAnimationPlayer;
	private uint defaultCullMask;
	
	public override async void _Ready()
	{
		creditsTimer = GetNode<Timer>("CutsceneTimer");
		cameraAnimationPlayer = GetNode<AnimationPlayer>("Camera/CameraAnimationPlayer");
		var creditsAnimationPlayer = GetNode<AnimationPlayer>("IntroUI/CreditsPanel/AnimationPlayer");
		
		//Stop camera from rendering while the intro is playing, make sure to reset the cull mask to original value to restore render.
		defaultCullMask = GetNode<Camera>("Camera").CullMask;
		GetNode<Camera>("Camera").CullMask = 0;
		creditsAnimationPlayer.Play("intro_credits");
		
		//Wait for the intro GUI cutscene to start, before playing intro.
		await ToSignal(creditsAnimationPlayer, "animation_finished");
		PlayIntro();
	}

	private void PlayIntro()
	{
		GetNode<Camera>("Camera").CullMask = defaultCullMask;
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
