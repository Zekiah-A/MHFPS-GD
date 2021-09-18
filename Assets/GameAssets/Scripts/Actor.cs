using Godot;
using System;
using System.Collections.Generic;

public class Actor : Spatial
{
	[Export]
	public int State;
	public List<Node> BodiesInSight = new List<Node>();
	
	private AnimationPlayer animationPlayer;
	private Area actorSight;

	public override void _Ready()
	{
		animationPlayer = GetNode("GenericModel").GetNode<AnimationPlayer>("AnimationPlayer");
		actorSight = GetNode<Area>("ActorSight");

		actorSight.Connect("body_entered", this, nameof(OnBodyEnteredSight));
		actorSight.Connect("body_exited", this, nameof(OnBodyExitedSight));
	}
	
	//TODO: Could also run on ActorTick timer?
	public override void _PhysicsProcess(float delta)
	{
		if (BodiesInSight.Count != 0)
			State = (int) States.Patrol;
		else
			State = (int) States.Idle;

		ApplyState();
	}
	
	//Could add every player actor sees to a sight list, if an actor does an action, such as a crime while in the list, then reaction occurs. 
	//This is more efficient probably too! No foreach usage here.
	private void OnBodyEnteredSight(Node node)
	{
		if (node.IsInGroup("Actor") || node.IsInGroup("Player"))
			BodiesInSight.Add(node);
	}

	private void OnBodyExitedSight(Node node)
	{
		if (node.IsInGroup("Actor") || node.IsInGroup("Player"))
			BodiesInSight.Remove(node);
	}

	public void ApplyState()
	{
		switch (State)
		{
			case (int) States.Idle:
				animationPlayer.Play("Idle");
				break;
			case (int) States.Patrol:
				animationPlayer.Play("Walk"); //Obviusly only play when walking during patrol
				break;
			case (int) States.Converse:
				break;
			case (int) States.Alert:
				break;
			case (int) States.Hostile:
				break;
			case (int) States.Dead:
				break;
		}
	}
	
	public enum States
	{
		Idle = 0,
		Patrol,
		Converse,
		Alert,
		Hostile,
		Dead
	}
}
