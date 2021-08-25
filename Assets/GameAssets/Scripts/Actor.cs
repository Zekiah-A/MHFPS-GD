using Godot;
using System;

public class Actor : Spatial
{
	[Export] public int State;
	
	public override void _Ready() { }
	
	//NOTE: Could also run on ActorTick timer?
	public override void _PhysicsProcess(float delta)
	{
		CheckState();
	}
	
	public void CheckState()
	{
		switch (State)
		{
			case (int) States.Idle:
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
		Alert,
		Hostile,
		Dead
	}
}

