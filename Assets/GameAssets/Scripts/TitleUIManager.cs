using Godot;
using System;

public class TitleUIManager : Control
{	
	private Panel _tabPanel;
	private Button _tabOther;
	private Button _tabPlay;
	private Button _tabMultiplayer;
	private Tween _tabPanelTween;
	
	private Panel _mainPanels;
	private Tween _panelTween;

	private const float _underlineMax = 1.2f;
	private const float _tweenSpeed = 0.5f;
	
	public override void _Ready()
	{		
		_tabPanel = GetNode<Panel>("TabPanel");
		_tabOther = _tabPanel.GetNode<Button>("Other");
		_tabPlay = _tabPanel.GetNode<Button>("Play");
		_tabMultiplayer = _tabPanel.GetNode<Button>("Multiplayer");
		_tabPanelTween = _tabPanel.GetNode<Tween>("TabPanelTween");
		
		_mainPanels = GetNode<Panel>("MainPanels");
		_panelTween = GetNode<Tween>("PanelTween");
	}

	public override void _UnhandledInput(InputEvent @event)
	{	//should this whole system just be relative L/R
		//make ui bumpers on controller switch tab
		if (@event.IsActionPressed("ui_bumperl"))
			SwitchPanel(1, true);
		if (@event.IsActionPressed("ui_bumperr"))
			SwitchPanel(3, true);
	}
	
	#region TAB_BUTTONS
	//Index 1: Other panel, Index 2: Play Panel, Index 3: Multiplayer Panel
	private void onTabButtonPressed(int _index) => SwitchPanel(_index);
	
	private void onTabButtonHover(int _index)
	{
		switch (_index)
		{
			case 1:
				_tabPanelTween.InterpolateProperty (
					_tabOther.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(1, 1), //from
					new Vector2(_underlineMax, 1), //to
					_tweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				_tabPanelTween.Start();
				GD.Print("TabOther hovered.");
				break;
			case 2:
				_tabPanelTween.InterpolateProperty (
					_tabPlay.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(1, 1), //from
					new Vector2(_underlineMax, 1), //to
					_tweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				_tabPanelTween.Start();
				GD.Print("TabPlay hovered");
				break;
			case 3:
				_tabPanelTween.InterpolateProperty (
					_tabMultiplayer.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(1, 1), //from
					new Vector2(_underlineMax, 1), //to
					_tweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				_tabPanelTween.Start();
				GD.Print("TabMultiplayer hovered");
				break;
		}
	}
	
	private void onTabButtonExit(int _index)
	{
		switch (_index)
		{
			case 1:
				_tabPanelTween.InterpolateProperty (
					_tabOther.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(_underlineMax, 1), //from
					new Vector2(1, 1), //to
					_tweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				_tabPanelTween.Start();
				GD.Print("TabOther exited.");
				break;
			case 2:
				_tabPanelTween.InterpolateProperty (
					_tabPlay.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(_underlineMax, 1), //from
					new Vector2(1, 1), //to
					_tweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				_tabPanelTween.Start();
				GD.Print("TabPlay exited");
				break;
			case 3:
				_tabPanelTween.InterpolateProperty (
					_tabMultiplayer.GetNode("Underline"), //Object
					"rect_scale", //Property being tweened
					new Vector2(_underlineMax, 1), //from
					new Vector2(1, 1), //to
					_tweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				_tabPanelTween.Start();
				GD.Print("TabMultiplayer exited");
				break;
		}
	}
	#endregion

	#region PANEL_LOGIC
	private void SwitchPanel(int _selected, bool controller = true)
	{

		Vector2 _mainPanels1 = new Vector2(GetViewport().GetVisibleRect().Size.x, 0);
		Vector2 _mainPanels2 = new Vector2(0, 0);
		Vector2 _mainPanels3 = new Vector2(-GetViewport().GetVisibleRect().Size.x, 0);

		if (controller)
		{ //1 = left, 3 = right
			if (_mainPanels.RectPosition == _mainPanels1 && _selected == 3)
				_selected = 2;
			else if (_mainPanels.RectPosition == _mainPanels2 && _selected == 3)
				_selected = 3;
			
			else if (_mainPanels.RectPosition == _mainPanels3 && _selected == 1)
				_selected = 2;
			else if (_mainPanels.RectPosition == _mainPanels2 && _selected == 1)
				_selected = 1;
		}

		switch (_selected)
		{
			case 1:
				//Focus "Other" panel to the centre by moving all
				_panelTween.InterpolateProperty (
					_mainPanels,
					"rect_position", //Property being tweened
					_mainPanels.RectPosition, //from
					_mainPanels1, //to
					_tweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				break;

			case 2:
				//Focus "Player" panel to the centre by moving all
				_panelTween.InterpolateProperty (
					_mainPanels,
					"rect_position", //Property being tweened
					_mainPanels.RectPosition, //from
					_mainPanels2, //to
					_tweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				break;

			case 3:
				//Focus "Multiplayer" panel to the centre by moving all
				_panelTween.InterpolateProperty (
					_mainPanels,
					"rect_position", //Property being tweened
					_mainPanels.RectPosition, //from
					_mainPanels3, //to
					_tweenSpeed, //speed
					Tween.TransitionType.Cubic,
					Tween.EaseType.Out
				);
				break;
		}
		_panelTween.Start();
	}
	#endregion

	#region WIDGET_LOGIC
	
	private void OnDemoButtonPressed() => GetTree().ChangeScene("res://Assets/Scenes/Other/Demo.tscn");

	#endregion
}
