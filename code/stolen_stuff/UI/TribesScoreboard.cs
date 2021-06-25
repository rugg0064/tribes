using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections;
using System.Collections.Generic;
using Tribes;

public class TribesScoreboard : Panel
{
	//public Panel header;
	public Panel canvas; //The center shaded region, with alignment settings
	List<TribesScoreboardObject> entryList;
	private IList<String> cachedNames;
	public TribesScoreboard()
	{
		cachedNames = getList();
		entryList = new List<TribesScoreboardObject>();
		StyleSheet.Load( "/stolen_stuff/UI/Scoreboard.scss" ); 
		AddClass( "scoreboard" );
		//With the TribesScoreboard panel being a scoreboard class, any children will have common text settings.
		//Every component is then added to the canvas, which is the centered shaded part.

		canvas = Add.Panel( "canvas" );

		/*
		for ( int i = 0; i < 32; i++ )
		{
			Panel p1 = new Panel();
			p1.SetClass( "entryPanel", true );
			p1.Add.Label( "User" + (i+1) );
			TribesScoreboardObject o1 = new TribesScoreboardObject( 0, p1 );
			addObject( o1 );
		}
		*/

		/*
		Panel p1 = new Panel();
		p1.SetClass( "entryPanel", true);
		p1.Add.Label( "User1" );
		TribesScoreboardObject o1 = new TribesScoreboardObject( 0, p1 );
		
		Panel p2 = new Panel();
		p2.SetClass( "entryPanel", true);
		p2.Add.Label( "User2" );
		TribesScoreboardObject o2 = new TribesScoreboardObject( 0, p2 );

		addObject( o1 );
		addObject( o2 );
		*/

	}

	private IList<String> getList()
	{
		return ((TribesGame)Game.Current).names;
	}

	public void printList<T>( IList<T> l )
	{
		for ( int i = 0; i < l.Count; i++ )
		{
			Log.Info( l[i] );
		}
	}

	private void buildFromList()
	{
		IList<String> names = getList(); 
		entryList.Clear();
		for ( int i = 0; i < names.Count; i++ )
		{
			Panel p1 = new Panel();
			p1.SetClass( "entryPanel", true );
			p1.Add.Label( names[i] );
			TribesScoreboardObject o1 = new TribesScoreboardObject( 0, p1 );
			addObject( o1 );
		}
	}

	public override void Tick()
	{
		buildFromList();
		/*
		IList<String> newList;
		//Log.Info( "Tick" );
		if(cachedNames != (newList = getList()) )
		{
			Log.Info( "Not the same!" );
			cachedNames = newList;
			entryList.Clear();
			for ( int i = 0; i < cachedNames.Count; i++ )
			{
				Panel p1 = new Panel();
				p1.SetClass( "entryPanel", true );
				p1.Add.Label( cachedNames[i] );
				TribesScoreboardObject o1 = new TribesScoreboardObject( 0, p1 );
				addObject( o1 );
			}
		}
		*/

		base.Tick();
		SetClass( "open", Input.Down( InputButton.Score ) );
	}

	public void addObject( TribesScoreboardObject scoreObj )
	{
		int i = 0;
		while(i < entryList.Count && scoreObj.priority <= entryList[i].priority)
		{
			i++;
		}
		entryList.Insert( i, scoreObj );
		reorder();
	}

	//ASSUMES LIST IS ORDERED
	public void reorder()
	{
		//entryList.Sort(new TribesScoreboardObjectComparer());
		canvas.DeleteChildren( true );
		for(int i = 0; i < entryList.Count; i++)
		{
			canvas.AddChild( entryList[i].panel );
		}
	}
}

public class TribesScoreboardObject
{
	public int priority;
	public Panel panel;

	public TribesScoreboardObject( int priority, Panel panel)
	{
		this.priority = priority;
		this.panel = panel;
	}
}

public class TribesScoreboardObjectComparer : System.Collections.Generic.Comparer<TribesScoreboardObject>
{
	public TribesScoreboardObjectComparer()
	{

	}

	public override int Compare( TribesScoreboardObject x, TribesScoreboardObject y )
	{
		return x.priority - y.priority;
	}
}
