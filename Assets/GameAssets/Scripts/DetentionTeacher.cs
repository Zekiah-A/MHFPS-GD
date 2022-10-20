using Godot;
using System;

public partial class DetentionTeacher : Node3D
{
    protected AudioStreamMP3 jumpscareStream;
    private AudioStreamPlayer jumpscarePlayer;
    private ColorRect deathColourRect;
    private Tween deathScreenTween;

    public override void _Ready()
    {
        jumpscareStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/jumpscare1.mp3");
        jumpscarePlayer = GetTree().CurrentScene.GetNode<AudioStreamPlayer>("JumpscarePlayer");
        jumpscarePlayer.Connect("finished",new Callable(this,nameof(JumpscarePlayerFinished)));

        deathColourRect = GetTree().CurrentScene.GetNode("DetentionUI").GetNode("DeathScreen").GetNode<ColorRect>("DeathColour");
        deathScreenTween = GetTree().CurrentScene.GetNode("DetentionUI").GetNode("DeathScreen").GetNode<Tween>("DeathScreenTween");
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
