using Godot;
using System;
	//BASE CLASS FOR ALL WEAPONS
public class Weapon : Spatial
{
	[Export] public int MaxAmmo;
	[Export] public int CurrentAmmo;

	private Timer timer;
	private RayCast weaponCast; //really should be gun only
	private Area weaponArea; //really should be melee only for collisions of melee weapon
	
	public override void _Ready() //better idea, extend for melee
	{
		timer = GetNode<Timer>("Timer"); //TODO: connect timer signals
		weaponCast = GetNode<RayCast>("WeaponCast");
	}

	public override void _PhysicsProcess(float delta)
	{
		//if(weaponCast.IsColliding())
		//	GD.Print("WeaponCast is colliding");
	}

	public virtual void Reload()
	{
	
	}

	public virtual void Fire()
	{

	}
}
