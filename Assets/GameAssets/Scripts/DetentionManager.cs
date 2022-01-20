using Godot;
using System;

public class DetentionManager : Node
{
	public bool BunsenBurnerOn = false;
	public bool LeftDoorOpen = true;
	public bool RightDoorOpen = true;
	
	private Texture dotCursor;
	private Texture hoverCursor;
	private AudioStreamMP3 ambientStream;
	private AudioStreamMP3[] casetteStreams;
	private AudioStreamPlayer ambientPlayer;
	private AudioStreamPlayer3D casettePlayer;
	
	private CSGBox leftDoor;
	private CSGBox rightDoor;
	private Spatial bunsenBurner;
	private Spatial clock;

	public override void _Ready()
	{
		dotCursor = ResourceLoader.Load<Texture>("res://Assets/GameAssets/Textures/aim_reticle.png");
		hoverCursor = ResourceLoader.Load<Texture>("res://Assets/GameAssets/Textures/aim_hover.png");
		ambientStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/ambient.mp3");
		casetteStreams = new []
		{
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete0.mp3"),
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete1.mp3"),
		};
		ambientPlayer = GetNode<AudioStreamPlayer>("AmbientPlayer");
		casettePlayer = GetNode("CasetteRecorder").GetNode<AudioStreamPlayer3D>("CasettePlayer");
		leftDoor = GetNode<CSGBox>("LeftDoor");
		rightDoor = GetNode<CSGBox>("RightDoor");
		bunsenBurner = GetNode<Spatial>("BunsenBurner");
		clock = GetNode<Spatial>("Clock");
		
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
			(clock as Clock)?.StartClockTimer();
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
	
	private void OnClockClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		//Unwind clock sequence.
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int) ButtonList.Left && mouseButton.Pressed)
			{
				(clock as Clock)?.Unwind();
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
}
