using Godot;
using System;
	//BASE CLASS FOR ALL WEAPONS
public class Weapon : Spatial
{
	[Export] public int MaxAmmo;
	[Export] public int CurrentAmmo;
	private float swayThreshold = 10;
	private readonly float swayLeft = -0.5f;
	private readonly float swayRight = 0.5f;
	private readonly float swayStrength = 5;

	private RayCast weaponCast; //really should be gun only
	private Area weaponArea; //really should be melee only for collisions of melee weapon
	private Spatial muzzleFlash;
	private Timer weaponTimer;
	private PackedScene impactParticles;
	private PackedScene bulletHole;
	private Spatial player;
	private Camera playerCamera;
	private Tween weaponTween;
	private float mouseRelativeMovement;



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
		if (@event.IsActionPressed("game_fire") && GetParent<InventoryItem>().Enabled)
			Fire();

		if (@event is InputEventMouseMotion eventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
			mouseRelativeMovement = -eventMouseMotion.Relative.x;
	}

	public override void _PhysicsProcess(float delta)
	{
		SwayWeapon(delta);
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
		} //TODO: Call a "shake" animation in the camera.
	}

	public void OnFireEnd()
	{
		muzzleFlash.Visible = false;		
	}

	public virtual void SwayWeapon(float delta)
	{
		if (mouseRelativeMovement > swayThreshold)
			Rotation = new Vector3(0, Mathf.Lerp(Rotation.y, swayLeft, swayStrength * delta), 0);
		else if (mouseRelativeMovement < -swayThreshold)
			Rotation = new Vector3(0, Mathf.Lerp(Rotation.y, swayRight, swayStrength * delta), 0);
		else
			Rotation = new Vector3(0, Mathf.Lerp(Rotation.y, 0, swayStrength * delta), 0);
	}
}
