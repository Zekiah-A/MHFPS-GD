using Godot;
using System;
//BASE CLASS FOR ALL WEAPONS
public partial class Weapon : Node3D
{
	[Export] public int TotalAmmo = 16;
	[Export] public int MagazineSize = 8;
	[Export] public int CurrentAmmo = 8;
	private float swayThreshold = 4; //How much a mouse must be pushed to trigger.
	private readonly float swayLeft = -0.5f;
	private readonly float swayRight = 0.5f;
	private readonly float swayStrength = 5;

	private RayCast3D weaponCast; //really should be gun only
	private Area3D weaponArea; //really should be melee only for collisions of melee weapon
	private Node3D muzzleFlash;
	private Timer weaponTimer;
	private PackedScene impactParticles;
	private PackedScene bulletHole;
	private Node3D player;
	private Camera3D playerCamera;
	private Tween weaponTween;
	private Label ammoHud;
	AnimationPlayer animPlayer;

	private float mouseRelativeMovement = 0;
	private bool canFire = true;

	public override void _Ready() //better idea, extend for melee
	{
		weaponCast = GetNode<RayCast3D>("WeaponCast");

		muzzleFlash = GetNode("Mesh").GetNode<Node3D>("MuzzleFlash");
		muzzleFlash.Visible = false;

		weaponTimer = GetNode<Timer>("Timer");
		weaponTimer.WaitTime = 0.2f; //make  ext
		weaponTimer.Connect("timeout",new Callable(this,"OnFireEnd"));

		impactParticles = GD.Load("res://Assets/Scenes/Resources/ImpactParticles.tscn") as PackedScene; //other = bloodparticles
		bulletHole = GD.Load("res://Assets/Scenes/Resources/BulletHole.tscn") as PackedScene; //other = other mats?

		player = GetTree().CurrentScene.FindChild("Player") as Node3D;
		//playerCamera = player.GetNode("SpringArm3D").GetNode<Camera3D>("Camera3D"); //player should issue this to the head class?

		weaponTween = GetNode<Tween>("WeaponTween");
		ammoHud = GetNode("WeaponHUD").GetNode<Label>("Ammo");

		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		Reload();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("game_fire") && GetParent<InventoryItem>().Enabled)
			Fire();
		if (@event.IsActionPressed("game_reload") && GetParent<InventoryItem>().Enabled)
			Reload();
		if (@event is InputEventMouseMotion eventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
			mouseRelativeMovement = -eventMouseMotion.Relative.X;
	}

	public override void _PhysicsProcess(double delta)
	{
		SwayWeapon(delta);
		RotateWeapon(); //should use delta here too?
	}

	public void Fire()
	{

		RayCast3D weaponRay = GetNode<RayCast3D>("WeaponCast");

		if (animPlayer.IsPlaying() || canFire != true) return;
		CurrentAmmo--;
		if (CurrentAmmo != 0)
		{
			animPlayer.Play("WeaponFire");
			ammoHud.Text = $"{CurrentAmmo}/{MagazineSize} | {TotalAmmo}";
		}
		else //reload instead of fire, or reload then fire?
			Reload();

		muzzleFlash.Visible = true; //timer and then false
		weaponTimer.Start();

		if (!weaponRay.IsColliding()) return;
		var target = weaponRay.GetCollider();
		Vector3 hitPosition = weaponRay.GetCollisionPoint();
		//if target.isInGroup("Enemy"): //NOTE: THis "group" stuff could be useful
		//target.health -= damage
		//else
		//{
		var particles = impactParticles.Instantiate<GpuParticles3D>();
		particles.LookAt(-weaponRay.GetCollisionNormal(), Vector3.Up);
		particles.Position = hitPosition;
		GetTree().CurrentScene.AddChild(particles);
		particles.Emitting = true;

		//make a dict and GC after it reaches a number
		var hole = bulletHole.Instantiate<Node3D>();
		hole.LookAt(-weaponRay.GetCollisionNormal(), Vector3.Up);
		hole.Position = hitPosition;
		GetTree().CurrentScene.AddChild(hole);
		//}

		GD.Print($"Weapon hit target {target}");
		//TODO: Call a "shake" animation in the camera.
	}

	protected void OnFireEnd()
	{
		muzzleFlash.Visible = false;
	}

	protected virtual void Reload()
	{
		animPlayer.Play("WeaponReload");

		int diff = MagazineSize - CurrentAmmo;

		if (TotalAmmo - diff >= 0)
		{
			TotalAmmo -= diff;
			CurrentAmmo += diff;
		}
		else
		{
			CurrentAmmo = TotalAmmo;
			TotalAmmo = 0;
			if (CurrentAmmo == 0)
				canFire = false;
		}
		ammoHud.Text = $"{CurrentAmmo}/{MagazineSize} | {TotalAmmo}";
	}

	protected void RotateWeapon() //THIS doesn't matter anyway, because the ray should come from player HEAD and the bullet should hit where player is looking!
	{
		/*RotationDegrees = new Vector3(Mathf.Lerp(RotationDegrees.x, springArm.RotationDegrees.x, 0.2f), RotationDegrees.y, RotationDegrees.z);*/
	}

	protected virtual void SwayWeapon(double delta)
	{
		if (mouseRelativeMovement > swayThreshold)
			Rotation = new Vector3(0, Mathf.Lerp(Rotation.Y, swayLeft, (float) (swayStrength * delta)), 0);
		else if (mouseRelativeMovement < -swayThreshold)
			Rotation = new Vector3(0, Mathf.Lerp(Rotation.Y, swayRight, (float) (swayStrength * delta)), 0);
		else
			Rotation = new Vector3(0, Mathf.Lerp(Rotation.Y, 0, (float) (swayStrength * delta)), 0);
	}
}
