using Godot;
using System;

public class DetentionManager : Node
{
	private AudioStreamMP3 ambientStream;
	private AudioStreamMP3[] casetteStreams;
	private AudioStreamPlayer ambientPlayer;
	private AudioStreamPlayer3D casettePlayer;
	private AudioStreamPlayer3D clockPlayer;
	
	public override void _Ready()
	{
		ambientStream = GD.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/ambient.mp3");
		casetteStreams = new []
		{
			GD.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete0.mp3"),
			GD.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete1.mp3"),
		};
		ambientPlayer = GetNode<AudioStreamPlayer>("AmbientPlayer");
		casettePlayer = GetNode("CasetteRecorder").GetNode<AudioStreamPlayer3D>("CasettePlayer");
		clockPlayer = GetNode("Clock").GetNode<AudioStreamPlayer3D>("ClockPlayer");
		
		BeginGame();
	}

	public void BeginGame()
	{
		Input.SetMouseMode(Input.MouseMode.Captured);

		ambientPlayer.Stream = ambientStream;
		ambientPlayer.Play();
		
		casettePlayer.Stream = casetteStreams[0];
		casettePlayer.Play();
	}
	
	private void OnStreamFinish()
	{
		casettePlayer.Stop();
		casettePlayer.Stream = casetteStreams[1];
		casettePlayer.Play();
	}
}
