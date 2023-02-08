using Godot;
using System;

public partial class BunsenBurner : Node3D
{
	private int burnTime;
	private Light3D flameLight;
	private GpuParticles3D flameParticles;
	private Timer burnerTimer;
	private Timer overuseTimer;
	private Label workLeftLabel;

	public override void _Ready()
	{
		flameLight = GetNode("Flame").GetNode<Light3D>("FlameLight");
		flameParticles = GetNode("Flame").GetNode<GpuParticles3D>("FlameParticles");
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
		burnTime++;
		workLeftLabel.Text = (300 - burnTime).ToString();
		
		//Total mach length
		if (burnTime >= 300)
		{
			GD.Print("Game won: You successfully survived the night.");
		}
	}
	
	//If bunsen burner is left on for more than 20 seconds straight (the wait time of the overuse timer), game over!
	public void OnOveruseTimerFinished()
	{
		GD.Print("Game over: Bunsen burner overuse.");
		//TODO: Use the correct Jumpscare.
		GetTree().CurrentScene.GetNode<DetentionTeacher>("StainTeacher").Jumpscare();
	}
}
