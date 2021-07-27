using Godot;
using System;

///<summary> All items that can be stored within the inventory (Weapon, Object, EffectObject), will inherit from this?</summary>
public class InventoryItem : Spatial 
{
	[Export] public Spatial GameObject; //return the object this script is attached to.
//Player does not ever need to acess the individual weapon script, as that nworks cocpletely independantly, and is disabled / enabled when needed?
	[Export] public int ItemType; //type - from enum

	[Export] public string inventoryTexture;

	private Area pickupCollider;

	//public override void _Ready() => pickupCollider = GetNode<Area>("PickupCollider");

	private void OnPickupCollide()
	{
		//if inventory is not containing this, move into player and update (if collided with plr)
		GD.Print("Collision!");
	}

	//Collide signal
}
