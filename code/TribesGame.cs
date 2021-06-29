
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
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
		public Prop platform1;
		public Prop platform2;

		[Net]
		public List<TribesScoreboardStruct> playerScores { get; private set; }
		[Net]
		public int redScore { get; private set; }
		[Net]
		public int bluScore { get; private set; }
		[Net]
		public int mainSeed { get; private set; }

		private int teamNumbers;
		private Random random;

		private Vector3 position = new Vector3(-4096,-4096,-1024);
		public TribesTerrain<ModifiedNoise<StackedPerlin>> terrain { get; private set; }

		public TribesFlag redFlag;
		public TribesFlag bluFlag;
		
		public ModifiedNoise<StackedPerlin> noise;
		//public int mainSeed;
		public TribesGame()
		{
			if( IsServer)
			{
				this.random = new Random();
				this.mainSeed = random.Next();

				StackedPerlin perlin = new StackedPerlin( this.mainSeed, new PerlinLayerData[]
				{
					new PerlinLayerData(32, 0.70f),
					new PerlinLayerData(64, 0.30f),
					//new PerlinLayerData(16,  0.10f),
					//new PerlinLayerData(8,   0.10f)
				} );
				noise = new ModifiedNoise<StackedPerlin>( perlin );
				List<Func<float, float>> list = new List<Func<float, float>>();
				list.Add( ( f ) => { return f * f; } );
				list.Add( ( f ) => { return f - 0.1f; } );
				//list.Add( ( f ) => { return MathF.Max(0.0f + MathF.Abs(f / 100), f); } );
				list.Add( ( f ) => { return MathF.Max( 0.0f + MathF.Abs( f / 1000000f ), f ); } );
				noise.modifierList = list;
			}

			

			if ( IsServer )
			{
				this.playerScores = new List<TribesScoreboardStruct>();
				
				this.redScore = this.bluScore = 0;

				new TribesUI();
			}

			if ( IsClient )
			{

			}
		}

		public override void PostLevelLoaded()
		{
			terrain = new TribesTerrain<ModifiedNoise<StackedPerlin>>( position , noise);

			redFlag = new TribesFlag( "addons/rust/models/rust_props/ladder_set/ladder_300.vmdl" );
			redFlag.SetupPhysicsFromModel( PhysicsMotionType.Static );

			bluFlag = new TribesFlag( "addons/rust/models/rust_props/ladder_set/ladder_300.vmdl" );
			bluFlag.SetupPhysicsFromModel( PhysicsMotionType.Static );
			
			int size = terrain.vertSize;
			int size1x = (int)(size * 0.1f);
			int size2x = (int)(size * 0.9f);

			float rollMod = 90.0f;

			Prop p1 = new Prop();
			platform1 = p1;
			p1.SetModel( "models/platform2/platform.vmdl" );
			p1.Spawn();
			p1.SetupPhysicsFromModel( PhysicsMotionType.Static, false );
			p1.Position = this.terrain.getPos( size1x, size1x );

			Vector3 normal1 = terrain.getNormal( size1x, size1x );

			Angles angles1 = normal1.EulerAngles;
			angles1.pitch += rollMod;
			p1.Rotation = Rotation.From( angles1 );


			Prop p2 = new Prop();
			platform2 = p2;
			p2.SetModel( "models/platform2/platform.vmdl" );
			p2.Spawn();
			p2.SetupPhysicsFromModel( PhysicsMotionType.Static, false );
			p2.Position = this.terrain.getPos( size2x, size2x );

			Vector3 normal2 = terrain.getNormal( size2x, size2x );
			Angles angles2 = normal2.EulerAngles;
			angles2.pitch += rollMod;
			p2.Rotation = Rotation.From( angles2 );

			returnRedFlag();
			returnBluFlag();
		}

		[Event.Tick.Server]
		private void thing()
		{
			TribesPlayer[] players = All.OfType<TribesPlayer>().ToArray();
			for ( int i = 0; i < players.Length; i++ )
			{
				TribesPlayer player = players[i];
				if ( player.flag != null )
				{
					Prop platform = player.team ? platform1 :  platform2;
					//Log.Info( platform.Position.Distance( player.Position ) );
					if(platform.Position.Distance(player.Position) < 640)
					{
						givePoint( player );
						TribesFlag f = (player.team ? redFlag : bluFlag);
						f.stopFollowingBone();
						returnFlag( f );
						//returnFlag( player.flag );
						player.flag = null;
					}
				}
			}
		}

		public override void ClientJoined( Client client )
		{
			Log.Info( client.SteamId );
			base.ClientJoined( client );

			TribesPlayer player = new TribesPlayer();
			client.Pawn = player;
			player.generateTerrain(position);
			//If teams are equal, pick a random number, else pick the team which would approach equality
			player.team = teamNumbers == 0 ? random.Next(0,2) == 0 : teamNumbers < 0;
			teamNumbers += player.team ? 1 : -1;

			player.Respawn();
				
			playerScores.Add( new TribesScoreboardStruct( client.NetworkIdent, player.team ) );

			player.generateTerrain( position );
		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			base.ClientDisconnect( cl, reason );
			bool found = false;
			for(int i = 0; i < playerScores.Count && !found; i++)
			{
				if(playerScores[i].netID == cl.NetworkIdent)
				{
					playerScores.RemoveAt( i );
					found = true;
				}
			}
			//int index = playerIndicies[cl.Name];
			//playerNames.RemoveAt( index );
			//playerScores.RemoveAt( index );
			//playerIndicies.Remove( cl.Name );
		}

		public void gotDied( Client c )
		{
			bool found = false;
			for ( int i = 0; i < playerScores.Count && !found; i++ )
			{
				if ( playerScores[i].netID == c.NetworkIdent )
				{
					found = true;
					Log.Info( "dying" );
					playerScores[i] = playerScores[i].giveDeath();
				}
			}
		}

		public void gotKilled( Client c )
		{
			bool found = false;
			for ( int i = 0; i < playerScores.Count && !found; i++ )
			{
				if ( playerScores[i].netID == c.NetworkIdent )
				{
					found = true;
					Log.Info( "dying" );
					playerScores[i] = playerScores[i].giveKill();
				}
			}
		}


		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
		}

		public void returnRedFlag()
		{
			TribesTerrain<ModifiedNoise<StackedPerlin>> terrain = this.terrain;
			int size = terrain.vertSize;
			int size1x = (int)(size * 0.1f);
			Vector3 pos = terrain.getPos( size1x, size1x );
			redFlag.Position = pos;
			bluFlag.stopFollowingBone();
		}

		public void returnBluFlag()
		{
			TribesTerrain<ModifiedNoise<StackedPerlin>> terrain = this.terrain;
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
				redScore++;
			}
			else
			{
				bluScore++;
			}
		}

		public void givePoint( TribesPlayer player)
		{
			returnFlag( player );
			givePoint( player.team );
		}
	}
}
