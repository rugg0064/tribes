using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

[Library]
public partial class TribesUI : HudEntity<RootPanel>
{
	public TribesUI()
	{
		if ( !IsClient )
			return;
		
		RootPanel.AddChild<TribesHud>();
		RootPanel.AddChild<TribesScoreboard>();
	}

	/*
	[ClientRpc]
	public void OnPlayerDied( string victim, string attacker = null )
	{
		Host.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Host.AssertClient();
	}
	*/
}
