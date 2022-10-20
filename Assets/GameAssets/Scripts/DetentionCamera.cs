using Godot;
using System;

public partial class DetentionCamera : Node3D
{
	const float MouseSensitivity = 0.04f; //TODO: Make values like this customisable.
	private Camera3D camera;
	private SpotLight3D torch;
	
	public override void _Ready()
	{
		camera = GetNode<Camera3D>("Camera3D");
		torch = GetNode<SpotLight3D>("Torch");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			camera.RotateY(Mathf.DegToRad(-eventMouseMotion.Relative.x * MouseSensitivity));
			camera.Rotation = new Vector3(0, Mathf.Clamp(camera.Rotation.y, Mathf.DegToRad(-45), Mathf.DegToRad(45)), 0);
			var rayNormal = camera.ProjectPosition(GetViewport().GetMousePosition(), 1);
			torch.LookAt(rayNormal, Vector3.Up);
		}
	}

	public void BindRotate(Node node)
	{
		
	}
	
	public void UnbindRotate(Node node)
	{
		
	}
}
