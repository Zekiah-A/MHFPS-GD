using Godot;

public partial class StainTeacher : DetentionTeacher
{
    public override void _Ready()
    {
        //Teachers may override ready in order to have their own jumpscare sounds.
        jumpscareStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/jumpscare1.mp3");
        base._Ready();
    }

    public override void Jumpscare()
    {
        GD.Print("Game Over: Jumpscared by Stain teacher.");
        base.Jumpscare();
    }
}