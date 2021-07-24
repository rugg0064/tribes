
using Sandbox;

namespace Tribes
{
    class TribesWalkController : Sandbox.WalkController
    {
		//public float GroundAngle { get; set; } = 46.0f;
		//public new float GroundAngle { get; set; } = 9999f;

		//public override float GroundFriction = 0;
		//public override float MoveFriction = 0;

		public float slidingGroundFriction = 0.1f;
        public float slidingMoveFriction = 0.1f;
        public float normalGroundFriction = 4f;
        public float normalMoveFriction = 1f;

        public TribesWalkController() : base()
		{
			GroundAngle = -99999f;
            GroundFriction = normalGroundFriction;
            MoveFriction = normalMoveFriction;
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

            Velocity = Velocity.ClampLength(2000);

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
