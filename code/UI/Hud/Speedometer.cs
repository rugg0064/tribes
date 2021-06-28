using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using Tribes;

public class Speedometer : Panel
{
	public Panel Spedometer;
    public Speedometer()
	{
		//StyleSheet.Load( "/stolen_stuff/UI/Speedometer.scss" );
		AddClass( "speedometer" );
		//Spedometer = Add.Panel( "speedometer" );
		Spedometer = Add.Panel( );
    }

    public override void Tick()
    {
        
        Tribes.TribesPlayer player = (Tribes.TribesPlayer) Local.Pawn;
        if ( player == null ) return;

		//Max = center 40%
		//Left over 60%
		//Top 30%
		//Start at the bottom, top=70% height = 1%

		SetClass( "active", true );
        
        
        float horizVel = player.Velocity.WithZ(0).Length;
        float maxSpeed = 1200;
        float completion = horizVel / maxSpeed; // [0.0,1.0]
        float heightPercent = completion * 40;
        heightPercent = MathF.Max(1.0f, heightPercent);
        heightPercent = MathF.Min(40.0f, heightPercent);

        float topPercent = 30.0f + (40.0f - heightPercent);
        Sandbox.UI.Length heightLength = new Sandbox.UI.Length(){
            Unit =  Sandbox.UI.LengthUnit.Percentage,
            Value = heightPercent
        };

        Sandbox.UI.Length topLength = new Sandbox.UI.Length(){
            Unit =  Sandbox.UI.LengthUnit.Percentage,
            Value = topPercent
        };

        Style.Height = heightLength;
        Style.Top = topLength;
        Style.Dirty();
		
        
    }
}

