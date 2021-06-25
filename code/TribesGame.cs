
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Tribes
{
	/// <summary>
	/// This is your game class. This is an entity that is created serverside when
	/// the game starts, and is replicated to the client. 
	/// 
	/// You can use this to create things like HUDs and declare which player class
	/// to use for spawned players.
	/// 
	/// Your game needs to be registered (using [Library] here) with the same name 
	/// as your game addon. If it isn't then we won't be able to find it.
	/// </summary>
	[Library( "minimal" )]
	public partial class TribesGame : Sandbox.Game
	{
		private int mainSeed;

		private Vector3 position = new Vector3(-2053.71f,-1662.20f,-31.97f);
		public TribesTerrain terrain;
		
		//public int mainSeed;
		public TribesGame()
		{
			if ( IsServer )
			{
				this.mainSeed = new Random().Next();

				// Create a HUD entity. This entity is globally networked
				// and when it is created clientside it creates the actual
				// UI panels. You don't have to create your HUD via an entity,
				// this just feels like a nice neat way to do it.
				new DeathmatchHud();
				new TribesCrosshairHud();
			}

			if ( IsClient )
			{

			}
		}

		public override void PostLevelLoaded()
		{
			this.terrain = new TribesTerrain(mainSeed, position);	
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			MinimalPlayer player = new MinimalPlayer();
			//player.mainSeed = TribesGame.mainSeed;
			client.Pawn = player;
			player.generateTerrain(this.mainSeed, position);
			bool team = true;
			//bool team = new Random().Next( 0, 1 ) == 0;
			Log.Info( team + "ASDASDASDASDS" );
			player.team = team;
			player.Respawn();
		}
	}

}
