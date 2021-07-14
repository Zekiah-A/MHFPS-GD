using Godot;
using System;

public class Player : RigidBody
{
	public float Health = 100f;

	private const float MouseSensitivity = 0.3f;
	private const float MovementSpeed = 40f;
	private const float MaxSpeed = 10;

	private RayCast _groundSensor;
	private Spatial _cameraPivot;
	private float _yaw = 0f;
	private float _pitch = 0f;
	private Vector3 _viewDirection = Vector3.Zero;
	private Spatial _model;
	
	public override void _Ready()
	{
		_groundSensor = GetNode<RayCast>("GroundSensor");
		_cameraPivot = GetNode<Spatial>("CameraPivot");
		_model = GetNode<Spatial>("Model");
		_viewDirection = _cameraPivot.GlobalTransform.basis.z;
	}

	public override void _Input(InputEvent @_event)
	{
		if (@_event is InputEventMouseMotion _mouse)
		{
			_yaw = (_yaw - _mouse.Relative.x * MouseSensitivity) % 360; //Fmod
			_pitch = Math.Max(Math.Min(_pitch - _mouse.Relative.y * MouseSensitivity, 70), -50);
			_cameraPivot.RotationDegrees = new Vector3(_pitch, _yaw, 0);
		}
	}

	public override void _Process(float _delta)
	{
		if (GetLinearVelocity().Length() < MaxSpeed)
		{
			if (Input.IsActionPressed("game_forward"))
			{
				//move the player forward with LinearVelocity(0, 0, MovementSpeed), in the direction of the _cameraPivot's forward vector
				LinearVelocity -= _cameraPivot.GlobalTransform.basis.z * MovementSpeed * _delta;
				_model.Rotation = new Vector3(this.Rotation.x, _cameraPivot.Rotation.y, this.Rotation.z);
			}
			if (Input.IsActionPressed("game_backward"))
			{
				//move the player backward with LinearVelocity(0, 0, -MovementSpeed), in the direction of the _cameraPivot's forward vector
				LinearVelocity += _cameraPivot.GlobalTransform.basis.z * MovementSpeed * _delta;
				_model.Rotation = new Vector3(this.Rotation.x, _cameraPivot.Rotation.y, this.Rotation.z);
			}
			if (Input.IsActionPressed("game_left"))
			{
				//move the player left with LinearVelocity(0, MovementSpeed, 0), in the direction of the _cameraPivot's right vector
				LinearVelocity -= _cameraPivot.GlobalTransform.basis.x * MovementSpeed * _delta;
				_model.Rotation = new Vector3(this.Rotation.x, _cameraPivot.Rotation.y, this.Rotation.z);
			}
			if (Input.IsActionPressed("game_right"))
			{
				//move the player right with LinearVelocity(0, -MovementSpeed, 0), in the direction of the _cameraPivot's right vector
				LinearVelocity += _cameraPivot.GlobalTransform.basis.x * MovementSpeed * _delta;
				_model.Rotation = new Vector3(this.Rotation.x, _cameraPivot.Rotation.y, this.Rotation.z);
			}
			//make the player jump if they are on the ground and the jump key is pressed
			if (Input.IsActionJustPressed("game_jump") && _groundSensor.IsColliding())
			{
				LinearVelocity += _cameraPivot.GlobalTransform.basis.y * 10 * _delta;
			}
		}

		// if game_forward, game_backward is released, stop moving
		if (Input.IsActionJustReleased("game_forward") || Input.IsActionJustReleased("game_backward"))
		{
			LinearVelocity = Vector3.Zero;
		}
		// if game_left or game_right is released, stop moving
		if (Input.IsActionJustReleased("game_left") || Input.IsActionJustReleased("game_right"))
		{
			LinearVelocity = Vector3.Zero;
		}
	} 	
}
