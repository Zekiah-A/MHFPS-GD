using Godot;

public partial class TitleManager : Node3D
{
	private Camera3D camera;
	private AnimationPlayer cameraAnimationPlayer;
	[Export] public Vector3 EndCameraPosition = new(0, 2, 13);
	[Export] public Vector3 EndCameraRotation = Vector3.Zero;
	[Export] public Vector3 InitialCameraPosition = new(0, 2, 7.5f);
	[Export] public Vector3 InitialCameraRotation = new(0, 0, 0.872665f);
	[Export] public float TweenSpeed = 2.5f;

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("Camera3D");
		cameraAnimationPlayer = GetNode<AnimationPlayer>("Camera3D/CameraAnimationPlayer");
		cameraAnimationPlayer.AnimationFinished += _ => cameraAnimationPlayer.Play("title_camera_animation");

		camera.Position = InitialCameraPosition;
		camera.Rotation = InitialCameraRotation;

		var tween = CreateTween().SetParallel();
		tween.TweenProperty
		(
			camera,
			"position",
			EndCameraPosition,
			TweenSpeed
		)
		.SetTrans(Tween.TransitionType.Cubic)
		.SetEase(Tween.EaseType.Out);
		tween.TweenProperty
		(
			camera,
			"rotation",
			EndCameraRotation,
			TweenSpeed
		)
		.SetTrans(Tween.TransitionType.Cubic)
		.SetEase(Tween.EaseType.Out);
		tween.TweenProperty
		(
			GetNode<ColorRect>("IntroFadeColorRect"),
			"modulate",
			Colors.Transparent,
			TweenSpeed
		);
		tween.Chain().TweenCallback
		(
			Callable.From(() => cameraAnimationPlayer.Play("title_camera_animation"))
		);
		tween.Play();
		
		var a = GetNode("GenericModel").GetNode<AnimationPlayer>("AnimationPlayer");
		a.AnimationFinished += _ => { a.Play("Idle"); };
		a.Play("Idle");
		a.SpeedScale = 1.25f;
		var b = GetNode("GenericModel2").GetNode<AnimationPlayer>("AnimationPlayer");
		b.AnimationFinished += _ => { b.Play("Idle"); };
		b.Play("Idle");
		b.SpeedScale = 0.7f;
		var c = GetNode("GenericModel3").GetNode<AnimationPlayer>("AnimationPlayer");
		c.AnimationFinished += _ => { c.Play("Idle"); };
		c.Play("Idle");
	}
}
