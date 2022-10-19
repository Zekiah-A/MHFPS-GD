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
	public const bool MouseSmoothing = true; //TODO:

	//get "Weapon class from the spatial" - all weapon logic will go into it's own class
	//[Export] public int currentSelected;
	public static int InventoryCurrent = 0;
	public static List<InventoryItem> Inventory = new List<InventoryItem>();
	List<Button> inventoryPanelButtons = new List<Button>();

	private Vector3 velocity = Vector3.Zero;
	private Vector3 lastvelocity;
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
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
				Input.MouseMode = (Input.MouseModeEnum.Visible);
			else
				Input.MouseMode = Input.MouseModeEnum.Captured;
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

		if (@event is InputEventMouseMotion eventMouseMotion && Input.GetMouseMode() == Input.MouseModeEnum.Captured)
		{
			armY.RotateY(Mathf.DegToRad(-eventMouseMotion.Relative.x * MouseSensitivity));
			springArm.RotateX(Mathf.DegToRad(-eventMouseMotion.Relative.y * MouseSensitivity));
			springArm.Rotation = new Vector3(Mathf.Clamp(springArm.Rotation.x, Mathf.DegToRad(-75), Mathf.DegToRad(75)),
				springArm.Rotation.y, springArm.Rotation.z);
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
		Vector3 inputVector = GetInputVector();
		Vector3 direction = GetDirection(inputVector);
		ApplyMovement(inputVector, direction, delta);
		ApplyFriction(direction, delta);
		ApplyGravity(delta);
		UpdateSnapVector();
		Jump();
		//RotateInventory();
		MoveSpringArm();
		AccelerationTilt(delta);

		//TODO: Make camera controllable with right stick & arrow keys
		//velocity = MoveAndSlide(velocity, snapVector, Vector3.Up, true);
	
		//TODO: Constantly rotate camera to face back of player / pivot back
		//camera rotation = lerp player pivot rotation,
		//blocked by mouse movement somehow? would still need to get mouse motion in process
	}

	///<summary>Per-frame operations. Not to be used for movement or other related things.</summary>
	public override void _Process(double delta)
	{
		Vector3 inputVector = GetInputVector();
		Vector3 direction = GetDirection(inputVector); //TODO: THESE SHOULD BE GLObAL - used in bothj process & physprocess!
		
		hologramCamera.GlobalTransform = hologramCameraPos.GlobalTransform;
		inventoryCamera.GlobalTransform = inventoryCameraPos.GlobalTransform;
		ApplyRotation(delta, direction); //separate from movement to remove annoying mouse rotation delay due to Godot input inconsistency
	}

	//<summary> foreach object under the  "inventory" Empty, add it to the list with all of the parameters found. -- get these "objects", by their "InventoryItem" attached! </summary>
	public void UpdateInventory()
	{
		foreach (Node node in inventoryNode.GetChildren())
		{
			if (node is InventoryItem nodeItem)
			{
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
		}
		
		//(Somewhere else) - only enable curitem, so that it's scripts will run
	}


	private Vector3 GetInputVector()
	{
		Vector3 inputVector = Vector3.Zero;
		inputVector.x = Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left");
		inputVector.z = Input.GetActionStrength("game_backward") - Input.GetActionStrength("game_forward");
		
		if (inputVector.Length() > 1)
			return inputVector.Normalized();
		else
			return inputVector;
	}

	private Vector3 GetDirection(Vector3 inputVector)
	{
		Vector3 direction = (inputVector.x * armY.Transform.basis.x) + (inputVector.z * armY.Transform.basis.z); 
		return direction;
	}

	private void ApplyMovement(Vector3 inputVector, Vector3 direction, double delta)
	{
		lastvelocity.x = velocity.x;
		lastvelocity.z = velocity.z;
		
		if (direction != Vector3.Zero)
		{
			velocity.x = Mathf.MoveToward(velocity.x, direction.x * MaxSpeed, (float) (Acceleration * delta));
			velocity.z = Mathf.MoveToward(velocity.z, direction.z * MaxSpeed, (float) (Acceleration * delta));
		}
	}
	
	private void ApplyRotation(double delta, Vector3 direction)
	{
		if (FirstPerson)
		{
			pivot.Rotation = new Vector3(
				pivot.Rotation.x,
				armY.Rotation.y,
				pivot.Rotation.z
			);
		}
		else
		{
			if (direction != Vector3.Zero)
			{
				pivot.Rotation = new Vector3(
					pivot.Rotation.x,
					Mathf.LerpAngle(pivot.Rotation.y, Mathf.Atan2(-velocity.x, -velocity.z), (float) (RotSpeed * delta)),
					pivot.Rotation.z
				);
			}
		}
	}

	private void ApplyFriction(Vector3 direction, double delta)
	{
		if (direction == Vector3.Zero)
		{
			if (IsOnFloor())
			{
				velocity.x = Mathf.MoveToward(velocity.x, 0, (float) (Friction * delta));
				velocity.y = Mathf.MoveToward(velocity.y, 0, (float) (Friction * delta));
				velocity.z = Mathf.MoveToward(velocity.z, 0, (float) (Friction * delta));
			}
			else
			{
				velocity.x = Mathf.MoveToward(velocity.x, 0, (float) (AirFriction * delta));
				velocity.y = Mathf.MoveToward(velocity.y, 0, (float) (AirFriction * delta));
				velocity.z = Mathf.MoveToward(velocity.z, 0, (float) (AirFriction * delta));
			}
		}
	}

	private void ApplyGravity(double delta)
	{
		velocity.y += (float) (Gravity * delta);
		velocity.y = Mathf.Clamp(velocity.y, Gravity, JumpImpulse);
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
			velocity.y = JumpImpulse;
		}
		if (Input.IsActionJustReleased("game_jump") && velocity.y > JumpImpulse / 2)
			velocity.y = JumpImpulse / 2;
	}
/*
	private void RotateInventory()
	{
		inventoryNode.RotationDegrees = new Vector3(Mathf.Lerp(inventoryNode.RotationDegrees.x, springArm.RotationDegrees.x, 0.2f), inventoryNode.RotationDegrees.y, inventoryNode.RotationDegrees.z);
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
				switch (Inventory[InventoryCurrent].ItemType)
				{
					case (int) ItemTypes.Weapon:
						springArm.Position = new Vector3(Mathf.Lerp(springArm.Position.x, 0, 0.1f), Mathf.Lerp(springArm.Position.y, 2, 0.1f), 0);
						break;
					case (int) ItemTypes.MeleeWeapon:
						springArm.Position = new Vector3(Mathf.Lerp(springArm.Position.x, 1, 0.1f), Mathf.Lerp(springArm.Position.y, 1.5f, 0.1f), 0);
						break;
					default:
						springArm.Position = new Vector3(Mathf.Lerp(springArm.Position.x, 0, 0.1f), Mathf.Lerp(springArm.Position.y, 1.5f, 0.1f), 0);
						break;
				}
			}
			else
				springArm.Position = new Vector3(Mathf.Lerp(springArm.Position.x, 0, 0.1f), Mathf.Lerp(springArm.Position.y, 1.5f, 0.1f), 0);
		}
	}

	private void DrawHologramViewport() =>
		hologramRect.Texture = hologramViewport.GetTexture();


	
	private void UpdateInventoryPanel()
	{
		UpdateInventory();

		foreach (Node node in inventoryPanel.GetChildren())
		{
			if (node is Button button)
			{
				inventoryPanelButtons.Add(button);
			
				if (inventoryPanelButtons.IndexOf(button) == InventoryCurrent)
					button.AddThemeColorOverride("font_color", new Color(0, 0, 1, 1));
				else
					button.AddThemeColorOverride("font_color", new Color(1, 1, 1, 0.5f));
				
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

	private void AccelerationTilt(double delta)
	{
		//i need directional velocity, not general, could be in applymovement?
		/*
		Vector3 Acceleration = new Vector3(
			(velocity.x - lastvelocity.x),
			0,
			(velocity.z - lastvelocity.z)
		) / delta;

		Vector3 directionalAcceleration = Acceleration + Rotation;*/
		
	}
}
