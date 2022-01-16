using Godot;
using System;

public class BunsenBurner : Spatial
{
	public int BurnTime = 0;
	private Spatial flame;
	private Timer burnerTimer;
	private Timer overuseTimer;
	
	public override void _Ready()
	{
		flame = GetNode<Spatial>("Flame");
		burnerTimer = GetNode<Timer>("BurnerTimer");
		overuseTimer = GetNode<Timer>("OveruseTimer");
	}

	//Start burn timer, and start overuse timer.
	public void Burn()
	{
		flame.Visible = true;
		burnerTimer.Start();
		overuseTimer.Start();
	}
	
	//Pause burn timer, and reset over use timer.
	public void StopBurning()
	{
		flame.Visible = false;
		burnerTimer.Paused = true;
		overuseTimer.Stop();
	}
	
	//Called each second, adds to burn time. Once burn time reaches 300, as work is done when the burner is on, the player will win. Enough work has been done.
	private void OnBurnerTimerFinished()
	{
		BurnTime++;
		
		//Total mach length
		if (BurnTime >= 300)
		{
			GD.Print("Game won: You sucessfully survived the night.");
		}
	}
	
	//If bunsen burner is left on for more than 20 seconds straight (the wait time of the overuse timer), game over!
	public void OnOveruseTimerFinished()
	{
		GD.Print("Game over: Time us overused.");
		//TODO: Jumpscare.
	}
}
