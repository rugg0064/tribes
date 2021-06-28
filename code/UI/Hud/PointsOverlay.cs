using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections;
using System.Collections.Generic;
using Tribes;

public class PointsOverlay : Panel
{
	Panel thePanel;
	Label red;
	Label blu;
	public PointsOverlay()
	{
		AddClass( "pointsOverlay" );
		thePanel = Add.Panel( "container" );
		red = thePanel.Add.Label( "0", "red" );
		blu = thePanel.Add.Label( "0", "blu" );

	}
	public override void Tick()
	{
		ScoreStruct curScore = ((TribesGame)Game.Current).score;
		red.Text = curScore.red.ToString();
		blu.Text = curScore.blu.ToString();
	}
}
