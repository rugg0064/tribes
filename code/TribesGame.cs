
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
		[Net]
		public ScoreStruct score { get; set; }
		[Net]
		public List<String> names { get; set; }
		[Net]
		public List<TribesScoreboardStruct> nameData { get; set; }
		private Dictionary<String, int> playerIndicies;
		[Net]
		public int mainSeed { get; set; }

		private int teamNumbers;
		private Random random;

		private Vector3 position = new Vector3(-2053.71f,-1662.20f,-31.97f);
		public TribesTerrain terrain;

		public TribesFlag redFlag;
		public TribesFlag bluFlag;
		
		//public int mainSeed;
		public TribesGame()
		{
			if ( IsServer )
			{
				this.names = new List<String>();
				this.nameData = new List<TribesScoreboardStruct>();
				this.playerIndicies = new Dictionary<string, int>();
				this.random = new Random();
				this.mainSeed = random.Next();
				this.score = new ScoreStruct();

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


			redFlag = new TribesFlag( "addons/rust/models/rust_props/ladder_set/ladder_300.vmdl" );
			redFlag.SetupPhysicsFromModel( PhysicsMotionType.Static );
			//redFlag.Spawn();

			bluFlag = new TribesFlag( "addons/rust/models/rust_props/ladder_set/ladder_300.vmdl" );
			bluFlag.SetupPhysicsFromModel( PhysicsMotionType.Static );
			//bluFlag.Spawn();

			returnRedFlag();
			returnBluFlag();
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			TribesPlayer player = new TribesPlayer();
			client.Pawn = player;
			player.generateTerrain(this.mainSeed, position);
			//If teams are equal, pick a random number, else pick the team which would approach equality
			player.team = teamNumbers == 0 ? random.Next(0,2) == 0 : teamNumbers < 0;
			teamNumbers += player.team ? 1 : -1;

			player.Respawn();
				
			if(playerIndicies.ContainsKey (client.Name ))
			{
				int index = playerIndicies[client.Name];
				nameData[index] = new TribesScoreboardStruct( player.team );
			}
			else
			{
				names.Add( client.Name );
				nameData.Add( new TribesScoreboardStruct( player.team ) );
				playerIndicies.Add( client.Name, names.Count - 1 );
			}

			//ModelEntity t = new ModelEntity( "addons/rust/models/rust_props/ladder_set/ladder_300.vmdl" );
			//t.Spawn();
			//t.Position = player.Position;
			//dAt.Parent = player;
		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			base.ClientDisconnect( cl, reason );
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
		}

		public void returnRedFlag()
		{
			TribesTerrain terrain = this.terrain;
			int size = terrain.vertSize;
			int size1x = (int)(size * 0.1f);
			Vector3 pos = terrain.getPos( size1x, size1x );
			redFlag.Position = pos;
			bluFlag.stopFollowingBone();
		}

		public void returnBluFlag()
		{
			TribesTerrain terrain = this.terrain;
			int size = terrain.vertSize;
			int size2x = (int)(size * 0.9f);
			Vector3 pos = terrain.getPos( size2x, size2x );
			bluFlag.Position = pos;
			bluFlag.stopFollowingBone();
		}

		public void returnFlag(TribesFlag tfg)
		{
			if(tfg == bluFlag)
			{
				returnBluFlag();
			}
			else
			{
				returnRedFlag();
			}
		}

		public void returnFlag(TribesPlayer p)
		{
			if ( p.flag != null )
			{
				returnFlag( p.flag );
				p.flag = null;
			}
		}

		public void givePoint(bool team)
		{
			if(team)
			{
				score = new ScoreStruct( score.red + 1, score.blu );
			}
			else
			{
				score = new ScoreStruct( score.red, score.blu + 1 );
			}
		}

		public void givePoint( TribesPlayer player)
		{
			returnFlag( player );
			givePoint( player.team );
		}
	}
}
