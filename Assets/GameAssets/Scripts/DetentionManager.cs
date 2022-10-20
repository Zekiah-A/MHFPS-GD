using Godot;

//TODO: Namespace MHFPS.Detention
public partial class DetentionManager : Node
{
	private bool bunsenBurnerOn;
	private bool phoneActivated;

	private Texture2D dotCursor;
	private Texture2D hoverCursor;
	private AudioStreamMP3 ambientStream;
	private AudioStreamMP3[] caseCassettestreams;
	private AudioStreamPlayer ambientPlayer;
	private AudioStreamPlayer3D cassettePlayer;
	
	private Control detentionUiCanvas;
	private Node3D bunsenBurner;
	private Node3D clock;
	private Node3D phone;
	private Node3D doors;

	public override void _Ready()
	{
		dotCursor = ResourceLoader.Load<Texture2D>("res://Assets/GameAssets/Textures/aim_reticle.png");
		hoverCursor = ResourceLoader.Load<Texture2D>("res://Assets/GameAssets/Textures/aim_hover.png");
		ambientStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/ambient.mp3");
		caseCassettestreams = new []
		{
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete0.mp3"),
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete1.mp3"),
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete2.mp3")
		};
		detentionUiCanvas = GetNode<Control>("DetentionUI"); //TODO: Mouse down loading-like animation for the player crumpling up paper
		ambientPlayer = GetNode<AudioStreamPlayer>("AmbientPlayer");
		cassettePlayer = GetNode("CasetteRecorder").GetNode<AudioStreamPlayer3D>("CasettePlayer");
		bunsenBurner = GetNode<Node3D>("BunsenBurner");
		clock = GetNode<Node3D>("Clock");
		phone = GetNode<Node3D>("Phone");
		doors = GetTree().CurrentScene.GetNode<Node3D>("Doors");

		BeginGame();
	}

	public void BeginGame()
	{
		Input.SetCustomMouseCursor(dotCursor);

		ambientPlayer.Stream = ambientStream;
		ambientPlayer.Play();
		
		cassettePlayer.Stream = caseCassettestreams[0];
		cassettePlayer.Play();
	}
	
	private void OnStreamFinish()
	{
		// Begin the second half of the casette announcement, after the "audio interruption".
		if (cassettePlayer.Stream == caseCassettestreams[0])
		{
			cassettePlayer.Stop();
			cassettePlayer.Stream = caseCassettestreams[1];
			cassettePlayer.Play();
		}
		//Begin the clock after the second announcement is finished.
		else if (cassettePlayer.Stream == caseCassettestreams[1])
		{
			cassettePlayer.Stop();
			(clock as Clock)?.StartClockTimer();
		}
	}
	
	private void OnPhoneClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				if (!phoneActivated)
				{
					(phone as Phone)?.Activate();
					//DetentionCamera.BindRotate(phone);
				}
				else
				{
					(phone as Phone)?.Deactivate();
					//DetentionCamera.UnbindRotate(phone);
				}
			}
		}
		
		//If is hovering, change cursor
		if (@event is InputEventMouse)
			Input.SetCustomMouseCursor(hoverCursor);
	}

	private void OnBunsenBurnerClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				bunsenBurnerOn = !bunsenBurnerOn;
				GD.Print($"Bunsen Burner on: {bunsenBurnerOn}");
				
				if (bunsenBurnerOn)
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
		if (@event is InputEventMouse)
			Input.SetCustomMouseCursor(hoverCursor);
	}
	
	private void OnClockClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		//Unwind clock sequence.
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				(clock as Clock)?.Unwind();
			}
		}
		
		//If is hovering, change cursor
		if (@event is InputEventMouse)
			Input.SetCustomMouseCursor(hoverCursor);
	}
	
	//TODO: Casette should be spelled "cassette", this needs to be fixed everywhere, including in the editor.
	private void OnCasetteRecorderClicked(object camera, object @event, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				//Stop the casette recorder from playing. Play click sound to notify that it has been shut off.
				cassettePlayer.Stream = caseCassettestreams[2];
				cassettePlayer.Play();
				(clock as Clock)?.StartClockTimer();
			}
		}
		
		//If is hovering, change cursor
		if (@event is InputEventMouse)
			Input.SetCustomMouseCursor(hoverCursor);
	}
	
	//TODO: Implement signals
	private void OnVentMouseEntered() =>
		(doors as Doors)?.TorchOnVent();

	private void OnVentMouseExit() =>
		(doors as Doors)?.TorchOffVent();

	public void OnObjectMouseExit() =>
		Input.SetCustomMouseCursor(dotCursor);
}
