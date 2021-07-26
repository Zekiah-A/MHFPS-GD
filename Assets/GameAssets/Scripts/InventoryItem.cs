using Godot;
using System;

///<summary> All items that can be stored within the inventory (Weapon, Object, EffectObject), will inherit from this?</summary>
public class InventoryItem : Spatial 
{
	[Export] Spatial GameObject; //return the object this script is attached to.
//Player does not ever need to acess the individual weapon script, as that nworks cocpletely independantly, and is disabled / enabled when needed?
	[Export] public int ItemType; //type - from enum
}

//Or maybe just search up a godot inventory tutorial? LOL!
