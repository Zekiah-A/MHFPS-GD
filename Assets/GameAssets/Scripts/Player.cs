using Godot;
using System;

public class Player : RigidBody
{
	public float Health = 100f;

	private const float MouseSensitivity = 0.3f;
	private const float MovementSpeed = 5f;

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
		if (Input.IsKeyPressed((int) KeyList.W))
		{
			//move the player forward with LinearVelocity(0, 0, MovementSpeed), in the direction of the _cameraPivot's forward vector
			LinearVelocity += _cameraPivot.GlobalTransform.basis.z * MovementSpeed * _delta;
		}
		if (Input.IsKeyPressed((int) KeyList.S))
		{
			//move the player backward with LinearVelocity(0, 0, -MovementSpeed), in the direction of the _cameraPivot's forward vector
			LinearVelocity -= _cameraPivot.GlobalTransform.basis.z * MovementSpeed * _delta;
		}
		if (Input.IsKeyPressed((int) KeyList.A))
		{
			//move the player left with LinearVelocity(0, MovementSpeed, 0), in the direction of the _cameraPivot's right vector
			LinearVelocity -= _cameraPivot.GlobalTransform.basis.x * MovementSpeed * _delta;
		}
		if (Input.IsKeyPressed((int) KeyList.D))
		{
			//move the player right with LinearVelocity(0, -MovementSpeed, 0), in the direction of the _cameraPivot's right vector
			LinearVelocity += _cameraPivot.GlobalTransform.basis.x * MovementSpeed * _delta;
		}
		//make the player jump if they are on the ground and the jump key is pressed
		if (Input.IsKeyPressed((int) KeyList.Space) && _groundSensor.IsColliding())
		{
			LinearVelocity += _cameraPivot.GlobalTransform.basis.y * MovementSpeed * 10 * _delta;
		}
		
		_model.Rotation = new Vector3(this.Rotation.x, _cameraPivot.Rotation.y, this.Rotation.z);
	} 	
}
