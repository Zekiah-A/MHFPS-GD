using Godot;
using System;

public class DetentionTeacher : Spatial
{
    protected AudioStreamMP3 jumpscareStream;
    protected AudioStreamPlayer jumpscarePlayer;

    public override void _Ready()
    {
        jumpscareStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/jumpscare1.mp3");
        jumpscarePlayer = GetTree().CurrentScene.GetNode<AudioStreamPlayer>("JumpscarePlayer");
        jumpscarePlayer.Connect("finished", this, nameof(JumpscarePlayerFinished));
    }

    public virtual void Jumpscare()
    {
        jumpscarePlayer.Stream = jumpscareStream;
        jumpscarePlayer.Play();
    }

    public void JumpscarePlayerFinished()
    {
        jumpscarePlayer.Stop();
    }
}
