using Godot;
using System;

public partial class TitleManager : Node3D
{
	[Export] public float TweenSpeed = 2.5f;
	[Export] public Vector3 InitialCameraPosition = new(0, 2, 7.5f);
	[Export] public Vector3 EndCameraPosition = new(0, 2, 13);
	[Export] public Vector3 InitialCameraRotation = new(0, 0, 0.872665f);
	[Export] public Vector3 EndCameraRotation = Vector3.Zero;
	
	private Camera3D camera;
	private AnimationPlayer fadeAnimation;
	private AnimationPlayer cameraAnimationPlayer;
	
	public override void _Ready()
	{
		camera =  GetNode<Camera3D>("Camera3D");
		fadeAnimation = GetNode("IntroFade").GetNode<AnimationPlayer>("AnimationPlayer");
		cameraAnimationPlayer = GetNode("Camera3D").GetNode<AnimationPlayer>("CameraAnimationPlayer");
		
		camera.Position = InitialCameraPosition;
		camera.Rotation = InitialCameraRotation;
		
		var tween = CreateTween();
		tween.TweenProperty
		(
			camera,
			"position",
			EndCameraPosition,
			TweenSpeed
		)
		.SetTrans(Tween.TransitionType.Cubic)
		.SetEase(Tween.EaseType.Out);

		tween.Chain()
		.TweenProperty
		(
			camera,
			"rotation",
			EndCameraRotation,
			TweenSpeed
		)
		.SetTrans(Tween.TransitionType.Cubic)
		.SetEase(Tween.EaseType.Out);
		
		tween.Chain()
		.TweenCallback(new Callable(this, nameof(PlayCameraAnimation)));
		
		fadeAnimation.Play("FadeAnimation");
		tween.Play();


		var a = GetNode("GenericModel").GetNode<AnimationPlayer>("AnimationPlayer");
		a.Play("Idle");
		a.SpeedScale = 1.25f;
		var b = GetNode("GenericModel2").GetNode<AnimationPlayer>("AnimationPlayer");
		b.Play("Idle");
		b.SpeedScale = 0.7f;
		var c = GetNode("GenericModel3").GetNode<AnimationPlayer>("AnimationPlayer");
		c.Play("Idle");

		cameraAnimationPlayer.AnimationFinished += _ =>
		{
			cameraAnimationPlayer.Play("title_camera_animation");
		};
	}

	public void PlayCameraAnimation()
	{
		cameraAnimationPlayer.Play("title_camera_animation");
	}
}
