using Godot;
using System;

public class BunsenBurner : Spatial
{
	public int BurnTime = 0;
	private Light flameLight;
	private Particles flameParticles;
	private Timer burnerTimer;
	private Timer overuseTimer;
	private Label workLeftLabel;

	public override void _Ready()
	{
		flameLight = GetNode("Flame").GetNode<Light>("FlameLight");
		flameParticles = GetNode("Flame").GetNode<Particles>("FlameParticles");
		burnerTimer = GetNode<Timer>("BurnerTimer");
		overuseTimer = GetNode<Timer>("OveruseTimer");
		workLeftLabel = GetTree().CurrentScene.GetNode("DetentionUI").GetNode("WorkPanel").GetNode<Label>("WorkLeft");
	}

	//Start burn timer, and start overuse timer.
	public void Burn()
	{
		flameLight.Visible = true;
		flameParticles.Emitting = true;
		burnerTimer.Paused = false;
		burnerTimer.Start();
		overuseTimer.Start();
	}
	
	//Pause burn timer, and reset over use timer.
	public void StopBurning()
	{
		flameLight.Visible = false;
		flameParticles.Emitting = false;
		burnerTimer.Paused = true;
		overuseTimer.Stop();
	}
	
	//Called each second, adds to burn time. Once burn time reaches 300, as work is done when the burner is on, the player will win. Enough work has been done.
	private void OnBurnerTimerFinished()
	{
		BurnTime++;
		workLeftLabel.Text = (300 - BurnTime).ToString();
		
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
		//TODO: Use the correct Jumpscare.
		GetTree().CurrentScene.GetNode<DetentionTeacher>("StainTeacher").Jumpscare();
	}
}
