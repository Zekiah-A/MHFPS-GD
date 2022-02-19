using Godot;

public class Phone : Spatial
{
	private Transform defaultTransform;
	
	private Spatial doors;
	private Spatial cameraBody;
	private Tween phoneTween;
	private Area[] doorButtons;
	private Texture hoverCursor;
	private Environment worldEnvironment;
	private Button[] doorUiButtons;

	public override void _Ready()
	{
		hoverCursor = ResourceLoader.Load<Texture>("res://Assets/GameAssets/Textures/aim_hover.png");
		doors = GetTree().CurrentScene.GetNode<Spatial>("Doors");
		cameraBody = GetTree().CurrentScene.GetNode<Spatial>("CameraBody");
		phoneTween = GetNode<Tween>("Tween");
		worldEnvironment = GetTree().CurrentScene.GetNode<WorldEnvironment>("WorldEnvironment").Environment;
		doorButtons = new []
		{
			GetNode<Area>("LeftDoorButton"),
			GetNode<Area>("RightDoorButton"),
			GetNode<Area>("OpenButton"),
		};

		doorUiButtons = new[]
		{
			GetNode("Viewport").GetNode("PhoneUI").GetNode("ContentPanel").GetNode<Button>("LeftButton"),
			GetNode("Viewport").GetNode("PhoneUI").GetNode("ContentPanel").GetNode<Button>("RightButton"),
		};
		
		doorButtons[0].InputRayPickable = false;
		doorButtons[1].InputRayPickable = false;
		doorButtons[2].InputRayPickable = true;
		defaultTransform = Transform;
		worldEnvironment.Set("dof_blur_far_enabled", false);
	}
	
	public void Activate()
	{
		//TODO: Tween, bring phone to face.
		RotationDegrees = new Vector3(0, 90, 90);
		Translation = new Vector3(cameraBody.Translation.x, cameraBody.Translation.y, cameraBody.Translation.z - 0.8f);
		doorButtons[0].InputRayPickable = true;
		doorButtons[1].InputRayPickable = true;
		doorButtons[2].InputRayPickable = false;
		worldEnvironment.Set("dof_blur_far_enabled", true);
	}

	public void Deactivate()
	{
		//TODO: Make this tween
		doorButtons[0].InputRayPickable = false;
		doorButtons[1].InputRayPickable = false;
		doorButtons[2].InputRayPickable = true;
		Transform = defaultTransform;
		worldEnvironment.Set("dof_blur_far_enabled", false);
	}

	public void Disable()
	{
		doorButtons[0].EmitSignal("mouse_exit");
		doorButtons[0].QueueFree();
		doorButtons[1].QueueFree();
		doorButtons[2].QueueFree();
		doorUiButtons[0].SetPressedNoSignal(false);
		doorUiButtons[1].SetPressedNoSignal(false);
		Transform = defaultTransform;
		worldEnvironment.Set("dof_blur_far_enabled", false);
	}

	//TODO: GUI colours for opened/closed, fake button "press" effect when clicked, etc (use "IsPressed" on the button, and make a style for pressed / unpressed.)
	//TODO: Vent should not be here, and should only be affected by hovering the mouse over it in doors, as it is light based.
	
	//TODO: Mouse state nees to reset once mouse is off of these.
	private void OnLeftDoorClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int) ButtonList.Left && mouseButton.Pressed)
			{
				if (((Doors) doors).LeftOpened)
				{
					((Doors) doors)?.CloseLeft();
					doorUiButtons[0].SetPressedNoSignal(true);
				}
				else
				{
					((Doors) doors)?.OpenLeft();
					doorUiButtons[0].SetPressedNoSignal(false);
				}
			}
		}
		
		//If is hovering, change cursor
		if (@event is InputEventMouse mouse)
		{
			Input.SetCustomMouseCursor(hoverCursor);
		}
	}

	private void OnRightDoorClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int) ButtonList.Left && mouseButton.Pressed)
			{
				if (((Doors) doors).RightOpened)
				{
					((Doors) doors)?.CloseRight();
					doorUiButtons[1].SetPressedNoSignal(true);
				}
				else
				{
					((Doors) doors)?.OpenRight();
					doorUiButtons[1].SetPressedNoSignal(false);
				}
			}
		}
		
		//If is hovering, change cursor
		if (@event is InputEventMouse mouse)
		{
			Input.SetCustomMouseCursor(hoverCursor);
		}
	}

	private void OnBackgroundClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx) //TODO: Only enable this area when the phone is opened.
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int) ButtonList.Left && mouseButton.Pressed)
			{
				Deactivate();
			}
		}
	}
	
	//NOTE: Make sure to enable Resource > Local to scene under the viewport material for the phone screen, or else it will not work.
}
