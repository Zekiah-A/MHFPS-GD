using System;
using Godot;

//TODO: Namespace MHFPS.Detention
public class DetentionManager : Node
{
	public bool BunsenBurnerOn = false;
	public bool PhoneActivated = false;

	private Texture dotCursor;
	private Texture hoverCursor;
	private AudioStreamMP3 ambientStream;
	private AudioStreamMP3[] casetteStreams;
	private AudioStreamPlayer ambientPlayer;
	private AudioStreamPlayer3D casettePlayer;
	
	private Control detentionUiCanvas;
	private Spatial bunsenBurner;
	private Spatial clock;
	private Spatial phone;

	public override void _Ready()
	{
		dotCursor = ResourceLoader.Load<Texture>("res://Assets/GameAssets/Textures/aim_reticle.png");
		hoverCursor = ResourceLoader.Load<Texture>("res://Assets/GameAssets/Textures/aim_hover.png");
		ambientStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/ambient.mp3");
		casetteStreams = new []
		{
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete0.mp3"),
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete1.mp3"),
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete2.mp3")
		};
		detentionUiCanvas = GetNode<Control>("DetentionUI"); //TODO: Mouse down loading-like animation for the player crumpling up paper
		ambientPlayer = GetNode<AudioStreamPlayer>("AmbientPlayer");
		casettePlayer = GetNode("CasetteRecorder").GetNode<AudioStreamPlayer3D>("CasettePlayer");
		bunsenBurner = GetNode<Spatial>("BunsenBurner");
		clock = GetNode<Spatial>("Clock");
		phone = GetNode<Spatial>("Phone");
		
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
	
	private void OnPhoneClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int) ButtonList.Left && mouseButton.Pressed)
			{
				if (!PhoneActivated)
					(phone as Phone)?.Activate();
				else
					(phone as Phone)?.Deactivate();
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
	
	//TODO: Casette should be spelled "cassette", this needs to be fixed everywhere, including in the editor.
	private void OnCasetteRecorderClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == (int) ButtonList.Left && mouseButton.Pressed)
			{
				//Stop the casette recorder from playing. Play click sound to notify that it has been shut off.
				casettePlayer.Stream = casetteStreams[2];
				casettePlayer.Play();
				(clock as Clock)?.StartClockTimer();
			}
		}
		
		//If is hovering, change cursor
		if (@event is InputEventMouse mouse)
		{
			Input.SetCustomMouseCursor(hoverCursor);
		}
	}
	
	private void OnVentHovered(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx) //TODO: IMPLEMENT
	{
		throw new NotImplementedException();
		
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
