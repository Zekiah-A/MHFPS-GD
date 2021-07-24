using Godot;
using System;

public class KinematicPlayer : KinematicBody
{
	public const int maxSpeed = 10; //TODO: these are PUUBLIC and consts, why are they lower?

	public const int acceleration = 70;
	public const int friction = 60;
	public const int airFriction = 10;
	public const float gravity = -39.2f; //4x earth gravity.
	public const int jumpImpulse = 20;
	public const int rotSpeed = 25;
	[Export] public const int maxSpringLength = 8;
	[Export] public const int mouseSensitivity = 1;

	private int scrollDir;

	private Vector3 velocity = Vector3.Zero;
	private Vector3 snapVector = Vector3.Zero;

	private SpringArm springArm;
	private Position3D pivot;


	public override void _Ready()
	{
		Input.SetMouseMode(Input.MouseMode.Captured);
	
		springArm = GetNode<SpringArm>("SpringArm"); //find node as well
		pivot = GetNode<Position3D>("Pivot");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("click"))
			Input.SetMouseMode(Input.MouseMode.Captured);
		
		if (@event.IsActionPressed("toggle_capture"))
		{
			if (Input.GetMouseMode() == Input.MouseMode.Captured)
				Input.SetMouseMode(Input.MouseMode.Visible);
			else
				Input.SetMouseMode(Input.MouseMode.Captured);
		}

		if (@event is InputEventMouseMotion eventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
		{
			RotateY(Mathf.Deg2Rad(-eventMouseMotion.Relative.x * mouseSensitivity));
			springArm.RotateX(Mathf.Deg2Rad(-eventMouseMotion.Relative.y * mouseSensitivity));
			springArm.Rotation = new Vector3(Mathf.Clamp(springArm.Rotation.x, Mathf.Deg2Rad(-75), Mathf.Deg2Rad(75)),
				springArm.Rotation.y, springArm.Rotation.z);
		}
		
		if (@event.IsActionPressed("game_zoomin"))
		{
			if (springArm.SpringLength > 0)
				springArm.SpringLength -= 1;
		}
		else if (@event.IsActionPressed("game_zoomout"))
		{
			if (springArm.SpringLength < maxSpringLength)
				springArm.SpringLength += 1;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector3 inputVector = GetInputVector();
		Vector3 direction = GetDirection(inputVector);
		ApplyMovement(inputVector, direction, delta);
		ApplyFriction(direction, delta);
		ApplyGravity(delta);
		UpdateSnapVector();
		Jump();
		velocity = MoveAndSlideWithSnap(velocity, snapVector, Vector3.Up, true);
	}


	private Vector3 GetInputVector()
	{
		Vector3 inputVector = Vector3.Zero;
		inputVector.x = Input.GetActionStrength("game_right") - Input.GetActionStrength("game_left");
		inputVector.z = Input.GetActionStrength("game_backward") - Input.GetActionStrength("game_forward");
		
		if (inputVector.Length() > 1)
			return inputVector.Normalized();
		else
			return inputVector;
	}

	private Vector3 GetDirection(Vector3 inputVector)
	{
		var direction = (inputVector.x * Transform.basis.x) + (inputVector.z * Transform.basis.z);
		return direction;
	}

	private void ApplyMovement(Vector3 inputVector, Vector3 direction, float delta)
	{
		if (direction != Vector3.Zero)
		{
			velocity.x = Mathf.MoveToward(velocity.x, direction.x * maxSpeed, acceleration * delta);
			velocity.z = Mathf.MoveToward(velocity.z, direction.z * maxSpeed, acceleration * delta);
			
			pivot.Rotation = new Vector3(
				pivot.Rotation.x,
				Mathf.LerpAngle(pivot.Rotation.y, Mathf.Atan2(-inputVector.x, -inputVector.z), rotSpeed * delta),
				pivot.Rotation.z
			);
		}
	}

	private void ApplyFriction(Vector3 direction, float delta)
	{
		if (direction == Vector3.Zero)
		{
			if (IsOnFloor())
			{
				velocity.x = Mathf.MoveToward(velocity.x, 0, friction * delta);
				velocity.y = Mathf.MoveToward(velocity.y, 0, friction * delta);
				velocity.z = Mathf.MoveToward(velocity.z, 0, friction * delta);
			}
			else
			{
				velocity.x = Mathf.MoveToward(velocity.x, 0, airFriction * delta);
				velocity.y = Mathf.MoveToward(velocity.y, 0, airFriction * delta);
				velocity.z = Mathf.MoveToward(velocity.z, 0, airFriction * delta);
			}
		}
	}

	private void ApplyGravity(float delta)
	{
		velocity.y += gravity * delta;
		velocity.y = Mathf.Clamp(velocity.y, gravity, jumpImpulse);
	}

	public void UpdateSnapVector() //use fancy terneary thing
	{
		if (IsOnFloor())
			snapVector = -GetFloorNormal();
		else
			snapVector = Vector3.Down;
	}

	private void Jump()
	{
		if (Input.IsActionJustPressed("game_jump") && IsOnFloor())
		{
			snapVector = Vector3.Zero; //allows plr to jump
			velocity.y = jumpImpulse;
		}
		if (Input.IsActionJustReleased("game_jump") && velocity.y > jumpImpulse / 2)
			velocity.y = jumpImpulse / 2;
	}
}
