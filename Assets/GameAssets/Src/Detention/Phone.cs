using System;
using System.Linq;
using Godot;

public partial class Phone : Node3D
{
	private Node3D cameraBody;
	public SubViewport SubViewport;
	private Transform3D defaultTransform;

	private Node3D doors;
	private Texture2D hoverCursor;
	private Texture2D dotCursor;

	private WorldEnvironment worldEnvironment;
	
	public bool Active;
	public bool MouseIsOver;
	
	public override void _Ready()
	{
		SubViewport = GetNode<SubViewport>("SubViewport");
		hoverCursor = ResourceLoader.Load<Texture2D>("res://Assets/GameAssets/Textures/aim_hover.png");
		dotCursor = ResourceLoader.Load<Texture2D>("res://Assets/GameAssets/Textures/aim_reticle.png");
		doors = GetTree().CurrentScene.GetNode<Node3D>("Doors");
		cameraBody = GetTree().CurrentScene.GetNode<Node3D>("CameraBody");
		worldEnvironment = GetTree().CurrentScene.GetNode<WorldEnvironment>("WorldEnvironment");
		
		defaultTransform = Transform;
		worldEnvironment.CameraAttributes.Set("dof_blur_far_enabled", false);
	}
	
	private void OnDoorLeftButtonPressed()
	{
		GD.Print("I like");
	}


	private void OnDoorRightButtonPressed()
	{
		GD.Print("I sike");
	}

	public void Activate()
	{
		Active = true;
		Rotation = new Vector3(0, Mathf.Pi / 2, Mathf.Pi / 2);
		Position = new Vector3(cameraBody.Position.X, cameraBody.Position.Y, cameraBody.Position.Z - 0.8f);
		worldEnvironment.CameraAttributes.Set("dof_blur_far_enabled", false);
	}

	public void Deactivate()
	{
		Active = false;
		Transform = defaultTransform;
		worldEnvironment.CameraAttributes.Set("dof_blur_far_enabled", false);
	}

	public void Disable()
	{
		Transform = defaultTransform;
		worldEnvironment.CameraAttributes.Set("dof_blur_far_enabled", false);
	}
}
