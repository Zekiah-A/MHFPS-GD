using Godot;
using System;
using DiscordRPC;

public partial class TitleUIManager : Control
{	
	private Panel tabPanel;
	private Godot.Button tabOther;
	private Godot.Button tabPlay;
	private Godot.Button tabMultiplayer;
	private Tween tabPanelTween;
	private Tween panelTween;
	private Panel mainPanels;

	private Vector2 mainPanels1;
	private Vector2 mainPanels2;
	private Vector2 mainPanels3;
	
	private const float UnderlineMax = 1.2f;
	private const float TweenSpeed = 0.5f;
	
	public static DiscordRpcClient client;
	
	public override void _Ready()
	{		
		tabPanel = GetNode<Panel>("TabPanel");
		tabOther = tabPanel.GetNode<Godot.Button>("Other");
		tabPlay = tabPanel.GetNode<Godot.Button>("Play");
		tabMultiplayer = tabPanel.GetNode<Godot.Button>("Multiplayer");
		tabPanelTween = tabPanel.GetNode<Tween>("TabPanelTween");
		
		mainPanels = GetNode<Panel>("MainPanels");
		panelTween = GetNode<Tween>("PanelTween");

		Connect("resized",new Callable(this,nameof(OnUIResized)));
		mainPanels1 = new Vector2(GetViewport().GetVisibleRect().Size.x, 0);
		mainPanels2 = new Vector2(0, 0);
		mainPanels3 = new Vector2(-GetViewport().GetVisibleRect().Size.x, 0);

		//TODO: ID should be obfuscated?
		///<summary>Rich presence for Discord.</summary>
		client = new DiscordRpcClient("870411160363614358");

		client.OnReady += (sender, e) =>
		{
			GD.Print("Received Ready from user {0}", e.User.Username);
		};
			
		client.OnPresenceUpdate += (sender, e) =>
		{
			GD.Print("Received Update! {0}", e.Presence);
		};

		client.Initialize();
		client.SetPresence(new RichPresence
		{
			Details = "github.com/Zekiah-A/MHFPS-GD",
			State = "Ingame",
			Assets = new Assets()
			{
				LargeImageKey = "coolstuff",
				LargeImageText = "|| Playing MHFPS ||",
				SmallImageKey = "coolstuff"
			}
		});	
	}

	public override void _UnhandledInput(InputEvent @event)
	{	//should this whole system just be relative L/R
		//make ui bumpers on controller switch tab
		if (@event.IsActionPressed("ui_bumperl"))
			SwitchPanel(1, true);
		if (@event.IsActionPressed("ui_bumperr"))
			SwitchPanel(3, true);
	}

	public void OnStartPressed() => GetTree().ChangeSceneToFile("res://Assets/Scenes/Intro/Intro.tscn");

	//Index 1: Other panel, Index 2: Play Panel, Index 3: Multiplayer Panel
	private void OnTabButtonPressed(int index) => SwitchPanel(index);
	
	private void OnTabButtonHover(int index)
	{
		switch (index)
		{
			case 1:
				/*tabPanelTween.InterpolateProperty (
					tabOther.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(1, 1), //from
					new Vector2(UnderlineMax, 1), //to
					TweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				tabPanelTween.Start();*/
				break;
			case 2:
				/*tabPanelTween.InterpolateProperty (
					tabPlay.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(1, 1), //from
					new Vector2(UnderlineMax, 1), //to
					TweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				tabPanelTween.Start();*/
				break;
			case 3:
				/*tabPanelTween.InterpolateProperty (
					tabMultiplayer.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(1, 1), //from
					new Vector2(UnderlineMax, 1), //to
					TweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				tabPanelTween.Start();*/
				break;
		}
	}
	
	private void OnTabButtonExit(int index)
	{
		switch (index)
		{
			case 1:
				/*tabPanelTween.InterpolateProperty (
					tabOther.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(UnderlineMax, 1), //from
					new Vector2(1, 1), //to
					TweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				tabPanelTween.Start();*/
				break;
			case 2:
				/*tabPanelTween.InterpolateProperty (
					tabPlay.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(UnderlineMax, 1), //from
					new Vector2(1, 1), //to
					TweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				tabPanelTween.Start();*/
				break;
			case 3:
				/*tabPanelTween.InterpolateProperty (
					tabMultiplayer.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(UnderlineMax, 1), //from
					new Vector2(1, 1), //to
					TweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				tabPanelTween.Start();*/
				break;
		}
	}

	private void OnUIResized()
	{
		if (mainPanels.Position == mainPanels1)
			SwitchPanel(1);
		else if (mainPanels.Position == mainPanels2)
			SwitchPanel(2);
		else if (mainPanels.Position == mainPanels3)
			SwitchPanel(3);
	}
	
	private void SwitchPanel(int selected, bool controller = false)
	{
		mainPanels1 = new Vector2(GetViewport().GetVisibleRect().Size.x, 0);
		mainPanels2 = new Vector2(0, 0);
		mainPanels3 = new Vector2(-GetViewport().GetVisibleRect().Size.x, 0);
		
		if (controller)
		{
			//1 = left, 3 = right
			if (mainPanels.Position == mainPanels1 && selected == 3)
				selected = 2;
			else if (mainPanels.Position == mainPanels2 && selected == 3)
				selected = 3;
			
			else if (mainPanels.Position == mainPanels3 && selected == 1)
				selected = 2;
			else if (mainPanels.Position == mainPanels2 && selected == 1)
				selected = 1;
		}

		switch (selected)
		{
			case 1:
				//Focus "Other" panel to the centre by moving all
				/*panelTween.InterpolateProperty (
					mainPanels,
					"rect_position", //Property being tweened
					mainPanels.RectPosition, //from
					mainPanels1, //to
					TweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);*/
				break;

			case 2:
				//Focus "Player" panel to the centre by moving all
				/*panelTween.InterpolateProperty (
					mainPanels,
					"rect_position", //Property being tweened
					mainPanels.RectPosition, //from
					mainPanels2, //to
					TweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);*/
				break;

			case 3:
				//Focus "Multiplayer" panel to the centre by moving all
				/*panelTween.InterpolateProperty (
					mainPanels,
					"rect_position", //Property being tweened
					mainPanels.RectPosition, //from
					mainPanels3, //to
					TweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);*/
				break;
		}
		/*panelTween.Start();*/
	}
	
	private void OnDemoButtonPressed() => GetTree().ChangeSceneToFile("res://Assets/Scenes/Other/Demo.tscn");
	private void OnMultiplayerButtonPressed() => GetTree().ChangeSceneToFile("res://Assets/Scenes/Multiplayer/Lobby.tscn");
	private void OnDetentionButtonPressed() => GetTree().ChangeSceneToFile("res://Assets/Scenes/Detention/Detention.tscn");
}
