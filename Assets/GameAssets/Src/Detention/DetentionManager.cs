using System;
using Godot;

//TODO: Namespace MHFPS.Detention
public partial class DetentionManager : Node
{
	private AudioStreamPlayer ambientPlayer;
	private AudioStreamMP3 ambientStream;
	private Node3D bunsenBurner;
	private bool bunsenBurnerOn;
	private AudioStreamMP3[] caseCassetteStreams;
	private AudioStreamPlayer3D cassettePlayer;
	private Node3D clock;

	private Timer watcherTimer;
	
	private Texture2D dotCursor;
	private Texture2D hoverCursor;
	private Node3D phone;
	private bool phoneActivated;

	private Node3D watcher;
	private readonly Random random = new();

	public override void _Ready()
	{
		dotCursor = ResourceLoader.Load<Texture2D>("res://Assets/GameAssets/Textures/aim_reticle.png");
		hoverCursor = ResourceLoader.Load<Texture2D>("res://Assets/GameAssets/Textures/aim_hover.png");
		ambientStream = ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/ambient.mp3");
		caseCassetteStreams = new[]
		{
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete0.mp3"),
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete1.mp3"),
			ResourceLoader.Load<AudioStreamMP3>("res://Assets/GameAssets/Audio/Detention/cassete2.mp3")
		};
		ambientPlayer = GetNode<AudioStreamPlayer>("AmbientPlayer");
		cassettePlayer = GetNode<AudioStreamPlayer3D>("CasetteRecorder/CasettePlayer");
		bunsenBurner = GetNode<Node3D>("BunsenBurner");
		clock = GetNode<Node3D>("Clock");
		phone = GetNode<Node3D>("Phone");
		watcher = GetNode<Node3D>("Watcher");

		BeginGame();
	}

	public void BeginGame()
	{
		Input.SetCustomMouseCursor(dotCursor);

		ambientPlayer.Stream = ambientStream;
		ambientPlayer.Play();

		cassettePlayer.Stream = caseCassetteStreams[0];
		cassettePlayer.Play();

	}
	
	private void OnStreamFinish()
	{
		// Begin the second half of the casette announcement, after the "audio interruption".
		if (cassettePlayer.Stream == caseCassetteStreams[0])
		{
			cassettePlayer.Stop();
			cassettePlayer.Stream = caseCassetteStreams[1];
			cassettePlayer.Play();
		}
		//Begin the clock after the second announcement is finished.
		else if (cassettePlayer.Stream == caseCassetteStreams[1])
		{
			cassettePlayer.Stop();
			(clock as Clock)?.StartClockTimer();
		}
	}

	// Based off https://www.reddit.com/r/godot/comments/l6bqlw/what_is_an_efficient_calculation_for_the_aabb_of/
	public void OnPhoneClicked(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIndex)
	{
		if (camera is not Camera3D camera3D)
		{
			return;
		}

		if (phoneActivated == false)
		{
			if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
			{
				phoneActivated = true;
				(phone as Phone)?.Activate();
			}
			
			Input.SetCustomMouseCursor(hoverCursor);
			return;
		}
		
		Input.SetCustomMouseCursor(dotCursor);

		var mesh = phone.GetNode<MeshInstance3D>("Cube");
		var viewportStartX = camera3D.UnprojectPosition(mesh.ToGlobal(mesh.GetAabb().Position));
		var viewportStartY = camera3D.UnprojectPosition(mesh.ToGlobal(mesh.GetAabb().End));

		var viewportMousePosition = camera3D.UnprojectPosition(position);
		
		var viewportRetargetPosition = new Vector2(Mathf.Abs(viewportMousePosition.X - viewportStartX.X),
			Mathf.Abs(viewportMousePosition.Y - viewportStartY.Y));
		
		// ViewportStartY = viewport end x, and vice versa, idk why viewportAabbsize X and Y are inverted

		if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
		{
			var retargetedInput = new InputEventMouseButton
			{
				Position = viewportRetargetPosition,
				ButtonIndex = MouseButton.Left,
				Pressed = true,
			};
			
			(phone as Phone)?.SubViewport.PushInput(retargetedInput, true);
		}
	}

	public void OnBunsenBurnerClicked(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIndex)
	{
		if (@event is InputEventMouseButton {ButtonIndex: MouseButton.Left, Pressed: true})
		{
			bunsenBurnerOn = !bunsenBurnerOn;
			
			if (bunsenBurnerOn)
			{
				(bunsenBurner as BunsenBurner)?.Burn();
			}
			else
			{
				(bunsenBurner as BunsenBurner)?.StopBurning();
			}
		}

		//If is hovering, change cursor
		if (@event is InputEventMouse)
		{
			Input.SetCustomMouseCursor(hoverCursor);
		}
	}

	public void OnClockClicked(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIndex)
	{
		//Unwind clock sequence.
		if (@event is InputEventMouseButton {ButtonIndex: MouseButton.Left, Pressed: true})
		{
			(clock as Clock)?.Unwind();
		}

		//If is hovering, change cursor
		if (@event is InputEventMouse)
		{
			Input.SetCustomMouseCursor(hoverCursor);
		}
	}

	public void OnCassetteRecorderClicked(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIndex)
	{
		if (@event is InputEventMouseButton {ButtonIndex: MouseButton.Left, Pressed: true})
		{
			//Stop the casette recorder from playing. Play click sound to notify that it has been shut off.
			cassettePlayer.Stream = caseCassetteStreams[2];
			cassettePlayer.Play();
			
			// Only start clock if not yet started
			if ((clock as Clock)?.Started == false)
			{
				(clock as Clock)?.StartClockTimer();
			}
		}

		//If is hovering, change cursor
		if (@event is InputEventMouse)
		{
			Input.SetCustomMouseCursor(hoverCursor);
		}
	}

	public void OnVentMouseEntered()
	{
	}

	public void OnVentMouseExit()
	{
	}

	public void OnObjectMouseExit()
	{
		Input.SetCustomMouseCursor(dotCursor);
	}
}
