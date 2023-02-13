using Godot;

public partial class Phone : Node3D
{
    private Node3D cameraBody;
    private Transform3D defaultTransform;
    private Area3D[] doorButtons;

    private Node3D doors;
    private Button[] doorUiButtons;
    private Texture2D hoverCursor;
    private Tween phoneTween;
    private WorldEnvironment worldEnvironment;
    
    public override void _Ready()
    {
        hoverCursor = ResourceLoader.Load<Texture2D>("res://Assets/GameAssets/Textures/aim_hover.png");
        doors = GetTree().CurrentScene.GetNode<Node3D>("Doors");
        cameraBody = GetTree().CurrentScene.GetNode<Node3D>("CameraBody");
        phoneTween = GetNode<Tween>("Tween");
        worldEnvironment = GetTree().CurrentScene.GetNode<WorldEnvironment>("WorldEnvironment");
        doorButtons = new[]
        {
            GetNode<Area3D>("LeftDoorButton"),
            GetNode<Area3D>("RightDoorButton"),
            GetNode<Area3D>("OpenButton")
        };

        doorUiButtons = new[]
        {
            GetNode("SubViewport").GetNode("PhoneUI").GetNode("ContentPanel").GetNode<Button>("LeftButton"),
            GetNode("SubViewport").GetNode("PhoneUI").GetNode("ContentPanel").GetNode<Button>("RightButton")
        };

        doorButtons[0].InputRayPickable = false;
        doorButtons[1].InputRayPickable = false;
        doorButtons[2].InputRayPickable = true;
        defaultTransform = Transform;
        worldEnvironment.CameraAttributes.Set("dof_blur_far_enabled", false);
    }

    public void Activate()
    {
        Rotation = new Vector3(0, Mathf.Pi / 4, Mathf.Pi / 4);
        Position = new Vector3(cameraBody.Position.X, cameraBody.Position.Y, cameraBody.Position.Z - 0.8f);
        doorButtons[0].InputRayPickable = true;
        doorButtons[1].InputRayPickable = true;
        doorButtons[2].InputRayPickable = false;
        worldEnvironment.CameraAttributes.Set("dof_blur_far_enabled", false);
    }

    public void Deactivate()
    {
        //TODO: Make this tween
        doorButtons[0].InputRayPickable = false;
        doorButtons[1].InputRayPickable = false;
        doorButtons[2].InputRayPickable = true;
        Transform = defaultTransform;
        worldEnvironment.CameraAttributes.Set("dof_blur_far_enabled", false);
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
        worldEnvironment.CameraAttributes.Set("dof_blur_far_enabled", false);
    }

    //TODO: GUI colours for opened/closed, fake button "press" effect when clicked, etc (use "IsPressed" on the button, and make a style for pressed / unpressed.)
    //TODO: Vent should not be here, and should only be affected by hovering the mouse over it in doors, as it is light based.

    //TODO: Mouse state needs to reset once mouse is off of these.
    private void OnLeftDoorClicked(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
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

        //If is hovering, change cursor
        if (@event is InputEventMouse mouse)
        {
            Input.SetCustomMouseCursor(hoverCursor);
        }
    }

    private void OnRightDoorClicked(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
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

        //If is hovering, change cursor
        if (@event is InputEventMouse mouse)
        {
            Input.SetCustomMouseCursor(hoverCursor);
        }
    }

    //TODO: Only enable this area when the phone is opened.
    private void OnBackgroundClicked(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIndex)
    {
        if (@event is not InputEventMouseButton mouseButton)
        {
            return;
        }

        if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
        {
            Deactivate();
        }
    }

    //NOTE: Make sure to enable Resource > Local to scene under the viewport material for the phone screen, or else it will not work.
}