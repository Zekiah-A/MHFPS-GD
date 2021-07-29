using Godot;
using System;
using System.Collections.Generic;

public class KinematicPlayer : KinematicBody
{
	public const int maxSpeed = 10; //TODO: these are PUUBLIC and consts, why are they lower?

	public const int acceleration = 70;
	public const int friction = 60;
	public const int airFriction = 10;
	public const float gravity = -39.2f; //4x earth gravity.
	public const int jumpImpulse = 20;
	public const int rotSpeed = 25;
	public const int maxSpringLength = 8;
	public const int mouseSensitivity = 1;

	//get "Weapon class from the spatial" - all weapon logic will go into it's own class
	//[Export] public int currentSelected;
	public static int InventoryCurrent = 0;
	public static List<InventoryItem> Inventory = new List<InventoryItem>();

	private Vector3 velocity = Vector3.Zero;
	private Vector3 snapVector = Vector3.Zero;
	private SpringArm springArm;
	private Position3D pivot;
	private Spatial inventoryNode;
	private TextureRect hologramRect;
	private Viewport hologramViewport; 
	private Camera hologramCamera;
	private Position3D hologramCameraPos;
	private Panel inventoryPanel;
	private TextureRect inventoryRect;
	private Viewport inventoryViewport;
	private Camera inventoryCamera;
	private Position3D inventoryCameraPos;

	public override void _Ready()
	{
		Input.SetMouseMode(Input.MouseMode.Captured);
	
		springArm = GetNode<SpringArm>("SpringArm"); //find node as well
		pivot = GetNode<Position3D>("Pivot");
		inventoryNode = pivot.GetNode<Spatial>("Inventory");
		hologramRect = GetNode("PlayerHUD").GetNode<TextureRect>("TextureRect");
		hologramViewport = pivot.GetNode<Viewport>("HoloViewport");
		hologramCamera = hologramViewport.GetNode<Camera>("HoloCamera");
		hologramCameraPos = pivot.GetNode<Position3D>("HoloCamPosition");
		inventoryPanel = GetNode("PlayerHUD").GetNode<Panel>("InvPanel");
		inventoryRect = inventoryPanel.GetNode<TextureRect>("TextureRect");
		inventoryViewport = pivot.GetNode<Viewport>("InvViewport");
		inventoryCamera = inventoryViewport.GetNode<Camera>("InvCamera");
		inventoryCameraPos = pivot.GetNode<Position3D>("InvCamPosition");

		inventoryPanel.Visible = false;

		UpdateInventory();
		DrawHologramViewport();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("click"))
			Input.SetMouseMode(Input.MouseMode.Captured);
		
		if (@event.IsActionPressed("toggle_capture"))
		{
			if (Input.GetMouseMode() == Input.MouseMode.Captured)
				Input.SetMouseMode(Input.MouseMode.Visible);
			else
				Input.SetMouseMode(Input.MouseMode.Captured);
		}

		if (@event is InputEventMouseMotion eventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
		{
			RotateY(Mathf.Deg2Rad(-eventMouseMotion.Relative.x * mouseSensitivity));
			springArm.RotateX(Mathf.Deg2Rad(-eventMouseMotion.Relative.y * mouseSensitivity));
			springArm.Rotation = new Vector3(Mathf.Clamp(springArm.Rotation.x, Mathf.Deg2Rad(-75), Mathf.Deg2Rad(75)),
				springArm.Rotation.y, springArm.Rotation.z);
		}
		
		if (@event.IsActionPressed("game_zoomin"))
		{
			if (springArm.SpringLength > 0)
				springArm.SpringLength -= 1;
		}
		else if (@event.IsActionPressed("game_zoomout"))
		{
			if (springArm.SpringLength < maxSpringLength)
				springArm.SpringLength += 1;
		}

		if (@event.IsActionPressed("game_radial"))
		{
			GD.Print("Open radial inventory here!");
			//Hack: For testing now
			//InventoryCurrent += 1;
			UpdateInventory();
			OpenInventoryPanel();
		}
		
		if (@event is InputEventKey eventKey && eventKey.Pressed)
		{	//eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape
			//update HUD as well
			switch (eventKey.Scancode)
			{
				case (int) KeyList.Key1:
					InventoryCurrent = 0;
					UpdateInventory();
					OpenInventoryPanel();
					OpenInventoryPanel();
					break;
				case (int) KeyList.Key2:
					InventoryCurrent = 1; //maybe use maths (int  keylist)
					UpdateInventory();
					OpenInventoryPanel();
					OpenInventoryPanel();
					break;
				case (int) KeyList.Key3:
					InventoryCurrent = 2;
					UpdateInventory();
					OpenInventoryPanel();
					OpenInventoryPanel();
					break;
				case (int) KeyList.Key4:
					InventoryCurrent = 3;
					UpdateInventory();
					OpenInventoryPanel();
					OpenInventoryPanel();
					break;
				case (int) KeyList.Key5:
					InventoryCurrent = 4;
					UpdateInventory();
					OpenInventoryPanel();
					OpenInventoryPanel();
					break;
			}
		}
		//--------------------------------------------------------------WHAT IF UpdateInvHud just called upd inv?
		//Make a switch, no break so these happen anyway or something lol
		if (@event.IsActionPressed("ui_bumperl"))
		{
			InventoryCurrent -= 1;
			if (InventoryCurrent < 0) //fix later
				InventoryCurrent = 4;
			UpdateInventory();
			OpenInventoryPanel();
			OpenInventoryPanel();
		}
		else if (@event.IsActionPressed("ui_bumperr"))
		{
			InventoryCurrent += 1;
			if (InventoryCurrent > 4) //fix later
				InventoryCurrent = 0;
			UpdateInventory();
			OpenInventoryPanel();
			OpenInventoryPanel();
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector3 inputVector = GetInputVector();
		Vector3 direction = GetDirection(inputVector);
		ApplyMovement(inputVector, direction, delta);
		ApplyFriction(direction, delta);
		ApplyGravity(delta);
		UpdateSnapVector();
		Jump();
		//TODO: Make camera controllable with right stick & arrow keys
		velocity = MoveAndSlideWithSnap(velocity, snapVector, Vector3.Up, true);
	}
	///<summary>Per-frame operations. Not to be used for movement or other related things.</summary>
	public override void _Process(float delta)
	{
		hologramCamera.GlobalTransform = hologramCameraPos.GlobalTransform;
		inventoryCamera.GlobalTransform = inventoryCameraPos.GlobalTransform;	
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
		var direction = (inputVector.x * Transform.basis.x) + (inputVector.z * Transform.basis.z);
		return direction;
	}

	private void ApplyMovement(Vector3 inputVector, Vector3 direction, float delta)
	{
		if (direction != Vector3.Zero)
		{
			velocity.x = Mathf.MoveToward(velocity.x, direction.x * maxSpeed, acceleration * delta);
			velocity.z = Mathf.MoveToward(velocity.z, direction.z * maxSpeed, acceleration * delta);
			
			pivot.Rotation = new Vector3(
				pivot.Rotation.x,
				Mathf.LerpAngle(pivot.Rotation.y, Mathf.Atan2(-inputVector.x, -inputVector.z), rotSpeed * delta),
				pivot.Rotation.z
			);
		}
	}

	private void ApplyFriction(Vector3 direction, float delta)
	{
		if (direction == Vector3.Zero)
		{
			if (IsOnFloor())
			{
				velocity.x = Mathf.MoveToward(velocity.x, 0, friction * delta);
				velocity.y = Mathf.MoveToward(velocity.y, 0, friction * delta);
				velocity.z = Mathf.MoveToward(velocity.z, 0, friction * delta);
			}
			else
			{
				velocity.x = Mathf.MoveToward(velocity.x, 0, airFriction * delta);
				velocity.y = Mathf.MoveToward(velocity.y, 0, airFriction * delta);
				velocity.z = Mathf.MoveToward(velocity.z, 0, airFriction * delta);
			}
		}
	}

	private void ApplyGravity(float delta)
	{
		velocity.y += gravity * delta;
		velocity.y = Mathf.Clamp(velocity.y, gravity, jumpImpulse);
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
			velocity.y = jumpImpulse;
		}
		if (Input.IsActionJustReleased("game_jump") && velocity.y > jumpImpulse / 2)
			velocity.y = jumpImpulse / 2;
	}

	//TODO: Cleanup
	private void DrawHologramViewport()
	{
		hologramRect.Texture = (Texture) hologramViewport.GetTexture();
		//hologramCamera.GlobalTransform = hologramCameraPos.GlobalTransform;
	}

	List<Button> inventoryPanelButtons = new List<Button>();
	private void OpenInventoryPanel()
	{
		if (!inventoryPanel.Visible)
		{
			inventoryPanel.Visible = true;
			hologramRect.Visible = false;
			inventoryRect.Texture = (Texture) inventoryViewport.GetTexture();
			Input.SetMouseMode(Input.MouseMode.Visible);
			//inventoryCamera.GlobalTransform = inventoryCameraPos.GlobalTransform;
			
			foreach (Node node in inventoryPanel.GetChildren())
			{
				if (node is Button button)
				{
					inventoryPanelButtons.Add(button);
				
					if (inventoryPanelButtons.IndexOf(button) == InventoryCurrent)
						button.AddColorOverride("font_color", new Color(0, 1, 0, 1));
					else
						button.AddColorOverride("font_color", new Color(1, 1, 1, 1));
					
					try
					{
						button.Text = $"[{inventoryPanelButtons.IndexOf(button) + 1}] {Inventory[inventoryPanelButtons.IndexOf(button)].Name}";
					}
					catch
					{
						button.Text = $"[{inventoryPanelButtons.IndexOf(button) + 1}] ____________";
					}
				}
			}
		}
		else
		{
			inventoryPanel.Visible = false;
			hologramRect.Visible = true;
			inventoryRect.Texture = null; //clear
			Input.SetMouseMode(Input.MouseMode.Captured);
		}
	}
	
	private void InventoryButtonPressed(int index)
	{
		InventoryCurrent = index;
		UpdateInventory();
		//HACK: BODGE: Need to add to update inv or something
		OpenInventoryPanel();
		OpenInventoryPanel();
	}
}
