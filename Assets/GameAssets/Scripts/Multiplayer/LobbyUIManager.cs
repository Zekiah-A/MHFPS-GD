using Godot;
using System;

public partial class LobbyUIManager : Control
{
	private string ipEntry;
	public static string usernameEntry;

	private void OnIpTextChanged(string newIp) => ipEntry = newIp;
	private void OnUsernameTextChanged(string newUsername) => usernameEntry = newUsername;
	private void OnConnectPressed() => Client.instance.ConnectToServer(ipEntry);


}
