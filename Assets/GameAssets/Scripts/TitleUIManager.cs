using Godot;
using System;

public class TitleUIManager : Control
{
	private Button _play;
	
	private Panel _tabPanel;
	private Button _tabOther;
	private Button _tabPlay;
	private Button _tabMultiplayer;
	private Tween _tabPanelTween;
	
	private Panel _multiplayerPanel;
	private Tween _multiplayerTween;
	
	private const float _underlineMax = 1.2f;
	private const float _tweenSpeed = 0.5f;
	
	public override void _Ready()
	{
		_play = GetNode<Button>("Play");
		
		_tabPanel = GetNode<Panel>("TabPanel");
		_tabOther = _tabPanel.GetNode<Button>("Other");
		_tabPlay = _tabPanel.GetNode<Button>("Play");
		_tabMultiplayer = _tabPanel.GetNode<Button>("Multiplayer");
		_tabPanelTween = _tabPanel.GetNode<Tween>("TabPanelTween");
		
		_multiplayerPanel = GetNode<Panel>("MultiplayerPanel");
		_multiplayerTween = _multiplayerPanel.GetNode<Tween>("MultiplayerTween");
		
		_multiplayerPanel.Visible = false;
	}
	
	#region TAB_BUTTONS
	private void onTabButtonPressed(int _index)
	{
		switch (_index)
		{
			case 1:
				GD.Print("TabOther pressed.");
				GetTree().ChangeScene("res://Assets/Scenes/Other/Demo.tscn");
				break;
			case 2:
				GD.Print("TabPlay pressed");
				break;
			case 3:
				if (_multiplayerPanel.Visible == false)
				{
					_multiplayerPanel.Visible = true;
					_multiplayerTween.InterpolateProperty (
						_multiplayerPanel, //Object
						"rect_position", //Property being tweened
						new Vector2(128, -472), //from
						new Vector2(128, 64), //to
						_tweenSpeed, //speed
						Tween.TransitionType.Cubic,
						Tween.EaseType.Out
					);
					_multiplayerTween.Start();
					GD.Print("TabMultiplayer pressed");
				} 
				else
					_multiplayerPanel.Visible = false;
				break;
		}
	}
	
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
}
