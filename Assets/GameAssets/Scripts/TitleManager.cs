using Godot;
using System;

public class TitleManager : Spatial
{
	[Export] public float TweenSpeed = 2.5f;
	[Export] public Vector3 InitialCameraPosition = new Vector3(0, 2, 7.5f);
	[Export] public Vector3 EndCameraPosition = new Vector3(0, 2, 13);
	[Export] public Vector3 InitialCameraRotation = new Vector3(0, 0, 50);
	[Export] public Vector3 EndCameraRotation = Vector3.Zero;
	
	private Camera camera;
	private Tween introTween;
	private AnimationPlayer fadeAnimation;
	private AnimationPlayer cameraAnimationPlayer;
	
	
	public override void _Ready()
	{
		camera =  GetNode<Camera>("Camera");
		introTween = GetNode<Tween>("IntroTween");
		fadeAnimation = GetNode("IntroFade").GetNode<AnimationPlayer>("AnimationPlayer");
		cameraAnimationPlayer = GetNode("Camera").GetNode<AnimationPlayer>("CameraAnimationPlayer");
		
		PlayIntroAnimation();
	}
	
	private async void PlayIntroAnimation()
	{
		introTween.InterpolateProperty (
			camera,
			"translation", //property
			InitialCameraPosition, //from
			EndCameraPosition, //to
			TweenSpeed, //speed
			Tween.TransitionType.Cubic,
			Tween.EaseType.Out
		);

		introTween.InterpolateProperty (
			camera,
			"rotation_degrees",
			InitialCameraRotation,
			EndCameraRotation,
			TweenSpeed,
			Tween.TransitionType.Cubic,
			Tween.EaseType.Out
		);

		fadeAnimation.Play("FadeAnimation");
		introTween.Start();
		
		//Block camera animation from occuring during the intro.
		await ToSignal(introTween, "tween_all_completed");
		cameraAnimationPlayer.Play("title_camera_animation");
		
		//TODO: Implement these properly
		GetNode("GenericModel").GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
		GetNode("GenericModel").GetNode<AnimationPlayer>("AnimationPlayer").PlaybackSpeed = 1.25f;
		GetNode("GenericModel2").GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
		GetNode("GenericModel").GetNode<AnimationPlayer>("AnimationPlayer").PlaybackSpeed = 0.7f;
		GetNode("GenericModel3").GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
	}
	
	private void OnCameraAnimationPlayerFinished(string animName)
	{
		cameraAnimationPlayer.Play("title_camera_animation");
	}
	private void OnGenericAnimationFinished(string animName)
	{
		GetNode("GenericModel").GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
	}
	private void OnGeneric2AnimationFinished(string animName)
	{
		GetNode("GenericModel2").GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
	}
	private void OnGeneric3AnimationFinished(string animName)
	{
		GetNode("GenericModel3").GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
	}
}
