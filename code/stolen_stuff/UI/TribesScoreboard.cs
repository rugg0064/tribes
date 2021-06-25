using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections;
using System.Collections.Generic;
using Tribes;

public class TribesScoreboard : Panel
{
	public Panel canvas; //The center shaded region, with alignment settings

	private IList<String> cachedNames;
	private IList<TribesScoreboardStruct> cachedPlayerData;
	public TribesScoreboard()
	{
		this.cachedNames = null;
		this.cachedPlayerData = null;
		//cachedNames = getNames();
		//cachedPlayerData = getPlayerData();

		StyleSheet.Load( "/stolen_stuff/UI/Scoreboard.scss" ); 
		AddClass( "scoreboard" );
		//With the TribesScoreboard panel being a scoreboard class, any children will have common text settings.
		//Every component is then added to the canvas, which is the centered shaded part.
		canvas = Add.Panel( "canvas" );
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
		
		//Log.Info( "Tick " );
		if (!ensureUpToDate())
        {
			Log.Info( "Not up to date!" );
			int curChildCount = canvas.ChildCount;
			int correctChildCount = cachedNames.Count;
			Log.Info( "cur vs correct: " + curChildCount + ", " + correctChildCount );
			while(curChildCount < correctChildCount)
			{
				Log.Info( "Creating new entryPanel" );
				Panel p = canvas.Add.Panel( "entryPanel" );
				p.Add.Label("to be filled");
				curChildCount++;
			}

			for ( int i = 0; i < correctChildCount; i++ )
			{
				Panel child = canvas.GetChild( i );
				bool isRedTeam = cachedPlayerData[i].team;
				child.SetClass( "red", isRedTeam );
				child.SetClass( "blu", !isRedTeam );


				Panel subchild = child.GetChild( 0 );
				Label subChildLabel = (Label) subchild;
				subChildLabel.Text = cachedNames[i].ToString();
			}
        }
		//Log.Info( "Up to date!" );
		

		base.Tick();
		SetClass( "open", Input.Down( InputButton.Score ) );
	}
}
