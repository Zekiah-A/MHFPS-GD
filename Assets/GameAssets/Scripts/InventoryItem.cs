using Godot;
using System;

///<summary> All items that can be stored within the inventory (Weapon, Object, EffectObject), will inherit from this?</summary>
public class InventoryItem : Spatial 
{
	[Export] public Spatial GameObject;

	[Export] public int ItemType; //type - from enum

	[Export] public string InventoryTexture;
	private Area pickupCollider;

	//public override void _Ready() => pickupCollider = GetNode<Area>("PickupCollider");

	private void OnPickupCollide()
	{
		//if inventory is not containing this, move into player and update (if collided with plr)
		GD.Print("Collision!");
	}

	//Collide signal
}
