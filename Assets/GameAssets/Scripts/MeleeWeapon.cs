using Godot;
using System;

public partial class MeleeWeapon : Node3D
{
	private float swayThreshold = 4; //How much a mouse must be pushed to trigger.
	private readonly float swayLeft = 0.2f;
	private readonly float swayRight = -0.2f;
	private readonly float swayStrength = 5;
	
	private float mouseRelativeMovement = 0;
	
	public override void _Ready()
	{
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		//if (@event.IsActionPressed("game_fire") && GetParent<InventoryItem>().Enabled)
			//Fire();
		//if (@event.IsActionPressed("game_reload") && GetParent<InventoryItem>().Enabled)
			//Reload();
		if (@event is InputEventMouseMotion eventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
			mouseRelativeMovement = -eventMouseMotion.Relative.X;
	}

	public override void _PhysicsProcess(double delta)
	{
		SwayWeapon(delta);
	}

	protected virtual void SwayWeapon(double delta)
	{
		if (mouseRelativeMovement > swayThreshold)
			Rotation = new Vector3(0, 0, Mathf.Lerp(Rotation.Z, swayLeft, (float) (swayStrength * delta)));
		else if (mouseRelativeMovement < -swayThreshold)
			Rotation = new Vector3(0, 0, Mathf.Lerp(Rotation.Z, swayRight, (float) (swayStrength * delta)));
		else
			Rotation = new Vector3(0, 0, Mathf.Lerp(Rotation.Z, 0, (float) (swayStrength * delta)));
	}
}
