using Godot;
using System;
using DiscordRPC;

public partial class LobbyUIManager : Control
{
	private string ipEntry;
	public static string usernameEntry;

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

	private void OnIpTextChanged(string newIp) => ipEntry = newIp;
	private void OnUsernameTextChanged(string newUsername) => usernameEntry = newUsername;
	private void OnConnectPressed() => Client.instance.ConnectToServer(ipEntry);


}
