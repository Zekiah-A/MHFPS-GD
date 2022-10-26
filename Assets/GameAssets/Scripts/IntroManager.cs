using Godot;
using System;
using DiscordRPC.Exceptions;

public partial class IntroManager : Node3D
{
	private Timer creditsTimer;
	private AnimationPlayer cameraAnimationPlayer;
	private uint defaultCullMask;
	
	public override void _Ready()
	{
		creditsTimer = GetNode<Timer>("CutsceneTimer");
		cameraAnimationPlayer = GetNode<AnimationPlayer>("Camera3D/CameraAnimationPlayer");
		var creditsAnimationPlayer = GetNode<AnimationPlayer>("IntroUI/CreditsPanel/AnimationPlayer");
		
		//Stop camera from rendering while the intro is playing, make sure to reset the cull mask to original value to restore render.
		defaultCullMask = GetNode<Camera3D>("Camera3D").CullMask;
		GetNode<Camera3D>("Camera3D").CullMask = 0;
		creditsAnimationPlayer.Play("intro_credits");
		
		//Wait for the intro GUI cutscene to start, before playing intro.
		//await ToSignal(creditsAnimationPlayer, "animation_finished");
		//PlayIntro();
	}

	private void PlayIntro()
	{
		GetNode<Camera3D>("Camera3D").CullMask = defaultCullMask;
		GetNode<Panel>("IntroUI/CreditsPanel").Visible = false;
		GetNode<ColorRect>("IntroUI/CreditsBackground").Visible = false;
		
		var tween = CreateTween();
		tween.TweenProperty
		(
			GetNode<Panel>("IntroUI/CinematicBars"),
			"scale",
			new Vector2(1f, 1f),
			1f
		)
		.SetTrans(Tween.TransitionType.Cubic)
		.SetEase(Tween.EaseType.Out);
		tween.Play();
		
		cameraAnimationPlayer.Play("intro_camera");
	}
}
