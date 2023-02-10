using System;
using System.Collections.Generic;
using Godot;

//TODO:
// Make player face movement dir instead of input (rotate towards velocity)
// ^ A) Does it face input anyways? What about move in dir of rotation.

public partial class KinematicPlayer : CharacterBody3D
{
	private const int MovementSpeed = 4;
	private const int Acceleration = 30;
	private const int Friction = 100;
	private const int AirFriction = 10;
	private const float Gravity = -9.81f;
	private const int JumpImpulse = 5;
	private const int RotationSpeed = 4;
	private const int MaxSpringLength = 4;
	private const float MouseSensitivity = 0.2f;
	public const float AirRotationDamping = 5;

	//get "Weapon class from the spatial" - all weapon logic will go into it's own class
	//[Export] public int currentSelected;
	public static List<InventoryItem> Inventory = new();
	private Marker3D armY;
	public bool FirstPerson;
	private Camera3D hologramCamera;
	private Marker3D hologramCameraPos;
	private TextureRect hologramRect;
	private SubViewport hologramViewport;
	private Camera3D inventoryCamera;
	private Marker3D inventoryCameraPos;
	private Panel inventoryPanel;
	private SubViewport inventoryViewport;
	private Panel phoneScreen;
	private Marker3D pivot;

	private SpringArm3D springArm;
	private Vector3 velocity = Vector3.Zero;
	private Vector3 lastDirection = Vector3.Zero;
	private Vector3 snapVector = Vector3.Zero;
	private Vector3 lastVelocity = Vector3.Zero;

	public override void _Ready()
	{
		//Find node exists as well.
		Input.MouseMode = Input.MouseModeEnum.Captured;
		armY = GetNode<Marker3D>("SpringArmY");
		springArm = armY.GetNode<SpringArm3D>("SpringArm3D");
		pivot = GetNode<Marker3D>("Pivot");
		hologramRect = GetNode("PlayerHUD").GetNode<TextureRect>("TextureRect");
		hologramViewport = pivot.GetNode<SubViewport>("HoloViewport");
		hologramCamera = hologramViewport.GetNode<Camera3D>("HoloCamera");
		hologramCameraPos = pivot.GetNode<Marker3D>("HoloCamPosition");
		inventoryPanel = GetNode("PlayerHUD").GetNode<Panel>("InvPanel");
		inventoryViewport = pivot.GetNode<SubViewport>("InvViewport");
		inventoryCamera = inventoryViewport.GetNode<Camera3D>("InvCamera");
		inventoryCameraPos = pivot.GetNode<Marker3D>("InvCamPosition");
		phoneScreen = GetNode("PlayerHUD").GetNode<Panel>("PhoneScreen");

		inventoryPanel.Visible = false;
		phoneScreen.Visible = false;

		DrawHologramViewport();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("click"))
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if (@event.IsActionPressed("toggle_capture"))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
				? Input.MouseModeEnum.Visible
				: Input.MouseModeEnum.Captured;
		}
		else if (@event.IsActionPressed("game_zoom_in"))
		{
			if (springArm.SpringLength >= 1)
			{
				// Hide the zoom as we pass through the head by temporarily "blinking" the screen.
				if (Mathf.FloorToInt(springArm.SpringLength) == 1)
				{
					var tween = CreateTween();
					tween.TweenProperty
					(
						GetNode<ColorRect>("%ViewFadeRect"),
						"modulate",
						Colors.Black,
						0.1f
					);
					tween.TweenProperty
					(
						GetNode<ColorRect>("%ViewFadeRect"),
						"modulate",
						Colors.Transparent,
						0.1f
					).SetDelay(0.1);
				}

				CreateTween().TweenProperty
				(
					springArm,
					"spring_length",
					Mathf.Floor(springArm.SpringLength) - 1,
					0.2f
				)
				.SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Sine);
			}
		}
		else if (@event.IsActionPressed("game_zoom_out"))
		{
			if (springArm.SpringLength < MaxSpringLength)
			{
				// Hide the zoom as we pass through the head by temporarily "blinking" the screen.
				if (Mathf.FloorToInt(springArm.SpringLength) == 0)
				{
					var tween = CreateTween();
					tween.TweenProperty
					(
						GetNode<ColorRect>("%ViewFadeRect"),
						"modulate",
						Colors.Black,
						0.1f
					);
					tween.TweenProperty
					(
						GetNode<ColorRect>("%ViewFadeRect"),
						"modulate",
						Colors.Transparent,
						0.1f
					).SetDelay(0.1);
				}

				CreateTween().TweenProperty
				(
					springArm,
					"spring_length",
					Mathf.Floor(springArm.SpringLength) + 1,
					0.2f
				)
				.SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Sine);
			}
		}
		else if (@event.IsActionPressed("game_phone"))
		{
			TogglePhoneVisibility();
		}

		if (@event is InputEventMouseMotion eventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			armY.RotateY(Mathf.DegToRad(-eventMouseMotion.Relative.X * MouseSensitivity));
			springArm.RotateX(Mathf.DegToRad(-eventMouseMotion.Relative.Y * MouseSensitivity));
			springArm.Rotation = new Vector3(Mathf.Clamp(springArm.Rotation.X, Mathf.DegToRad(-75), Mathf.DegToRad(75)),
				springArm.Rotation.Y, springArm.Rotation.Z);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var direction = GetDirection(GetInputVector());
		ApplyMovement(direction, delta);
		ApplyFriction(direction, delta);
		ApplyGravity(delta);
		UpdateSnapVector();
		CheckJump();
		MoveSpringArm();

		Velocity = velocity;
		MoveAndSlide();
		velocity = Velocity;

		//TODO: Constantly rotate camera to face back of player / pivot back
		//camera rotation = lerp player pivot rotation,
		//blocked by mouse movement somehow? would still need to get mouse motion in process
		//springArm.Rotation = new Vector3(springArm.Rotation.X, Mathf.Lerp(springArm.Rotation.Z, 0, 1), springArm.Rotation.Z);

		lastDirection = direction;
	}

	///<summary>Per-frame operations. Not to be used for movement or other related things.</summary>
	public override void _Process(double delta)
	{
		var inputVector = GetInputVector();
		var direction = GetDirection(inputVector); //TODO: THESE SHOULD BE GLObAL - used in bothj process & physprocess!

		hologramCamera.GlobalTransform = hologramCameraPos.GlobalTransform;
		inventoryCamera.GlobalTransform = inventoryCameraPos.GlobalTransform;
		ApplyRotation(delta, direction); //separate from movement to remove annoying mouse rotation delay due to Godot input inconsistency
	}
	
	private Vector3 GetInputVector()
	{
		var inputVector = Vector3.Zero;
		inputVector.X = Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left");
		inputVector.Z = Input.GetActionStrength("game_backward") - Input.GetActionStrength("game_forward");

		return inputVector.Length() > 1 ? inputVector.Normalized() : inputVector;
	}

	private Vector3 GetDirection(Vector3 inputVector)
	{
		return inputVector.X * armY.Transform.Basis.X + inputVector.Z * armY.Transform.Basis.Z;
	}

	// Movement is applied in whatever direction player is currently facing, direction is vector magnitude for movement
	private void ApplyMovement(Vector3 direction, double delta)
	{
		if (direction == Vector3.Zero)
		{
			return;
		}

		if (FirstPerson)
		{
			velocity.X = Mathf.MoveToward
			(
				velocity.X,
				direction.X * MovementSpeed,
				(float) (Acceleration * delta)
			);
			
			velocity.Z = Mathf.MoveToward
			(
				velocity.Z,
				direction.Z * MovementSpeed,
				(float) (Acceleration * delta)
			);
		}
		else
		{
			// we basically need to make velocity the direction we are facing
			// SO we just use W, and use forward velocity
			velocity.X = Mathf.MoveToward
			(
				velocity.X,
				-Mathf.Sin(pivot.Rotation.Y) * MovementSpeed,  // velocity.X + ... (for now we ignore the effects of the previous velocity (momentum))
				(float) (Acceleration * delta)
			);

			velocity.Z = Mathf.MoveToward
			(
				velocity.Z,
				-Mathf.Cos(pivot.Rotation.Y) * MovementSpeed, // velocity.Z + ... (for now we ignore the effects of the previous velocity (momentum))
				(float) (Acceleration * delta)
			);
		}
		
		lastVelocity.X = velocity.X;
		lastVelocity.Z = velocity.Z;
	}

	private void ApplyRotation(double delta, Vector3 direction)
	{
		if (FirstPerson)
		{
			pivot.Rotation = new Vector3(
				pivot.Rotation.X,
				armY.Rotation.Y,
				pivot.Rotation.Z
			);
			
			return;
		}

		if (direction == Vector3.Zero)
		{
			return;
		}

		pivot.Rotation = new Vector3(
			pivot.Rotation.X,
			Mathf.LerpAngle
			(
				pivot.Rotation.Y,
				Mathf.Atan2(-direction.X, -direction.Z), (float) (RotationSpeed * delta) / (IsOnFloor() ? 1 : AirRotationDamping)
			),
			pivot.Rotation.Z
		);
	}

	private void ApplyFriction(Vector3 direction, double delta)
	{
		if (direction != Vector3.Zero)
		{
			return;
		}

		if (IsOnFloor())
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, (float) (Friction * delta));
			velocity.Y = Mathf.MoveToward(velocity.Y, 0, (float) (Friction * delta));
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, (float) (Friction * delta));
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, (float) (AirFriction * delta));
			velocity.Y = Mathf.MoveToward(velocity.Y, 0, (float) (AirFriction * delta));
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, (float) (AirFriction * delta));
		}
	}

	private void ApplyGravity(double delta)
	{
		velocity.Y += (float) (Gravity * delta);
		velocity.Y = Mathf.Clamp(velocity.Y, Gravity, JumpImpulse);
	}

	private void UpdateSnapVector()
	{
		snapVector = IsOnFloor() ? -GetFloorNormal() : Vector3.Down;
	}

	private void CheckJump()
	{
		if (Input.IsActionJustPressed("game_jump") && IsOnFloor())
		{
			snapVector = Vector3.Zero;
			velocity.Y = JumpImpulse;
		}

		if (Input.IsActionJustReleased("game_jump") && velocity.Y > (float) JumpImpulse / 2)
		{
			velocity.Y = (float) JumpImpulse / 2;
		}
	}

	private void MoveSpringArm()
	{
		if (springArm.SpringLength == 0)
		{
			FirstPerson = true;
			springArm.Position = new Vector3(0, 1.5f, -0.4f);
		}
		else
		{
			FirstPerson = false;
			springArm.Position = new Vector3(Mathf.Lerp(springArm.Position.X, 0, 0.1f),
				Mathf.Lerp(springArm.Position.Y, 1.5f, 0.1f), 0);
		}
	}

	private void DrawHologramViewport()
	{
		hologramRect.Texture = hologramViewport.GetTexture();
	}
	
	private void TogglePhoneVisibility()
	{
		var tween = CreateTween();
		tween.TweenProperty
		(
			phoneScreen,
			"position",
			GetNode("PlayerHUD").GetNode<Panel>(phoneScreen.Visible ? "ClosedPosition" : "OpenedPosition").Position,
			0.2f
		)
		.SetTrans(Tween.TransitionType.Cubic)
		.SetEase(Tween.EaseType.In);
		tween.TweenCallback(Callable.From(() =>
		{
			phoneScreen.Visible = !phoneScreen.Visible;
		}));

		if (phoneScreen.Visible)
		{
			GetNode("PlayerHUD").GetNode("PhoneScreen").GetNode("Cursor").Set("FollowMouse", true);
		}
		else
		{
			GetNode("PlayerHUD").GetNode("PhoneScreen").GetNode("Cursor").Set("FollowMouse", true);
			Input.WarpMouse(GetNode("PlayerHUD").GetNode<Panel>("OpenedPosition").Position +
							GetNode("PlayerHUD").GetNode<Panel>("OpenedPosition").Size / 2);
		}
	}
}
