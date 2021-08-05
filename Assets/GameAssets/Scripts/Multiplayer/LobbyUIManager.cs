using Godot;
using System;
using DiscordRPC;

public class LobbyUIManager : Control
{
	public override void _EnterTree()
	{
		TitleUIManager.client.SetPresence(new RichPresence()
		{
			Details = "github.com/Zekiah-A/MHFPS-GD",
			State = "Ingame - Multiplayer Lobby.",
			Assets = new Assets()
			{
				LargeImageKey = "coolstuff",
				LargeImageText = "S u  b l i m e ||zekiah-a.github.io/Subliminal||",
				SmallImageKey = "coolstuff"
			}
		});	
	}

	public override void _Ready()
	{
		
	}
}
