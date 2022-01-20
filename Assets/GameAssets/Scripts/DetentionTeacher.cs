using Godot;
using System;

public class DetentionTeacher : Spatial
{
    protected AudioStreamMP3 jumpscareStream;
    protected AudioStreamPlayer jumpscarePlayer;
    protected TextureRect[] deathScreenRects;
    protected ColorRect deathColourRect;
    protected Tween deathScreenTween;

    public override void _Ready()
    {
        jumpscareStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/jumpscare1.mp3");
        jumpscarePlayer = GetTree().CurrentScene.GetNode<AudioStreamPlayer>("JumpscarePlayer");
        jumpscarePlayer.Connect("finished", this, nameof(JumpscarePlayerFinished));
        deathScreenRects = new[]
        {
            GetTree().CurrentScene.GetNode("DetentionUI").GetNode("DeathScreen").GetNode<TextureRect>("DeathRect1"),
            GetTree().CurrentScene.GetNode("DetentionUI").GetNode("DeathScreen").GetNode<TextureRect>("DeathRect2"),
            GetTree().CurrentScene.GetNode("DetentionUI").GetNode("DeathScreen").GetNode<TextureRect>("DeathRect3"),
            GetTree().CurrentScene.GetNode("DetentionUI").GetNode("DeathScreen").GetNode<TextureRect>("DeathRect4"),
        };

        deathColourRect = GetTree().CurrentScene.GetNode("DetentionUI").GetNode("DeathScreen").GetNode<ColorRect>("DeathColour");
        
        deathScreenTween = GetTree().CurrentScene.GetNode("DetentionUI").GetNode("DeathScreen").GetNode<Tween>("DeathScreenTween");
    }

    public virtual void Jumpscare()
    {
        jumpscarePlayer.Stream = jumpscareStream;
        jumpscarePlayer.Play();
        
        PlayDeathSequence();
    }

    public void JumpscarePlayerFinished()
    {
        jumpscarePlayer.Stop();
    }
    
    // Death sequence animation.
    public void PlayDeathSequence()
    {
        for (int i = 0; i < 3; i++) //TODO: Change the opacity of the fade in shader, so each will fade in over the span of a few seconds.
        {
            deathScreenRects[i].Visible = true;
        }
    }
}
