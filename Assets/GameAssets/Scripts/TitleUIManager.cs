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
	
	private const float _underlineMax = 1.2f;
	
	public override void _Ready()
	{
		_play = GetNode<Button>("Play");
		
		_tabPanel = GetNode<Panel>("TabPanel");
		_tabOther = _tabPanel.GetNode<Button>("Other");
		_tabPlay = _tabPanel.GetNode<Button>("Play");
		_tabMultiplayer = _tabPanel.GetNode<Button>("Multiplayer");
		_tabPanelTween = _tabPanel.GetNode<Tween>("TabPanelTween");
		
		_multiplayerPanel = GetNode<Panel>("MultiplayerPanel");
		
		_multiplayerPanel.Visible = false;
	}
	
	#region TAB_BUTTONS
	private void onTabButtonPressed(int _index)
	{
		switch (_index)
		{
			case 1:
				GD.Print("TabOther pressed.");
				break;
			case 2:
				GD.Print("TabPlay pressed");
				break;
			case 3:
				GD.Print("TabMultiplayer pressed");
				_multiplayerPanel.Visible = true;
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
					1, //speed
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
					1, //speed
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
					1, //speed
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
					1, //speed
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
					1, //speed
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
					1, //speed
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
