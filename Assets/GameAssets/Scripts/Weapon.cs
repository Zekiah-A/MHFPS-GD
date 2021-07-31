using Godot;
using System;
	//BASE CLASS FOR ALL WEAPONS
public class Weapon : Spatial
{
	[Export] public int MaxAmmo;
	[Export] public int CurrentAmmo;

	private RayCast weaponCast; //really should be gun only
	private Area weaponArea; //really should be melee only for collisions of melee weapon
	private Spatial muzzleFlash;
	private Timer weaponTimer;
	private PackedScene impactParticles;
/////
	public override void _Ready() //better idea, extend for melee
	{
		weaponCast = GetNode<RayCast>("WeaponCast");
		
		muzzleFlash = GetNode("Mesh").GetNode<Spatial>("MuzzleFlash");
		muzzleFlash.Visible = false;

		weaponTimer = GetNode<Timer>("Timer");
		weaponTimer.WaitTime = 0.2f; //make  ext
		weaponTimer.Connect("timeout", this, "OnFireEnd");
		
		impactParticles = GD.Load("res://Assets/Scenes/Resources/ImpactParticles.tscn") as PackedScene; //other = bloodparticles
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("game_fire"))
			Fire();
	}

	public override void _PhysicsProcess(float delta)
	{
		//if(weaponCast.IsColliding())
		//	GD.Print("WeaponCast is colliding");
	}

	public void Fire()
	{
		AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		RayCast weaponRay = GetNode<RayCast>("WeaponCast");

		if (!animPlayer.IsPlaying()) //this kinda limits it to spamming at anim length (0.2 seconds, i could make this the timer length too thru script :thinking:)
		{
			animPlayer.Play("WeaponFire");
			muzzleFlash.Visible = true; //timer and then false
			weaponTimer.Start();

			if (weaponRay.IsColliding())
			{
				var target = weaponRay.GetCollider();
				Vector3 hitPosition = weaponRay.GetCollisionPoint();
				// V DO SPECIAL EFFECTS
				//if target.isInGroup("Enemy"): //NOTE: THis "group" stuff could be useful
					//target.health -= damage
				//else
				//{
					Spatial particles = (Spatial) impactParticles.Instance();
					particles.Translation = hitPosition;
					particles.Rotation = Vector3.Zero;
					AddChild(particles);

					GD.Print($"Impact particles instanced at {particles.Translation} with parent {particles.GetParent()}.");
				//}
				GD.Print($"Weapon hit target {target}");
			}
		}
	}

	public void OnFireEnd()
	{
		muzzleFlash.Visible = false;
	}
}
/*
	if Input.is_action_pressed("fire"):
		if not anim_player.is_playing():
			camera.translation = lerp(camera.translation, 
					Vector3(rand_range(MAX_CAM_SHAKE, -MAX_CAM_SHAKE), 
					rand_range(MAX_CAM_SHAKE, -MAX_CAM_SHAKE), 0), 0.5)
			if raycast.is_colliding():
				var target = raycast.get_collider()
				if target.is_in_group("Enemy"):
					target.health -= damage
		anim_player.play("AssaultFire")
	else:
		camera.translation = Vector3()
		anim_player.stop()

*/

/*
private PackedScene _bulletScene = (PackedScene)GD.Load("res://bullet.tscn");

public void OnShoot()
{
	Node bullet = _bulletScene.Instance();
	AddChild(bullet);
*/
