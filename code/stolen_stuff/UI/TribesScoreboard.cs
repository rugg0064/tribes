using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections;
using System.Collections.Generic;
using Tribes;

public class TribesScoreboard : Panel
{
	public Panel canvas; //The center shaded region
	public Panel header;
	public Panel legend;

	private IList<String> cachedNames;
	private IList<TribesScoreboardStruct> cachedPlayerData;
	public TribesScoreboard()
	{
		this.cachedNames = null;
		this.cachedPlayerData = null;

		StyleSheet.Load( "/stolen_stuff/UI/Scoreboard.scss" ); 
		AddClass( "scoreboard" );

		canvas = Add.Panel( "canvas" );
		header = canvas.Add.Panel( "header" );
		header.Add.Label( "Tribes Game - " );
		header.Add.Label( ((TribesGame)Game.Current).mainSeed.ToString() );

		legend = canvas.Add.Panel ( "legend" );
		legend.Add.Label( "Name", "legendEntry" );
		legend.Add.Label( "Deaths", "legendEntry" );
	}

	private IList<String> getNames()
	{
		return ((TribesGame)Game.Current).names;
	}

	private IList<TribesScoreboardStruct> getPlayerData()
	{
		return ((TribesGame)Game.Current).nameData;
	}

	public bool ensureUpToDate()
	{
		IList<String> names = getNames();
		IList<TribesScoreboardStruct> playerData = getPlayerData();
		if ( cachedNames == null || cachedPlayerData == null )
		{
			cachedNames = names;
			cachedPlayerData = playerData;
			return false;
		}

		if (cachedNames.Count != names.Count || cachedPlayerData.Count != playerData.Count)
		{
			cachedNames = names;
			cachedPlayerData = playerData;
			return false;
		}
		else
		{
			for (int i = 0; i < names.Count; i++)
            {
				if(cachedNames[i] != names[i] || !cachedPlayerData[i].Equals(playerData[i]))
				{
					cachedNames = names;
					cachedPlayerData = playerData;
					return false;
				}
            }
		}
		return true;
	}

	public override void Tick()
	{
		((Label)header.GetChild( 1 )).Text = ((TribesGame)Game.Current).mainSeed.ToString();
		//Log.Info( "Tick " );
		if (!ensureUpToDate())
        {
			Log.Info( "Not up to date!" );
			int curChildCount = canvas.ChildCount;
			int correctChildCount = cachedNames.Count + 2;
			Log.Info( "cur vs correct: " + curChildCount + ", " + correctChildCount );
			while(curChildCount < correctChildCount)
			{
				Log.Info( "Creating new entryPanel" );
				Panel p = canvas.Add.Panel( "entryPanel" );
				p.Add.Label("to be filled", "entry");
				p.Add.Label( "to be filled", "entry" );
				curChildCount++;
			}

			for ( int i = 0; i < correctChildCount-2; i++ )
			{
				Panel child = canvas.GetChild( i+2 );
				bool isRedTeam = cachedPlayerData[i].team;
				child.SetClass( "red", isRedTeam );
				child.SetClass( "blu", !isRedTeam );

				( (Label)child.GetChild( 0 )).Text = cachedNames[i].ToString();
				( (Label)child.GetChild( 1 )).Text = cachedPlayerData[i].deaths.ToString();
			}
        }
		//Log.Info( "Up to date!" );
		

		base.Tick();
		SetClass( "open", Input.Down( InputButton.Score ) );
	}
}
