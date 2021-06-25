using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

[Library]
public partial class DeathmatchHud : HudEntity<RootPanel>
{
	public DeathmatchHud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "stolen_stuff/UI/DeathmatchHud.scss" );

		RootPanel.AddChild<Ammo>();
		RootPanel.AddChild<Speedometer>();
		RootPanel.AddChild<TribesScoreboard>();
	}

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
}
