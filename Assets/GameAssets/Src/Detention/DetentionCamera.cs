using System;
using Godot;

public partial class DetentionCamera : Node3D
{
    private const float MouseSensitivity = 0.4f;
    private Camera3D camera;
    private SpotLight3D torch;

    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Camera3D");
        torch = GetNode<SpotLight3D>("Torch");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseMotion eventMouseMotion)
        {
            return;
        }
        
        camera.Rotation = new Vector3
        (
            camera.Rotation.X,
            Mathf.Clamp
            (
                (1 - eventMouseMotion.Position.X / GetViewport().GetVisibleRect().Size.X) * Mathf.Pi / 2 - Mathf.Pi / 4,
                -Mathf.Pi / 4,
                Mathf.Pi / 4
            ),
            camera.Rotation.Z
        );
            
        var rayNormal = camera.ProjectPosition(GetViewport().GetMousePosition(), 1);
        torch.LookAt(rayNormal, Vector3.Up);
    }

    public void BindRotate(Node node)
    {
    }

    public void UnbindRotate(Node node)
    {
    }
}