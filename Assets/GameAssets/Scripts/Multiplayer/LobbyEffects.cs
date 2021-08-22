using Godot;
using System;

public class LobbyEffects : Spatial
{
	private Camera camera;
	[Export] public float Damping = 10f;
	[Export] public float LerpStrength = 0.1f;

	public override void _Ready() => camera = GetNode<Camera>("Camera");

	public override void _Process(float delta) //input tween
	{
		Vector2 mousePosition = GetViewport().GetMousePosition();
		float posx = (mousePosition.x  - GetViewport().GetVisibleRect().Size.x / 2) / GetViewport().GetVisibleRect().Size.x;
		float posy = (mousePosition.y  - GetViewport().GetVisibleRect().Size.y / 2) / GetViewport().GetVisibleRect().Size.y;
		
		camera.Rotation = new Vector3 (
			Mathf.Lerp(camera.Rotation.x, -posy / Damping, LerpStrength),
			Mathf.Lerp(camera.Rotation.y, -posx / Damping, LerpStrength),
			camera.Rotation.z
		);
	}
}
