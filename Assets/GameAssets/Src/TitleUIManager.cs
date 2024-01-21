using System;
using Godot;

public partial class TitleUIManager : Control
{
	private const float UnderlineMax = 1.2f;
	private const float TweenSpeed = 0.5f;
	private Panel mainPanels;
	private int currentPanel;

	private Vector2 mainPanels1;
	private Vector2 mainPanels2;
	private Vector2 mainPanels3;
	private Button tabMultiplayer;
	private Button tabOther;
	private Panel tabPanel;
	private Button tabPlay;

	public override void _Ready()
	{
		tabPanel = GetNode<Panel>("TabPanel");
		tabOther = tabPanel.GetNode<Button>("Other");
		tabPlay = tabPanel.GetNode<Button>("Play");
		tabMultiplayer = tabPanel.GetNode<Button>("Multiplayer");
		mainPanels = GetNode<Panel>("MainPanels");

		Resized += () =>
		{
			SwitchPanel(currentPanel);
		};
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_bumperl"))
			SwitchPanel(1, true);
		if (@event.IsActionPressed("ui_bumperr"))
			SwitchPanel(3, true);
	}

	public void OnStartPressed()
	{
		GetTree().ChangeSceneToFile("res://Assets/Scenes/Intro/Intro.tscn");
	}

	//Index 1: Other panel, Index 2: Play Panel, Index 3: Multiplayer Panel
	private void OnTabButtonPressed(int index)
	{
		SwitchPanel(index);
	}

	private void OnTabButtonHover(int index)
	{
		var tween = CreateTween();
		var target = index switch
		{
			1 => tabOther,
			2 => tabPlay,
			3 => tabMultiplayer,
			_ => throw new ArgumentOutOfRangeException()
		};

		tween.TweenProperty
			(
				target.GetNode("Underline"),
				"scale",
				new Vector2(UnderlineMax, 1),
				TweenSpeed
			)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
		tween.Play();
	}

	private void OnTabButtonExit(int index)
	{
		var tween = CreateTween();
		var target = index switch
		{
			1 => tabOther,
			2 => tabPlay,
			3 => tabMultiplayer,
			_ => throw new ArgumentOutOfRangeException()
		};

		tween.TweenProperty
			(
				target.GetNode("Underline"),
				"scale",
				new Vector2(1, 1),
				TweenSpeed
			)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
		tween.Play();
	}
	
	private void SwitchPanel(int selected, bool controller = false)
	{
		var mainPanels1 = new Vector2(GetViewport().GetVisibleRect().Size.X, 0);
		var mainPanels2 = new Vector2(0, 0);
		var mainPanels3 = new Vector2(-GetViewport().GetVisibleRect().Size.X, 0);

		if (controller)
		{
			//1 = left, 3 = right
			if (mainPanels.Position == mainPanels1 && selected == 3)
			{
				selected = 2;
			}
			else if (mainPanels.Position == mainPanels2 && selected == 3)
			{
				selected = 3;
			}
			else if (mainPanels.Position == mainPanels3 && selected == 1)
			{
				selected = 2;
			}
			else if (mainPanels.Position == mainPanels2 && selected == 1)
			{
				selected = 1;
			}
		}
		
		currentPanel = selected;
		var target = selected switch
		{
			1 => mainPanels1,
			2 => mainPanels2,
			3 => mainPanels3,
		};

		var tween = CreateTween();
		tween.TweenProperty
			(
				mainPanels,
				"position",
				target,
				TweenSpeed
			)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
		tween.Play();
	}

	private void OnDemoButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Assets/Scenes/Other/Demo.tscn");
	}

	private void OnMultiplayerButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Assets/Scenes/Multiplayer/Lobby.tscn");
	}

	private void OnDetentionButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Assets/Scenes/Detention/Detention.tscn");
	}
}
