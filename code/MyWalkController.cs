
using Sandbox;

namespace Tribes
{
    class MyWalkController : Sandbox.WalkController
    {
        //public override float GroundFriction = 0;
        //public override float MoveFriction = 0;

        public float slidingGroundFriction = 0.1f;
        public float slidingMoveFriction = 0.1f;
        public float normalGroundFriction = 4f;
        public float normalMoveFriction = 1f;

        public MyWalkController() : base()
		{
            GroundFriction = 0.5f;
            MoveFriction = 0.5f;
            MaxNonJumpVelocity = 50f;
		}
        
        /*
        public override void ApplyFriction( float frictionAmount = 1.0f )
        {
            //Log.Info(GroundFriction + " " + MoveFriction);
        }
        */
        
        public override void WalkMove()
		{
			var wishdir = WishVelocity.Normal;
			var wishspeed = WishVelocity.Length;

			WishVelocity = WishVelocity.WithZ( 0 );
			WishVelocity = WishVelocity.Normal * wishspeed;

			Velocity = Velocity.WithZ( 0 );

			Accelerate( wishdir, wishspeed, 0, Acceleration );
            Velocity = Velocity.WithZ( 0 );
            Velocity = Velocity.ClampLength(1200);

			//   Player.SetAnimParam( "forward", Input.Forward );
			//   Player.SetAnimParam( "sideward", Input.Right );
			//   Player.SetAnimParam( "wishspeed", wishspeed );
			//    Player.SetAnimParam( "walkspeed_scale", 2.0f / 190.0f );
			//   Player.SetAnimParam( "runspeed_scale", 2.0f / 320.0f );

			//  DebugOverlay.Text( 0, Pos + Vector3.Up * 100, $"forward: {Input.Forward}\nsideward: {Input.Right}" );
             
            
            //DebugOverlay.ScreenText(new Vector2(50,50), 1, new Color(0,250,0), "ASD", 1.0f);
			// Add in any base velocity to the current velocity.
			Velocity += BaseVelocity;

			try
			{
				if ( Velocity.Length < 1.0f )
				{
					Velocity = Vector3.Zero;
					return;
				}

				// first try just moving to the destination	
				var dest = (Position + Velocity * Time.Delta).WithZ( Position.z );

				var pm = TraceBBox( Position, dest );

				if ( pm.Fraction == 1 )
				{
					Position = pm.EndPos;
					StayOnGround();
					return;
				}

				StepMove();
			}
			finally
			{

				// Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
				Velocity -= BaseVelocity;
			}

			StayOnGround();
		}

        public override void Simulate()
        {
            base.Simulate();
            DebugOverlay.ScreenText(new Vector2(50,50), 0, Color.Green, "X: " + (Velocity.x).ToString(), 0);
            DebugOverlay.ScreenText(new Vector2(50,50), 1, Color.Green, "Y: " + (Velocity.y).ToString(), 0);
            DebugOverlay.ScreenText(new Vector2(50,50), 2, Color.Green, "Z: " + (Velocity.z).ToString(), 0);
            DebugOverlay.ScreenText(new Vector2(50,50), 3, Color.Green, "Vel: " + Velocity.Length, 0);
            DebugOverlay.ScreenText(new Vector2(50,50), 4, Color.Green, "Horiz-Vel: " + (Velocity.WithZ(0)).Length, 0);
            
        }
    }
}
