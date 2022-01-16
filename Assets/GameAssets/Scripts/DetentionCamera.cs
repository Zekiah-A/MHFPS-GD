using Godot;
using System;

public class DetentionCamera : Spatial
{
	public float MouseSensitivity = 0.04f; //TODO: Make values like this customisable.
	private Camera camera;
	private SpotLight torch;
	
	public override void _Ready()
	{
		camera = GetNode<Camera>("Camera");
		torch = GetNode<SpotLight>("Torch");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			camera.RotateY(Mathf.Deg2Rad(-eventMouseMotion.Relative.x * MouseSensitivity));
			camera.Rotation = new Vector3(0, Mathf.Clamp(camera.Rotation.y, Mathf.Deg2Rad(-45), Mathf.Deg2Rad(45)), 0);
			var rayNormal = camera.ProjectPosition(GetViewport().GetMousePosition(), 1);
			torch.LookAt(rayNormal, Vector3.Up);
		}
	}

}
