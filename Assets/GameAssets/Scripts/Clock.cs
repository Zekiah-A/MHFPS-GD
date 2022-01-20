using Godot;
using System;

public class Clock : Spatial
{
	public int ClockTime = 0;
	
	private AudioStreamMP3 clockStream;
	private AudioStreamMP3 clockUnwindStream;
	private AudioStreamPlayer3D clockPlayer;
	private AudioStreamPlayer actionPlayer;
	private AnimationPlayer actionAnimationPlayer;
	
	private DetentionTeacher stainTeacher;
	private Timer clockTimer;
	private Spatial minuteHand;
	
	private const int maximumTime = 30;
	private const int warnTime = 5;

	public override void _Ready()
	{
		clockStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/clock.mp3");
		clockUnwindStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/clock_unwind.mp3");
		clockPlayer = GetNode<AudioStreamPlayer3D>("ClockPlayer");
		clockTimer = GetNode<Timer>("ClockTimer");
		minuteHand = GetNode<Spatial>("minutes"); //TODO: Make a convention in the dev guide for name casing and fix this.

		actionPlayer = GetTree().CurrentScene.GetNode("CameraBody").GetNode<AudioStreamPlayer>("PlayerActionPlayer");
		actionAnimationPlayer = GetTree().CurrentScene.GetNode("CameraBody").GetNode<AnimationPlayer>("ActionAnimationPlayer");
		stainTeacher = GetTree().CurrentScene.GetNode<DetentionTeacher>("StainTeacher");

		clockTimer.Connect("timeout", this, nameof(OnClockTimerTimeout)); //TODO: Connect through editor instead.
		minuteHand.RotationDegrees = new Vector3(minuteHand.RotationDegrees.x, minuteHand.RotationDegrees.y, 0);

	}
	
	public void Unwind()
	{
		if (!actionPlayer.Playing)
		{
			actionPlayer.Stream = clockUnwindStream;
			actionPlayer.Play();
			actionAnimationPlayer.Play("unwind_animation");
			ClockTime = 0; //TODO: Make the clock animate too, override tweens in order to make it wind back realistically with the player's hand.
			
			minuteHand.RotationDegrees = new Vector3(minuteHand.RotationDegrees.x, minuteHand.RotationDegrees.y, 0);
		}
	}

	// Clock does not seem to automatically tick after the radio announcement, as the audio sequence overruns by about 10 seconds.
	public void StartClockTimer()
	{
		clockTimer.Start();
	}
	
	private void OnClockTimerTimeout()
	{	// Clock will need rewinding every 30 seconds
		ClockTime += 1;

		// Auditory warning to player that time is running out. 
		if (ClockTime >= maximumTime - warnTime && ClockTime < maximumTime) 
		{
			clockPlayer.Stream = clockStream;
			clockPlayer.Play();
		}
		// If gone for more than 30 seconds without rewind, then player will be jumpscared. 
		else if (ClockTime >= maximumTime) 
		{
			//Jumpscare player, time is out.
			clockPlayer.Stop();
			stainTeacher.Jumpscare();
		}
		GD.Print(minuteHand.Rotation.z);
		// Minute hand needs to travel 360 degrees to get to 6:00 in 30 seconds, so 12 degrees per second. Minus twelve because this model is stupid and the co-ords are wrong. 🤓🔫 
		minuteHand.RotationDegrees = new Vector3(minuteHand.RotationDegrees.x, minuteHand.RotationDegrees.y, minuteHand.RotationDegrees.z - 12); //TODO: Make fancy tweens to make it look good. 
	}
}

