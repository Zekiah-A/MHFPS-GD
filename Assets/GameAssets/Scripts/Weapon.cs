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
	private PackedScene bulletHole;
	private Spatial player;
	Camera playerCamera;
	Tween weaponTween;

	public override void _Ready() //better idea, extend for melee
	{
		weaponCast = GetNode<RayCast>("WeaponCast");
		
		muzzleFlash = GetNode("Mesh").GetNode<Spatial>("MuzzleFlash");
		muzzleFlash.Visible = false;

		weaponTimer = GetNode<Timer>("Timer");
		weaponTimer.WaitTime = 0.2f; //make  ext
		weaponTimer.Connect("timeout", this, "OnFireEnd");
		
		impactParticles = GD.Load("res://Assets/Scenes/Resources/ImpactParticles.tscn") as PackedScene; //other = bloodparticles
		bulletHole = GD.Load("res://Assets/Scenes/Resources/BulletHole.tscn") as PackedScene; //other = other mats?

		player = GetTree().CurrentScene.FindNode("Player") as Spatial;
		playerCamera = player.GetNode("SpringArm").GetNode<Camera>("Camera");

		weaponTween = GetNode<Tween>("WeaponTween");
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
					Particles particles = impactParticles.Instance<Particles>();
					particles.LookAt(-weaponRay.GetCollisionNormal(), Vector3.Up);
					particles.Translation = hitPosition;
					GetTree().CurrentScene.AddChild(particles);
					particles.Emitting = true;

					//make a dict and GC after it reaches a number
					Spatial hole = bulletHole.Instance<Spatial>();
					hole.LookAt(-weaponRay.GetCollisionNormal(), Vector3.Up);
					hole.Translation = hitPosition;
					GetTree().CurrentScene.AddChild(hole);
				//}

				GD.Print($"Weapon hit target {target}");
			}
		}

		playerCamera.RotationDegrees = new Vector3(
			playerCamera.Rotation.x + 1,
			playerCamera.RotationDegrees.y,
			playerCamera.RotationDegrees.z
		);
	}

	public void OnFireEnd()
	{
		muzzleFlash.Visible = false;
		playerCamera.RotationDegrees = new Vector3(
			playerCamera.Rotation.x - 1,
			playerCamera.RotationDegrees.y,
			playerCamera.RotationDegrees.z
		);
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
