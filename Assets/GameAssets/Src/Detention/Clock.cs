using System;
using Godot;

public partial class Clock : Node3D
{
    private const int MaximumTime = 60;
    private const int WarnTime = 5;
    private AnimationPlayer actionAnimationPlayer;
    private AudioStreamPlayer actionPlayer;
    private AudioStreamPlayer3D clockPlayer;

    private AudioStreamMP3 clockStream;
    private int clockTime;
    private Timer clockTimer;
    private AudioStreamMP3 clockUnwindStream;
    private Node3D minuteHand;

    private DetentionTeacher stainTeacher;

    public override void _Ready()
    {
        clockStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/clock.mp3");
        clockUnwindStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/clock_unwind.mp3");
        clockPlayer = GetNode<AudioStreamPlayer3D>("ClockPlayer");
        clockTimer = GetNode<Timer>("ClockTimer");
        minuteHand = GetNode<Node3D>("minutes");

        actionPlayer = GetTree().CurrentScene.GetNode("CameraBody").GetNode<AudioStreamPlayer>("PlayerActionPlayer");
        actionAnimationPlayer = GetTree().CurrentScene
            .GetNode("CameraBody")
            .GetNode<AnimationPlayer>("ActionAnimationPlayer");
        stainTeacher = GetTree().CurrentScene.GetNode<DetentionTeacher>("StainTeacher");

        minuteHand.Rotation = new Vector3(minuteHand.Rotation.X, minuteHand.Rotation.Y, 0);
    }

    public void Unwind()
    {
        if (actionPlayer.Playing) return;

        actionPlayer.Stream = clockUnwindStream;
        actionPlayer.Play();
        actionAnimationPlayer.Play("unwind_animation");
        clockTime = 0;

        minuteHand.Rotation = new Vector3(minuteHand.Rotation.X, minuteHand.Rotation.Y, 0);
    }

    // Clock does not seem to automatically tick after the radio announcement, as the audio sequence overruns by about 10 seconds.
    public void StartClockTimer()
    {
        clockTimer.Start();
    }

    private void OnClockTimerTimeout()
    {
        // Clock will need rewinding every 30 seconds
        clockTime += 1;

        // Auditory warning to player that time is running out. 
        if (clockTime >= MaximumTime - WarnTime && clockTime < MaximumTime)
        {
            clockPlayer.Stream = clockStream;
            clockPlayer.Play();
        }
        // If gone for more than 30 seconds without rewind, then player will be jumpscared. 
        else if (clockTime >= MaximumTime)
        {
            //Jumpscare player, time is out.
            clockPlayer.Stop();
            clockTimer.Stop();
            stainTeacher.Jumpscare();
        }

        // Minute hand needs to travel 360 degrees to get to 6:00 in 30 seconds, so 12 degrees per second. Minus twelve because this model is stupid and the co-ords are wrong. 🤓🔫 
        minuteHand.Rotation = new Vector3(minuteHand.Rotation.X, minuteHand.Rotation.Y,
            (float) (minuteHand.Rotation.Z - 2 * Math.PI / MaximumTime));
    }
}