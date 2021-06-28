using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Ammo : Panel
{
	public Label text;
    //public Label Weapon;
    //public Label Inventory;

    public Ammo()
    {
		AddClass( "ammo" );
		text = Add.Label();
		//Weapon = Add.Label( "100", "weapon" );
        //Inventory = Add.Label( "100", "inventory" );
    }

    public override void Tick()
	{
        Tribes.TribesPlayer player = (Tribes.TribesPlayer) Local.Pawn;
        if ( player == null ) return;

        //Log.Info(player.ammo);

        SetClass( "active", true );

		//Weapon.Text = string.Format("{0:0}", player.ammo);

		//Inventory.Text = $" / {100}";
		//Inventory.SetClass( "active", 5 >= 0 );

		text.Text = string.Format( "{0:0} / 100", player.ammo );
	}
}
