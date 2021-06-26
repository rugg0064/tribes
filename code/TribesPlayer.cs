using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Tribes
{
	public partial class TribesPlayer : Player
	{
		[Net]
		public bool team{get; set;}
		[Net, Local]
		public float ammo {get;set;} = 100;
		public TribesFlag flag;
		public TribesPlayer()
		{
			this.Inventory = new Inventory(this);
		}
		public override void Respawn()
		{
			Log.Info("RESPAWNING: " + (IsServer ? "SERVER" : "CLIENT"));
			
			//Log.Info(TribesGame.mainSeed);
		
			this.ammo = 100.0f;
			
			SetModel("models/citizen/citizen.vmdl");
			this.Controller = new TribesWalkController();
			this.Animator = new StandardPlayerAnimator();
			this.Camera = new ThirdPersonCamera();
			this.EnableAllCollisions = true;
			this.EnableDrawing = true;
			this.EnableHideInFirstPerson = true;
			this.EnableShadowInFirstPerson = true;

			base.Respawn();
			setSliding( false );

			TribesTerrain terrain = ((TribesGame)Game.Current).terrain;
			int centerPos = terrain.vertSize / 2;
			Vector3 pos = terrain.getPos( centerPos, centerPos ) + new Vector3(0,0,64);

			Log.Info( "Moving to: " + pos );
			//base.MoveTo( pos, 0.0f );
			this.Position = pos;
			if (this.Inventory != null)
			{
				Inventory.DeleteContents();
				Pistol pistol;
				Inventory.Add( pistol = new Pistol(), true );
				//Log.Info(pistol.PrimaryRate);
			}
		}

		private void setSliding(bool isSliding)
		{
			TribesWalkController c = (TribesWalkController)this.Controller;
			if (isSliding)
			{
				c.MoveFriction = c.slidingMoveFriction;
				c.GroundFriction = c.slidingMoveFriction;
			}
			else
			{
				c.MoveFriction = c.normalMoveFriction;
				c.GroundFriction = c.normalGroundFriction;
			}
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );

			if(Input.Pressed(InputButton.Run) || Input.Released(InputButton.Run))
			{
				this.setSliding( Input.Down( InputButton.Run ) );
			}

			/*MyWalkController c = (MyWalkController) this.Controller;
			if(Input.Pressed(InputButton.Run))
			{
				c.MoveFriction = c.slidingMoveFriction;
				c.GroundFriction = c.slidingMoveFriction;
			}
			if(Input.Released(InputButton.Run))
			{
				c.MoveFriction = c.normalMoveFriction;
				c.GroundFriction = c.normalGroundFriction;
			}*/

			if ( Input.Down(InputButton.Attack2) && this.GroundEntity == null && ammo > 0)
			{
				this.Velocity += (new Vector3(0,0,1) * 750) * Time.Delta;
				ammo -= 33 * Time.Delta;
				ammo = Math.Max(0, ammo);
				
			}
			else
			{
				if(ammo < 100)
				{
					ammo += Math.Min(100 - ammo, 15f * Time.Delta);
				}
			}

			if( Input.Pressed(InputButton.Attack1) )
			{
				//Log.Info(this.Position);
				//Log.Info( this.team );
				//MinimalGame.doThing(this.EyePos + (this.EyeRot.Forward * 100));
				//((MinimalGame) this.).doThing(Vector3.Zero);
				//Log.Info(this.mainSeed);
				//MinimalGame.doThing(new Vector3(-400, -250, 200));
				//Log.Info(Position);

				/*
				IList<TribesScoreboardStruct> data = ((TribesGame)Game.Current).nameData;
				IList<String> names = ((TribesGame)Game.Current).names;
				for (int i = 0; i < names.Count; i++ )
				{
					Log.Info( names[i].ToString() + " : " + data[i].ToString() );
				}
				*/
				ScoreStruct score = ((TribesGame)Game.Current).score;
				Log.Info( "RED: " + score.red + ", BLU:  " + score.blu );
			}
		}

		public override void OnKilled()
		{
			base.OnKilled();
			
			Inventory.DeleteContents();
			EnableDrawing = false;
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );
			Log.Info( "Touched" );
			TribesGame cur = ((TribesGame)Game.Current);
			Log.Info( cur.redFlag );

			if(other is TribesFlag)
			{
				TribesFlag collisionFlag = (TribesFlag)other;
				TribesFlag myFlag = this.team ? cur.redFlag : cur.bluFlag;
				TribesFlag otherFlag = this.team ? cur.bluFlag : cur.redFlag;

				if(collisionFlag == myFlag)
				{
					if(this.flag != null)
					{
						cur.givePoint( this );
					}
				}
				else
				{
					this.flag = collisionFlag;
					collisionFlag.followPlayer( this );
					
				}
				/*
				if ( (otherTFG == cur.redFlag && this.team == false) || (otherTFG == cur.bluFlag && this.team == true))
				{
					otherTFG.followBone( this, this.GetBoneIndex( "spine_2" ) );
				}
				*/
			}
		}


		[ClientRpc]
		public void generateTerrain( int seed, Vector3 pos )
		{
			TribesTerrain x = new TribesTerrain( seed, pos );
		}

	}
}
