using Godot;

///<summary> All items that can be stored within the inventory (Weapon, Object, EffectObject), will inherit from this?</summary>
public partial class InventoryItem : Node3D
{
    private bool enabled;
    private Texture2D inventorytexture;
    [Export] public string InventoryTexture;
    [Export] public int ItemType;

    private Area3D pickupCollider;

    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            OnEnabledChanged();
        }
    }

    private void OnPickupCollide()
    {
        //if inventory is not containing this, move into player and update (if collided with plr)
        GD.Print("Collision!");
    }

    private void OnEnabledChanged()
    {
        GD.Print($"Enabled set to {enabled}");

        //HACK: BODGE: Quick bodge job :)
        Visible = enabled;
    }

    //Collide signal
}