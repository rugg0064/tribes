using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Tribes
{
	public partial class TribesPlayer : Player
	{
		private Particles particleL;
		private Particles particleR;
		private bool drawParticle;
		private DamageInfo lastDamageInfo;

		[Net]
		public bool team{get; set;}
		[Net, Local]
		public float ammo {get;set;} = 100;
		public TribesFlag flag;
		public TribesPlayer()
		{
			//this.Controller = new TribesWalkController();
			this.Inventory = new Inventory(this);
			drawParticle = false;
		}
		public override void Respawn()
		{
			//Log.Info("RESPAWNING: " + (IsServer ? "SERVER" : "CLIENT"));
			
			//Log.Info(TribesGame.mainSeed);
		
			this.ammo = 100.0f;
			
			SetModel("models/citizen/citizen.vmdl");
			//this.Controller = new TribesWalkController();
			this.Controller = new TribesWalkController();
			this.Animator = new StandardPlayerAnimator();
			this.Camera = new ThirdPersonCamera();
			this.EnableAllCollisions = true;
			this.EnableDrawing = true;
			this.EnableHideInFirstPerson = true;
			this.EnableShadowInFirstPerson = true;

			base.Respawn();
			//setSliding( false );

			TribesTerrain<ModifiedNoise<StackedPerlin>> terrain = ((TribesGame)Game.Current).terrain;
			bool team = this.team;
			int flagCenter = (int)(terrain.vertSize * (team ? 0.1f : 0.9f));

			float randDistance = 2.0f + Rand.Float( 0, 2.0f );

			Vector2 randVector = Utils.randomVector2( new Random().Next() ).Normal * randDistance;
			int x = (int) randVector.x;
			int y = (int) randVector.y;
			Vector3 pos = terrain.getPos( flagCenter + x, flagCenter + y) + new Vector3(0,0,64);

			this.Position = pos;
			if (this.Inventory != null)
			{
				Inventory.DeleteContents();
				Pistol pistol;
				Inventory.Add( pistol = new Pistol(), true );
			}
			
		}

		public override void CreateHull()
		{
			CollisionGroup = CollisionGroup.Player;
			AddCollisionLayer( CollisionLayer.Player );
			//SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );

			var capsule = new Capsule( new Vector3( 0, 0, 16 ), new Vector3( 0, 0, 72 - 16 ), 16 );
			var phys = SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, capsule );

			//	phys.GetBody(0).RemoveShadowController();

			// TODO - investigate this? if we don't set movetype then the lerp is too much. Can we control lerp amount?
			// if so we should expose that instead, that would be awesome.
			MoveType = MoveType.MOVETYPE_WALK;
			EnableHitboxes = true;
		}


		[Event.Frame]
		private void doThing()
		{
			if ( particleL == null )
			{
				particleL = Particles.Create( "particles/myFancyParticles" );
			}

			if ( particleR == null )
			{
				particleR = Particles.Create( "particles/myFancyParticles" );
			}

			Vector3 posL;
			Vector3 posR;
			if(drawParticle)
			{
				posL = GetBoneTransform( GetBoneIndex( "ankle_L" ), true ).Position;
				posR = GetBoneTransform( GetBoneIndex( "ankle_R" ), true ).Position;
			}
			else
			{
				posL = posR = Vector3.Zero;
			}
			particleL.SetPosition( 0, posL );
			particleR.SetPosition( 0, posR );
		}

		public override void TakeDamage( DamageInfo info )
		{
			base.TakeDamage( info );
			lastDamageInfo = info;
		}
		
		private void setSliding(bool isSliding)
		{
			TribesWalkController c = (TribesWalkController) this.Controller;
			if (isSliding)
			{
				c.GroundAngle = -999999f;
				c.MoveFriction = c.slidingMoveFriction;
				c.GroundFriction = c.slidingMoveFriction;
			}
			else
			{
				c.GroundAngle = 42f;
				c.MoveFriction = c.normalMoveFriction;
				c.GroundFriction = c.normalGroundFriction;
			}
		}


		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );

			if (Input.Pressed(InputButton.Run) || Input.Released(InputButton.Run))
			{
				this.setSliding( Input.Down( InputButton.Run ) );
			}

			drawParticle = Input.Down( InputButton.Attack2 );

			if ( Input.Down(InputButton.Attack2) && this.GroundEntity == null && ammo > 0)
			{
				this.Velocity += (new Vector3(0,0,1) * 750 * 1.5f) * Time.Delta;

				this.Velocity += this.Velocity.WithZ( 0 ).Normal * 64 * Time.Delta;

				ammo -= 33 * 1.25f * Time.Delta;
				ammo = Math.Max(0, ammo);
			}
			else
			{
				if(ammo < 100)
				{
					ammo += Math.Min(100 - ammo, 10f * Time.Delta);
				}
			}
			if( Input.Pressed(InputButton.Attack1) )
			{
				Vector3 position = EyePos + (EyeRot.Forward * 128);
				Perlin p = new Perlin( new Random().Next(), 8 );
				float max = -9999f;
				float min= 9999f;
				for ( int i = 0; i < 64; i++ )
				{
					for ( int j = 0; j < 64; j++ )
					{
						float v = p.getHeight( i, j );
						max = MathF.Max( max, v );
						min = MathF.Min( min, v );
						//Log.Info( v );
						//DebugOverlay.Sphere( position + new Vector3( i, j, 0 ) * 64, (((float)Math.Abs( v )) * 50) + 0, Color.White, false, 10f );
					}
				}
				Log.Info( min );
				Log.Info( max );
			}
		}

		public override void OnKilled()
		{
			//Log.Info( "died!" );
			base.OnKilled();
			
			Inventory.DeleteContents();
			EnableDrawing = false;

			if(this.flag != null)
			{
				TribesGame cur = ((TribesGame)Game.Current);
				cur.returnFlag( this.flag );
			}
			TribesGame cur2 = ((TribesGame)Game.Current);
			//Log.Info( IsServer );
			cur2.gotDied( this.GetClientOwner() );
			cur2.gotKilled( lastDamageInfo.Attacker.GetClientOwner() );
		}



		public override void StartTouch( Entity other )
		{
			
			TribesGame cur = ((TribesGame)Game.Current);
			base.StartTouch( other );
			if (other is TribesFlag)
			{
				TribesFlag collisionFlag = (TribesFlag)other;
				TribesFlag myFlag = this.team ? cur.redFlag : cur.bluFlag;
				TribesFlag otherFlag = this.team ? cur.bluFlag : cur.redFlag;

				if(collisionFlag != myFlag)
				{
					this.flag = collisionFlag;
					collisionFlag.followPlayer( this );
				}
			}
		}


		[ClientRpc]
		public void generateTerrain( Vector3 pos )
		{
			int mainSeed = ((TribesGame)Game.Current).mainSeed;
			StackedPerlin perlin = new StackedPerlin( mainSeed, new PerlinLayerData[]
			{
				new PerlinLayerData(32, 0.70f),
				new PerlinLayerData(64, 0.30f),
				//new PerlinLayerData(16,  0.10f),
				//new PerlinLayerData(8,   0.10f)
			} );
			ModifiedNoise <StackedPerlin> noise = new ModifiedNoise<StackedPerlin>( perlin );
			List<Func<float, float>> list = new List<Func<float, float>>();
			list.Add( ( f ) => { return f * f; } );
			list.Add( ( f ) => { return f - 0.1f; } );
			//list.Add( ( f ) => { return MathF.Max(0.0f + MathF.Abs(f / 100), f); } );
			list.Add( ( f ) => { return MathF.Max( 0.0f + MathF.Abs( f / 1000000f ), f ); } );
			noise.modifierList = list;

			var x = new TribesTerrain<ModifiedNoise<StackedPerlin>>( pos, noise );
		}

	}
}
