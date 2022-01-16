using Godot;
using System;

public class DetentionCamera : Spatial
{
	public float MouseSensitivity = 0.5f;
	private Camera camera;
	
	public override void _Ready()
	{
		camera = GetNode<Camera>("Camera");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
		{
			camera.RotateY(Mathf.Deg2Rad(-eventMouseMotion.Relative.x * MouseSensitivity));
			camera.Rotation = new Vector3(0, Mathf.Clamp(camera.Rotation.y, Mathf.Deg2Rad(-45), Mathf.Deg2Rad(45)), 0);
		}
	}

}
