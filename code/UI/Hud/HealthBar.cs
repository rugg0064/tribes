using Sandbox.UI;
using Sandbox;
using System;
public class HealthBar : Panel
{
	public Panel shieldIcon;
	public Panel healthIcon;

	public Panel shieldContent;
	public Panel healthContent;

	public HealthBar()
	{
		//StyleSheet.Load( "/stolen_stuff/UI/PointsOverlay.scss" ); 
		AddClass( "healthBar" );
		Panel shield = this.Add.Panel( "shieldContainer" );
		shieldIcon = shield.Add.Panel( "icon" );
		shieldContent = shield.Add.Panel( "content" );

		Panel health = this.Add.Panel( "healthContainer" );
		healthIcon = health.Add.Panel( "icon" );
		healthContent = health.Add.Panel( "content" );
	}

	public override void Tick()
	{
		base.Tick();
		

		shieldIcon.Style.Width = Length.Pixels( ScaleFromScreen * shieldIcon.Box.Rect.height );
		healthIcon.Style.Width = Length.Pixels( ScaleFromScreen * healthIcon.Box.Rect.height );

		float remaining = (this.Box.Rect.width - healthIcon.Box.Rect.width);

		Tribes.TribesPlayer player = (Tribes.TribesPlayer)Local.Pawn;
		float percent = player.Health / 100;
		//Log.Info( player.Health );
		//float percent = ( (MathF.Sin(Time.Now ) +1) / 2);
		float val = ScaleFromScreen * remaining * percent;
		//Log.Info( val );
		val = MathF.Max( 2, val );

		
		Sandbox.UI.Length healthWidth = new Sandbox.UI.Length()
		{
			Unit = Sandbox.UI.LengthUnit.Pixels,
			Value = val
		};
		healthContent.Style.Width = healthWidth;
		healthContent.Style.Dirty();


		float percent2 = ( (MathF.Sin(Time.Now ) +1) / 2);
		float val2 = ScaleFromScreen * remaining * (percent2);
		val2 = MathF.Max( 2, val2 );

		Sandbox.UI.Length shieldWidth = new Sandbox.UI.Length()
		{
			Unit = Sandbox.UI.LengthUnit.Pixels,
			Value = val2
		};
		shieldContent.Style.Width = shieldWidth;
		shieldContent.Style.Dirty();


		/*
		float x = ((MathF.Sin( Time.Now ) + 1) / 2) * 100;
		Log.Info( x );
		Sandbox.UI.Length newWidth = new Sandbox.UI.Length()
		{
			Unit = Sandbox.UI.LengthUnit.Percentage,
			Value = x
		};
		//healthContent.Style.Width = newWidth;
		//healthContent.Style.Dirty();
		*/
	}
}
