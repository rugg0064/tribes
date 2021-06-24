using Sandbox;
using System;
using System.Linq;

namespace MinimalExample
{
	partial class MinimalPlayer : Player
	{
		[Net]
		public int mainSeed {get; set;}
		[Net, Local]
		public float ammo {get;set;} = 100; 
		public MinimalPlayer()
		{
			this.Inventory = new Inventory(this);
			
		}
		public override void Respawn()
		{
			ammo = 100.0f;
			
			SetModel("models/citizen/citizen.vmdl");
			Controller = new MyWalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new ThirdPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
			
			if(this.Inventory != null)
			{
				Inventory.DeleteContents();
				Pistol pistol;
				Inventory.Add( pistol = new Pistol(), true );
				//Log.Info(pistol.PrimaryRate);
			}
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );


			MyWalkController c = (MyWalkController) this.Controller;
			if(Input.Pressed(InputButton.Run))
			{
				c.MoveFriction = c.slidingMoveFriction;
				c.GroundFriction = c.slidingMoveFriction;
			}
			if(Input.Released(InputButton.Run))
			{
				c.MoveFriction = c.normalMoveFriction;
				c.GroundFriction = c.normalGroundFriction;
			}
		
			if(Input.Down(InputButton.Attack2) && this.GroundEntity == null && ammo > 0)
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
				//MinimalGame.doThing(this.EyePos + (this.EyeRot.Forward * 100));
				//((MinimalGame) this.).doThing(Vector3.Zero);
				//Log.Info(this.mainSeed);
				//MinimalGame.doThing(new Vector3(-400, -250, 200));
				Log.Info(Position);
			}
		}

		public override void OnKilled()
		{
			base.OnKilled();
			
			Inventory.DeleteContents();
			EnableDrawing = false;
		}
	}
}
