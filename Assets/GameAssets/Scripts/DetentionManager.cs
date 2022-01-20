using Godot;
using System;

public class DetentionManager : Node
{
	public bool BunsenBurnerOn = false;
	public bool LeftDoorOpen = true;
	public bool RightDoorOpen = true;
	public int ClockTime = 0;
	
	private Texture dotCursor;
	private Texture hoverCursor;
	private AudioStreamMP3 ambientStream;
	private AudioStreamMP3 clockStream;
	private AudioStreamMP3 clockUnwindStream;
	private AudioStreamMP3[] casetteStreams;
	private AudioStreamPlayer ambientPlayer;
	private AudioStreamPlayer actionPlayer;
	private AudioStreamPlayer3D casettePlayer;
	private AudioStreamPlayer3D clockPlayer;
	private AnimationPlayer actionAnimationPlayer;
	
	private CSGBox leftDoor;
	private CSGBox rightDoor;
	private Spatial bunsenBurner;
	private Timer clockTimer;

	private DetentionTeacher stainTeacher;

	
	public override void _Ready()
	{
		dotCursor = ResourceLoader.Load<Texture>("res://Assets/GameAssets/Textures/aim_reticle.png");
		hoverCursor = ResourceLoader.Load<Texture>("res://Assets/GameAssets/Textures/aim_hover.png");
		ambientStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/ambient.mp3");
		clockStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/clock.mp3");
		clockUnwindStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/clock_unwind.mp3");
		casetteStreams = new []
		{
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete0.mp3"),
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete1.mp3"),
		};
		
		ambientPlayer = GetNode<AudioStreamPlayer>("AmbientPlayer");
		actionPlayer = GetNode("CameraBody").GetNode<AudioStreamPlayer>("PlayerActionPlayer");
		casettePlayer = GetNode("CasetteRecorder").GetNode<AudioStreamPlayer3D>("CasettePlayer");
		clockPlayer = GetNode("Clock").GetNode<AudioStreamPlayer3D>("ClockPlayer");
		leftDoor = GetNode<CSGBox>("LeftDoor");
		rightDoor = GetNode<CSGBox>("RightDoor");
		bunsenBurner = GetNode<Spatial>("BunsenBurner");
		clockTimer = GetNode("Clock").GetNode<Timer>("ClockTimer");
		actionAnimationPlayer = GetNode("CameraBody").GetNode<AnimationPlayer>("ActionAnimationPlayer");

		stainTeacher = GetNode<DetentionTeacher>("StainTeacher");
		
		BeginGame();
	}

	public void BeginGame()
	{
		Input.SetCustomMouseCursor(dotCursor);

		ambientPlayer.Stream = ambientStream;
		ambientPlayer.Play();
		
		casettePlayer.Stream = casetteStreams[0];
		casettePlayer.Play();
	}
	
	private void OnStreamFinish()
	{
		// Begin the second half of the casette announcement, after the "audio interruption".
		if (casettePlayer.Stream == casetteStreams[0])
		{
			casettePlayer.Stop();
			casettePlayer.Stream = casetteStreams[1];
			casettePlayer.Play();
		}
		//Begin the clock after the second announcement is finished.
		else if (casettePlayer.Stream == casetteStreams[1])
		{
			casettePlayer.Stop();
			clockTimer.Start();
		}
	}
	
	private void OnLeftDoorClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int) ButtonList.Left && mouseButton.Pressed)
			{
				LeftDoorOpen = !LeftDoorOpen;
				GD.Print($"Left Door open {LeftDoorOpen}");

				if (LeftDoorOpen)
				{
					leftDoor.Visible = false;
				}
				else
				{
					leftDoor.Visible = true;
				}
			}
		}
		
		//If is hovering, change cursor
		if (@event is InputEventMouse mouse)
		{
			Input.SetCustomMouseCursor(hoverCursor);
		}
	}

	private void OnRightDoorClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int) ButtonList.Left && mouseButton.Pressed)
			{
				RightDoorOpen = !RightDoorOpen;
				GD.Print($"Right Door open {RightDoorOpen}");
				
				if (RightDoorOpen)
				{
					rightDoor.Visible = false; //TODO: Each door will have it's own script that will handle it's animations, etc
				}
				else
				{
					rightDoor.Visible = true;
				}
			}
		}
		
		//If is hovering, change cursor
		if (@event is InputEventMouse mouse)
		{
			Input.SetCustomMouseCursor(hoverCursor);
		}
	}

	private void OnBunsenBurnerClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int) ButtonList.Left && mouseButton.Pressed)
			{
				BunsenBurnerOn = !BunsenBurnerOn;
				GD.Print($"Bunsen Burner on: {BunsenBurnerOn}");
				
				if (BunsenBurnerOn)
				{
					(bunsenBurner as BunsenBurner)?.Burn();
				}
				else
				{
					(bunsenBurner as BunsenBurner)?.StopBurning();
				}
			}
		}
		
		//If is hovering, change cursor
		if (@event is InputEventMouse mouse)
		{
			Input.SetCustomMouseCursor(hoverCursor);
		}
	}

	//TODO: Move all clock logic to it's own class to avoid bloating up this script
	private void OnClockClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		//Unwind clock sequence.
		if (!actionPlayer.Playing)
		{
			if (@event is InputEventMouseButton mouseButton)
			{
				if (mouseButton.ButtonIndex == (int) ButtonList.Left && mouseButton.Pressed)
				{
					actionPlayer.Stream = clockUnwindStream;
					actionPlayer.Play();
					actionAnimationPlayer.Play("unwind_animation");
					ClockTime = 0;
				}
			}
		}
		
		//If is hovering, change cursor
		if (@event is InputEventMouse mouse)
		{
			Input.SetCustomMouseCursor(hoverCursor);
		}
	}

	public void OnObjectMouseExit()
	{
		Input.SetCustomMouseCursor(dotCursor);
	}

	private void OnClockTimerTimeout()
	{	// Clock will need rewinding every 30 seconds
		ClockTime += 1;

		// Auditory warning to player that time is running out. 
		if (ClockTime >= 25 && ClockTime < 30) 
		{
			clockPlayer.Stream = clockStream;
			clockPlayer.Play();
		}
		// If gone for more than 30 seconds without rewind, then player will be jumpscared. 
		else if (ClockTime >= 30) 
		{
			//Jumpscare player, time is out.
			clockPlayer.Stop();
			stainTeacher.Jumpscare();
		}
	}
}
