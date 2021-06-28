using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

[Library]
public partial class TribesHud : Panel
{
	public TribesHud()
	{
		StyleSheet.Load( "UI/Hud/TribesHud.scss" );
		AddClass( "hud" );

		AddChild<Crosshair>();
		AddChild<Ammo>();
		AddChild<Speedometer>();
		AddChild<PointsOverlay>();
		AddChild<HealthBar>();
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
