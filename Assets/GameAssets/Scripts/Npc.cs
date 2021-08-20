using Godot;
using System;

public class Npc : KinematicBody
{
	private float speed = 10f;
	private float rotationSpeed = 5f;
	private Vector3 direction = Vector3.Zero;
	private Vector3[] path = null; //maybe this bad?
	private int pathNode = 0;
	
	private Navigation navigation;
	private Spatial player; //for testing
	
	public override void _Ready()
	{
		navigation = GetParent<Navigation>();
		player = GetParent().GetParent().GetNode<Spatial>("Player"); //for testing
	}

	public override void _PhysicsProcess(float delta)
	{
		if (path != null && pathNode < path.Length)
		{
			direction = path[pathNode] - GlobalTransform.origin;
			//it could be cool to lerp rotate to direction and just constantly move forwards.
			if (direction.Length() < 1)
				pathNode++;
		}
		//TODO: NPC (and maybe player too) should MOVE in direction of ROTATION, not ROTATE in direction of MOVEMENT!
		//TODO: overwalk in the same direction, even if there is none!
		MoveAndSlide(direction.Normalized() * speed /* * delta*/, Vector3.Up);

		Rotation = new Vector3(
			Rotation.x,
			Mathf.LerpAngle(Rotation.y, Mathf.Atan2(-direction.x, -direction.z), rotationSpeed * delta),
			Rotation.z
		);
	}


	private void OnNavigationTimerTimeout()
	{
		MoveTo(player.GlobalTransform.origin); //go to player
	}

	public void MoveTo(Vector3 targetPosition)
	{
		path = navigation.GetSimplePath(GlobalTransform.origin, targetPosition);
		pathNode = 0;
	}
}
