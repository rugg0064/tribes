using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Ammo : Panel
{
    public Label Weapon;
    public Label Inventory;

    public Ammo()
    {
        Weapon = Add.Label( "100", "weapon" );
        Inventory = Add.Label( "100", "inventory" );
    }

    public override void Tick()
    {
        MinimalExample.MinimalPlayer player = (MinimalExample.MinimalPlayer) Local.Pawn;
        if ( player == null ) return;

        //Log.Info(player.ammo);

        SetClass( "active", true );

        Weapon.Text = string.Format("{0:0}", player.ammo);

        //var inv = weapon.AvailableAmmo();
        Inventory.Text = $" / {100}";
        Inventory.SetClass( "active", 5 >= 0 );
    }
}