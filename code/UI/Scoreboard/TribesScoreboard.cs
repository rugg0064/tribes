using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections;
using System.Collections.Generic;
using Tribes;

public class TribesScoreboard : Panel
{

	private IList<TribesScoreboardStruct> playerScores;
	private Dictionary<int, Panel> netIDToPanel;
	private Label totalPlayersLabel;
	private Label seedLabel;
	private Panel scores;

	public TribesScoreboard()
	{
		StyleSheet.Load( "/UI/Scoreboard/TribesScoreboard.scss" ); 
		AddClass( "scoreboard" );

		Panel canvas = Add.Panel( "canvas" );

		Panel header = canvas.Add.Panel( "header" );
		Label gamemode = header.Add.Label( "Tribes", "gamemode" );
		//((TribesGame)(game.current)).mainSeed.ToString()
		seedLabel = header.Add.Label( "Seed: XXXXXXXXXXX", "map" );
		totalPlayersLabel = header.Add.Label( "Total Players: X", "totalPlayers" );
		Label timeLeft = header.Add.Label( "Time Left: 00:00", "timeLeft" );

		Panel content = canvas.Add.Panel( "content" );
		Panel legend = content.Add.Panel( "legend" );
		legend.Add.Label( "Name", "name" );
		legend.Add.Label( "Kills" , "kills" );
		legend.Add.Label( "Deaths", "deaths" );
		legend.Add.Label( "Reserved", "reserved" );
		scores = content.Add.Panel( "scores" );


		//nameToPanel = new Dictionary<string, Panel>();
		//playerNames = new List<string>();
		//playerScores = ((TribesGame)(Game.Current)).playerScores;
		playerScores = new List<TribesScoreboardStruct>();
		netIDToPanel = new Dictionary<int, Panel>();
	}

	public override void Tick()
	{
		SetClass( "open", Input.Down( InputButton.Score ) );
		seedLabel.Text = "Seed: " + ((TribesGame)(Game.Current)).mainSeed.ToString();
		totalPlayersLabel.Text = "Total Players: " + Client.All.Count.ToString();



		IList <TribesScoreboardStruct> curScores = ((TribesGame)(Game.Current)).playerScores;
		(List<TribesScoreboardStruct> left, List<TribesScoreboardStruct> joined) changedScores = whatChanged( playerScores, curScores );

		List<TribesScoreboardStruct> list = new List<TribesScoreboardStruct>();
		foreach ( TribesScoreboardStruct s in curScores )
		{
			list.Add( s );
		}
		playerScores = list;
		
		foreach ( TribesScoreboardStruct s in changedScores.joined )
		{
			Panel p = scores.Add.Panel( "score" );
			p.Add.Label( getNameFromNetID(s.netID), "name" );
			p.Add.Label( "0", "kills" );
			p.Add.Label( "0", "deaths" );
			p.Add.Label( "R", "reserved" );
			//nameToPanel[s] = p;
			netIDToPanel[s.netID] = p;
		}
		foreach(TribesScoreboardStruct s in changedScores.left )
		{
			Log.Info( "SOMEONE LEFT: " + s + " : " + getNameFromNetID(s.netID));
			netIDToPanel[s.netID].Delete();
		}
		//RN assume height = 10%
		List<TribesScoreboardStruct> sorted = sortList( playerScores );

		for ( int i = 0; i < sorted.Count; i++ )
		{
			//float percentShouldBe = ((9.93f * i) );
			float percentShouldBe = ((10.5f * i) );
			Panel p = netIDToPanel[sorted[i].netID];
			Length? l = p.Style.Top;
			float curPercent = l.HasValue ? l.Value.Value : 0.0f;
			float changeAmount = curPercent - percentShouldBe;
			if(MathF.Abs(changeAmount) < 0.25f)
			{
				p.Style.Set( "top", percentShouldBe.ToString() + "%" );
			}
			else
			{
				p.Style.Set( "top", MathX.LerpTo( curPercent, percentShouldBe, 2.5f * Time.Delta ).ToString() + "%" );
			}

			((Label)p.GetChild( 1 )).Text = sorted[i].kills.ToString();
			((Label)p.GetChild( 2 )).Text = sorted[i].deaths.ToString();
		}

		/*
		IList<TribesScoreboardStruct> playerScores = ( (TribesGame)(Game.Current)).playerScores;
		for(int i = 0; i < playerScores.Count; i++ )
		{
			Panel p = nameToPanel[curPlayers[i]];
			((Label)p.GetChild( 0 )).Text = curPlayers[i];
			((Label)p.GetChild( 1 )).Text = playerScores[i].kills.ToString();
			((Label)p.GetChild( 2 )).Text = playerScores[i].deaths.ToString();
		}
		*/
	}

	private List<TribesScoreboardStruct> sortList(IList<TribesScoreboardStruct> list)
	{
		List<TribesScoreboardStruct> newList = new List<TribesScoreboardStruct>( list );
		for(int i = 0; i < newList.Count; i++)
		{
			for ( int j = i + 1; j < newList.Count; j++ )
			{
				{
					if(newList[i].kills < newList[j].kills)
					{
						TribesScoreboardStruct temp = newList[i];
						newList[i] = newList[j];
						newList[j] = temp;
					}
				}
			}
		}
		return newList;
	}

	private string getNameFromNetID(int netID)
	{
		IReadOnlyList<Client> clients = Client.All;
		for(int i = 0; i < clients.Count; i++)
		{
			if(clients[i].NetworkIdent == netID)
			{
				return clients[i].Name;
			}
		}
		return null;
	}

	//Terrible efficiency!
	private (List<TribesScoreboardStruct> list1Exclusive, List<TribesScoreboardStruct> list2Exclusive) whatChanged(IList<TribesScoreboardStruct> list1, IList<TribesScoreboardStruct> list2)
	{
		List<TribesScoreboardStruct> list1Exclusive = new List<TribesScoreboardStruct>();
		List<TribesScoreboardStruct> list2Exclusive = new List<TribesScoreboardStruct>();
		
		for ( int i = 0; i < list1.Count; i++ )
		{
			bool found = false;
			for(int j = 0; j < list2.Count && !found; j++ )
			{
				//if (list1[i].Equals(list2[j]))
				if(list1[i].netID == list2[j].netID)
				{
					found = true;
				}
			}
			if(!found)
			{
				list1Exclusive.Add( list1[i] );
			}
		}

		for ( int i = 0; i < list2.Count; i++ )
		{
			bool found = false;
			for ( int j = 0; j < list1.Count && !found; j++ )
			{
				//if (list2[i].Equals(list1[j]))
				if(list2[i].netID == list1[j].netID)
				{
					found = true;
				}
			}
			if(!found)
			{
				list2Exclusive.Add( list2[i] );
			}
		}

		return (list1Exclusive, list2Exclusive);
	}
}
