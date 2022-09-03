using Godot;
using System;

public class Player : RigidBody
{
	public float Health = 100f;

	private const float MouseSensitivity = 0.3f;
	private const float MovementSpeed = 100f;
	private const float MaxSpeed = 10f;

	private RayCast groundSensor;
	private Spatial cameraPivot;
	private float yaw = 0f;
	private float pitch = 0f;
	private Vector3 viewDirection = Vector3.Zero;
	private Spatial model;
	
	public override void _Ready()
	{
		groundSensor = GetNode<RayCast>("GroundSensor");
		cameraPivot = GetNode<Spatial>("CameraPivot");
		model = GetNode<Spatial>("Model");
		viewDirection = cameraPivot.GlobalTransform.basis.z;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouse)
		{
			yaw = (yaw - mouse.Relative.x * MouseSensitivity) % 360; //Fmod
			pitch = Math.Max(Math.Min(pitch - mouse.Relative.y * MouseSensitivity, 70), -50);
			cameraPivot.RotationDegrees = new Vector3(pitch, yaw, 0);
		}
	}

	//change to regular _Process?
	public override void _PhysicsProcess(float delta)
	{
		if (LinearVelocity.Length() < MaxSpeed)
		{
			if (Input.IsActionPressed("game_forward"))
			{
				//move the player forward with LinearVelocity(0, 0, MovementSpeed), in the direction of the _cameraPivot's forward vector
				LinearVelocity -= cameraPivot.GlobalTransform.basis.z * MovementSpeed * delta;
			}
			if (Input.IsActionPressed("game_backward"))
			{
				//move the player backward with LinearVelocity(0, 0, -MovementSpeed), in the direction of the _cameraPivot's forward vector
				LinearVelocity += cameraPivot.GlobalTransform.basis.z * MovementSpeed * delta;
			}
			if (Input.IsActionPressed("game_left"))
			{
				//move the player left with LinearVelocity(0, MovementSpeed, 0), in the direction of the _cameraPivot's right vector
				LinearVelocity -= cameraPivot.GlobalTransform.basis.x * MovementSpeed * delta;
			}
			if (Input.IsActionPressed("game_right"))
			{
				//move the player right with LinearVelocity(0, -MovementSpeed, 0), in the direction of the _cameraPivot's right vector
				LinearVelocity += cameraPivot.GlobalTransform.basis.x * MovementSpeed * delta;
			}
			
			//make the player jump if they are on the ground and the jump key is pressed
			if (Input.IsActionJustPressed("game_jump") && groundSensor.IsColliding())
			{
				LinearVelocity += cameraPivot.GlobalTransform.basis.y * 10 * delta;
			}
		}

		// if game_forward, game_backward is released, stop moving
		if (Input.IsActionJustReleased("game_forward") || Input.IsActionJustReleased("game_backward"))
		{
			LinearVelocity -= LinearVelocity / 2;
		}
		// if game_left or game_right is released, stop moving
		if (Input.IsActionJustReleased("game_left") || Input.IsActionJustReleased("game_right"))
		{
			LinearVelocity -= LinearVelocity / 2;
		}
		
		if (Input.IsActionPressed("game_any"))
		{
			model.Rotation = new Vector3(this.Rotation.x, Mathf.Lerp(model.Rotation.y, cameraPivot.Rotation.y - 90, 0.1f), this.Rotation.z);
		}
	} 	
}
