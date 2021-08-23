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
	
	
	public override void _Ready()
	{
		camera =  GetNode<Camera>("Camera");
		introTween = GetNode<Tween>("IntroTween");
		fadeAnimation = GetNode("IntroFade").GetNode<AnimationPlayer>("AnimationPlayer");
		
		PlayIntroAnimation();
	}
	
	private void PlayIntroAnimation()
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
	}
}
