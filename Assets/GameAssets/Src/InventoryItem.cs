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
	}

	private void OnEnabledChanged()
	{
		Visible = enabled;
	}
}
