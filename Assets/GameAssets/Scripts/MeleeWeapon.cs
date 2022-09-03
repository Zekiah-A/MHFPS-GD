using Godot;
using System;

public class MeleeWeapon : Spatial
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
			mouseRelativeMovement = -eventMouseMotion.Relative.x;
	}

	public override void _PhysicsProcess(float delta)
	{
		SwayWeapon(delta);
	}
	
	public virtual void SwayWeapon(float delta)
	{
		if (mouseRelativeMovement > swayThreshold)
			Rotation = new Vector3(0, 0, Mathf.Lerp(Rotation.z, swayLeft, swayStrength * delta));
		else if (mouseRelativeMovement < -swayThreshold)
			Rotation = new Vector3(0, 0, Mathf.Lerp(Rotation.z, swayRight, swayStrength * delta));
		else
			Rotation = new Vector3(0, 0, Mathf.Lerp(Rotation.z, 0, swayStrength * delta));
	}
}
