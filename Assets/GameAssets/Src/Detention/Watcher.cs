using Godot;
using System;

public partial class Watcher : Node3D
{
	private const int AppearCooldown = 4;//60;
	private const int MinKillTime = 16;
	private const int MaxKillTIme = 32;
	
	private Timer appearTimer;
	private Timer killTimer;
	private Random random = new();
	private AnimationPlayer animationPlayer;
	private AudioStreamPlayer3D audioPlayer;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		appearTimer = GetNode<Timer>("AppearTimer");
		killTimer = GetNode<Timer>("KillTimer");
		audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
			
		appearTimer.WaitTime = AppearCooldown + random.Next(0, AppearCooldown);
		Visible = false;

		appearTimer.Timeout += OnAppearTimerTimeout;
		killTimer.Timeout += OnKillTimerTimeout;
		
		appearTimer.Start();
	}

	// Appear
	public void OnAppearTimerTimeout()
	{
		Visible = true;
		animationPlayer.Play("watcher_appear");
		killTimer.WaitTime = random.Next(MinKillTime, MaxKillTIme);
		killTimer.Start();
		audioPlayer.Play();
	}
	
	// Disappear
	public void OnWatcherAreaHit()
	{
		animationPlayer.Play("watcher_disappear");
		audioPlayer.Stop();
	}
	
	public void OnKillTimerTimeout()
	{
		animationPlayer.Play("watcher_disappear");
		GD.Print("[Detention] Killed: Did not inhibit watcher.");
	}
}
