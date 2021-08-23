using Godot;
using System;

public class Pathfinder : KinematicBody
{
	public float jumpImpulse = 30f;

	private const float speed = 10f;
	private const float rotationSpeed = 5f;
	public const float gravity = -39.2f;
	private const float maxFallSpeed = 100f;

	//private Vector3 velocity = Vector3.Zero;
	private Vector3 direction = Vector3.Zero;
	private Vector3[] path = null;
	private int pathNode = 0;
	
	private Navigation navigation;
	private Spatial player; //for testing
	private RayCast groundCast;
	
	public override void _Ready()
	{
		navigation = GetParent<Navigation>();
		player = GetParent().GetParent().GetNode<Spatial>("Player"); //for testing
		groundCast = GetNode<RayCast>("GroundCast");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (path != null && pathNode < path.Length)
		{
			direction = path[pathNode] - GlobalTransform.origin;

			if (direction.Length() < 1)
				pathNode++;
		}
		
		Rotation = new Vector3(
				Rotation.x,
				Mathf.LerpAngle(Rotation.y, Mathf.Atan2(-direction.x, -direction.z), rotationSpeed * delta),
				Rotation.z
		);
		
		//MoveAndSlide(-GlobalTransform.basis.z * speed, Vector3.Up); //still testing
		
		if (groundCast.IsColliding())
			direction = MoveAndSlide(direction.Normalized() * speed, Vector3.Up);
		else
		{
			//Vector3 directionWithGravity = new Vector3(direction.x, 0, direction.z); //TODO: currently must be 0, no y velocity calculated yet until jumping is implemented
			//direction = MoveAndSlide(directionWithGravity.Normalized() * speed, Vector3.Up); //HACK: Don't normalise here			
		}

	}

	public void NavigationTick()
	{
		MoveTo(player.GlobalTransform.origin);
	}

	public void MoveTo(Vector3 targetPosition)
	{
		path = navigation.GetSimplePath(GlobalTransform.origin, targetPosition); //TODO: should not calculate if it generated path is longer than current distance (e.g on wall)?
		pathNode = 0;
	}
	
	private Vector3 ApplyGravity(float delta, float movementY)
	{
		movementY += gravity * delta;
		movementY = Mathf.Clamp(movementY, gravity, jumpImpulse);
		return new Vector3(0, movementY, 0);
		//direction.y += gravity * delta;
		//direction.y = Mathf.Clamp(direction.y, gravity, jumpImpulse);
	}

	private void OnNavigationTimerTimeout() =>
		NavigationTick();
}
