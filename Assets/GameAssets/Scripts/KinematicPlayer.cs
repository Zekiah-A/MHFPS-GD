using Godot;
using System;
using System.Collections.Generic;

//TODO:
// Make player face movement dir instead of input (rotate towards velocity)
// ^ A) Does it face input anyways? What about move in dir of rotation.
// Acceleration tilting (looks liek surfing), rotate around centre of mass

public partial class KinematicPlayer : CharacterBody3D
{
	public bool FirstPerson;

	public const int MaxSpeed = 4;
	public const int Acceleration = 30;
	public const int Friction = 100;
	public const int AirFriction = 10;
	public const float Gravity = -9.8f;
	public const int JumpImpulse = 5;
	public const int RotSpeed = 15;
	public const int MaxSpringLength = 4;
	public const float MouseSensitivity = 0.2f;

	//get "Weapon class from the spatial" - all weapon logic will go into it's own class
	//[Export] public int currentSelected;
	public static int InventoryCurrent = 0;
	public static List<InventoryItem> Inventory = new();
	List<Button> inventoryPanelButtons = new();

	private Vector3 velocity = Vector3.Zero;
	private Vector3 lastVelocity;
	private Vector3 snapVector = Vector3.Zero;

	private SpringArm3D springArm;
	private Marker3D armY;
	private Marker3D pivot;
	private Node3D inventoryNode;
	private TextureRect hologramRect;
	private SubViewport hologramViewport; 
	private Camera3D hologramCamera;
	private Marker3D hologramCameraPos;
	private Panel inventoryPanel;
	private TextureRect inventoryRect;
	private SubViewport inventoryViewport;
	private Camera3D inventoryCamera;
	private Marker3D inventoryCameraPos;
	private Panel phoneScreen;
	
	public override void _Ready()
	{
		//Find node exists as well.
		Input.MouseMode = Input.MouseModeEnum.Captured;
		armY = GetNode<Marker3D>("SpringArmY");
		springArm = armY.GetNode<SpringArm3D>("SpringArm3D");
		pivot = GetNode<Marker3D>("Pivot");
		inventoryNode = pivot.GetNode<Node3D>("Inventory");
		hologramRect = GetNode("PlayerHUD").GetNode<TextureRect>("TextureRect");
		hologramViewport = pivot.GetNode<SubViewport>("HoloViewport");
		hologramCamera = hologramViewport.GetNode<Camera3D>("HoloCamera");
		hologramCameraPos = pivot.GetNode<Marker3D>("HoloCamPosition");
		inventoryPanel = GetNode("PlayerHUD").GetNode<Panel>("InvPanel");
		inventoryRect = inventoryPanel.GetNode<TextureRect>("TextureRect");
		inventoryViewport = pivot.GetNode<SubViewport>("InvViewport");
		inventoryCamera = inventoryViewport.GetNode<Camera3D>("InvCamera");
		inventoryCameraPos = pivot.GetNode<Marker3D>("InvCamPosition");
		phoneScreen = GetNode("PlayerHUD").GetNode<Panel>("PhoneScreen");
		
		inventoryPanel.Visible = false;
		phoneScreen.Visible = false;
		
		UpdateInventory();
		DrawHologramViewport();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("click"))
			Input.MouseMode = Input.MouseModeEnum.Captured;
		else if (@event.IsActionPressed("toggle_capture"))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}
		else if (@event.IsActionPressed("game_zoomin"))
		{
			if (springArm.SpringLength > 0)
				springArm.SpringLength -= 1;
		}
		else if (@event.IsActionPressed("game_zoomout"))
		{
			if (springArm.SpringLength < MaxSpringLength)
				springArm.SpringLength += 1;
		}
		else if (@event.IsActionPressed("game_radial"))
		{
			GD.Print("Open radial inventory here!");
			UpdateInventory();
			OpenInventoryPanel();
			OpenPhone();
		}
		else if (@event.IsActionPressed("ui_bumperl"))
		{
			InventoryCurrent -= 1;
			if (InventoryCurrent < 0)
				InventoryCurrent = 4;
			UpdateInventory();
			UpdateInventoryPanel();
		}
		else if (@event.IsActionPressed("ui_bumperr"))
		{
			InventoryCurrent += 1;
			if (InventoryCurrent > 4)
				InventoryCurrent = 0;
			UpdateInventory();
			UpdateInventoryPanel();
		}

		if (@event is InputEventMouseMotion eventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			armY.RotateY(Mathf.DegToRad(-eventMouseMotion.Relative.X * MouseSensitivity));
			springArm.RotateX(Mathf.DegToRad(-eventMouseMotion.Relative.Y * MouseSensitivity));
			springArm.Rotation = new Vector3(Mathf.Clamp(springArm.Rotation.X, Mathf.DegToRad(-75), Mathf.DegToRad(75)),
				springArm.Rotation.Y, springArm.Rotation.Z);
			//smooth this out somehow - move to process
		}
		else if (@event is InputEventKey eventKey && eventKey.Pressed)
		{	//eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape
			///<summary>checking for number keys.</summary>
			/*if (eventKey.Keycode >= 49 && eventKey.Scancode <= 53)
			{
				InventoryCurrent = (int) eventKey.Scancode - 49;
				UpdateInventory();
				UpdateInventoryPanel();
			}*/
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var inputVector = GetInputVector();
		var direction = GetDirection(inputVector);
		ApplyMovement(inputVector, direction, delta);
		ApplyFriction(direction, delta);
		ApplyGravity(delta);
		UpdateSnapVector();
		Jump();
		//RotateInventory();
		MoveSpringArm();

		//TODO: Make camera controllable with right stick & arrow keys
		Velocity = velocity;
		MoveAndSlide(); //velocity, snapVector, Vector3.Up, true
		velocity = Velocity;
		
		//TODO: Constantly rotate camera to face back of player / pivot back
		//camera rotation = lerp player pivot rotation,
		//blocked by mouse movement somehow? would still need to get mouse motion in process
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

	//<summary> foreach object under the  "inventory" Empty, add it to the list with all of the parameters found. -- get these "objects", by their "InventoryItem" attached! </summary>
	private void UpdateInventory()
	{
		foreach (var node in inventoryNode.GetChildren())
		{
			if (node is not InventoryItem nodeItem) continue;
			
			if (!Inventory.Contains(nodeItem))
			{
				Inventory.Add(nodeItem);
				GD.Print($"Added item '{node.Name}' to Inventory.");
			}
			
			if (Inventory.IndexOf(nodeItem) == InventoryCurrent)
			{
				//enable this, it is the primary, maybe enable "isEnabled" bool in inventoryite.cs?
				GD.Print($"Enabled - {nodeItem.Name}");
				nodeItem.Enabled = true;
			}
			else
			{
				GD.Print($"Disabled - {nodeItem.Name}");
				nodeItem.Enabled = false;
			}
		}
		
		//(Somewhere else) - only enable curitem, so that it's scripts will run
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
		var direction = (inputVector.X * armY.Transform.Basis.X) + (inputVector.Z * armY.Transform.Basis.Z); 
		return direction;
	}

	private void ApplyMovement(Vector3 inputVector, Vector3 direction, double delta)
	{
		lastVelocity.X = velocity.X;
		lastVelocity.Z = velocity.Z;

		if (direction == Vector3.Zero) return;
		
		velocity.X = Mathf.MoveToward(velocity.X, direction.X * MaxSpeed, (float) (Acceleration * delta));
		velocity.Z = Mathf.MoveToward(velocity.Z, direction.Z * MaxSpeed, (float) (Acceleration * delta));
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
		}
		else
		{
			if (direction != Vector3.Zero)
			{
				pivot.Rotation = new Vector3(
					pivot.Rotation.X,
					Mathf.LerpAngle(pivot.Rotation.Y, Mathf.Atan2(-velocity.X, -velocity.Z), (float) (RotSpeed * delta)),
					pivot.Rotation.Z
				);
			}
		}
	}

	private void ApplyFriction(Vector3 direction, double delta)
	{
		if (direction != Vector3.Zero) return;
		
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

	private void UpdateSnapVector() //use fancy terneary thing
	{
		if (IsOnFloor())
			snapVector = -GetFloorNormal();
		else
			snapVector = Vector3.Down;
	}

	private void Jump()
	{
		if (Input.IsActionJustPressed("game_jump") && IsOnFloor())
		{
			snapVector = Vector3.Zero; //allows plr to jump
			velocity.Y = JumpImpulse;
		}
		if (Input.IsActionJustReleased("game_jump") && velocity.Y > JumpImpulse / 2)
			velocity.Y = JumpImpulse / 2;
	}
/*
	private void RotateInventory()
	{
		inventoryNode.RotationDegrees = new Vector3(Mathf.Lerp(inventoryNode.RotationDegrees.X, springArm.RotationDegrees.X, 0.2f), inventoryNode.RotationDegrees.Y, inventoryNode.RotationDegrees.Z);
	}
*/
	private void MoveSpringArm()
	{
		if (springArm.SpringLength == 0)
		{
			FirstPerson = true;
			springArm.Position = new Vector3(0, 1.5f, -0.4f);
		} else
		{
			FirstPerson = false;
			if (InventoryCurrent < Inventory.Count - 1)
			{
				springArm.Position = Inventory[InventoryCurrent].ItemType switch
				{
					(int) ItemTypes.Weapon => new Vector3(Mathf.Lerp(springArm.Position.X, 0, 0.1f),
						Mathf.Lerp(springArm.Position.Y, 2, 0.1f), 0),
					(int) ItemTypes.MeleeWeapon => new Vector3(Mathf.Lerp(springArm.Position.X, 1, 0.1f),
						Mathf.Lerp(springArm.Position.Y, 1.5f, 0.1f), 0),
					_ => new Vector3(Mathf.Lerp(springArm.Position.X, 0, 0.1f),
						Mathf.Lerp(springArm.Position.Y, 1.5f, 0.1f), 0)
				};
			}
			else
				springArm.Position = new Vector3(Mathf.Lerp(springArm.Position.X, 0, 0.1f), Mathf.Lerp(springArm.Position.Y, 1.5f, 0.1f), 0);
		}
	}

	private void DrawHologramViewport() =>
		hologramRect.Texture = hologramViewport.GetTexture();


	
	private void UpdateInventoryPanel()
	{
		UpdateInventory();

		foreach (var node in inventoryPanel.GetChildren())
		{
			if (node is not Button button) continue;
			inventoryPanelButtons.Add(button);

			button.AddThemeColorOverride("font_color",
				inventoryPanelButtons.IndexOf(button) == InventoryCurrent
					? new Color(0, 0, 1, 1)
					: new Color(1, 1, 1, 0.5f));

			try
			{
				button.Text = $"[{inventoryPanelButtons.IndexOf(button) + 1}] {Inventory[inventoryPanelButtons.IndexOf(button)].Name}";
				button.Icon = ResourceLoader.Load(Inventory[inventoryPanelButtons.IndexOf(button)].InventoryTexture) as Texture2D;
			}
			catch
			{
				button.Text = $"[{inventoryPanelButtons.IndexOf(button) + 1}] ____________";
			}
		}
	}

	private void OpenInventoryPanel()
	{
		if (!inventoryPanel.Visible)
		{
			//inventoryPanel.Visible = true;
			//hologramRect.Visible = false;
			//inventoryRect.Texture2D = (Texture2D) inventoryViewport.GetTexture();
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else
		{
			//inventoryPanel.Visible = false;
			//hologramRect.Visible = true;
			//inventoryRect.Texture2D = null;
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}	
	}

	private async void OpenPhone()
	{
		Tween phoneTween = GetNode("PlayerHUD").GetNode<Tween>("PhoneTween");
		
		//If already tweening, abort (in order to prevent spam).
		//if (!phoneTween.IsActive())
		{
			//If phone screen is open:
			if (phoneScreen.Visible)
			{
				GetNode("PlayerHUD").GetNode("PhoneScreen").GetNode("Cursor").Set("FollowMouse", true);
				/*phoneTween.InterpolateProperty(
					phoneScreen,
					"rect_position",
					GetNode("PlayerHUD").GetNode<Panel>("OpenedPosition")
						.RectPosition, //TODO: Make these const vars to avoid unecessary acess?
					GetNode("PlayerHUD").GetNode<Panel>("ClosedPosition").RectPosition,
					1,
					Tween.TransitionType.Cubic,
					Tween.EaseType.In
				);
				phoneTween.Start();*/
				await ToSignal(phoneTween, "tween_completed");
				phoneScreen.Visible = false;
			}
			else
			{
				GetNode("PlayerHUD").GetNode("PhoneScreen").GetNode("Cursor").Set("FollowMouse", true); //TODO: Make vars for some of these.
				//Input.WarpMouse(GetNode("PlayerHUD").GetNode<Panel>("OpenedPosition").RectPosition + (GetNode("PlayerHUD").GetNode<Panel>("OpenedPosition").RectSize / 2));
				phoneScreen.Visible = true;
				/*phoneTween.InterpolateProperty(
					phoneScreen,
					"rect_position",
					GetNode("PlayerHUD").GetNode<Panel>("ClosedPosition").RectPosition,
					GetNode("PlayerHUD").GetNode<Panel>("OpenedPosition").RectPosition,
					1,
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				phoneTween.Start();*/
			}
		}
	}
	
	private void InventoryButtonPressed(int index)
	{
		InventoryCurrent = index;
		UpdateInventory();
		OpenInventoryPanel();
	}
}
