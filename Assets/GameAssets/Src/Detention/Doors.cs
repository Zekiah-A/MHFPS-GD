using Godot;

public partial class Doors : Node3D
{
	private Label batteryLabel;

	private int BatteryPercentage;
	private Timer batteryTimer;
	private uint doorsClosed;

	private Node3D leftDoor;
	private Node3D rightDoor;
	public bool LeftOpened = true;
	public bool RightOpened = true;

	private bool VentLit;

	//If this value ever goes negative, then something has gone seriously wrong.
	private uint DoorsClosed
	{
		get => doorsClosed;
		set
		{
			doorsClosed = value;
			OnDoorsStateChanged();
		}
	}

	public override void _Ready()
	{
		leftDoor = GetNode<Node3D>("LeftDoor");
		rightDoor = GetNode<Node3D>("RightDoor");
		batteryTimer = GetNode<Timer>("BatteryTimer");
		batteryLabel = GetTree().CurrentScene.GetNode<Label>("DetentionUI/BatteryPanel/BatteryLeft");

		BatteryPercentage = 100;
		batteryTimer.Start();
	}

	public void OpenLeft()
	{
		DoorsClosed--;
		leftDoor.Visible = false;
		LeftOpened = true;
	}

	public void CloseLeft()
	{
		DoorsClosed++;
		leftDoor.Visible = true;
		LeftOpened = false;
	}

	public void OpenRight()
	{
		DoorsClosed--;
		rightDoor.Visible = false;
		RightOpened = true;
	}

	public void CloseRight()
	{
		DoorsClosed++;
		rightDoor.Visible = true;
		RightOpened = false;
	}

	public void TorchOnVent()
	{
		GD.Print("Vent Hovered (Lit)");
		VentLit = true;
	}

	public void TorchOffVent()
	{
		GD.Print("Vent Exited (Unlit)");
		VentLit = false;
	}

	/// <summary>
	///     Battery Mechanics:
	///     0 Doors open: Decrease by 1 every 4 seconds (at 0 from 100 after 6.6 minutes).
	///     1 Door open: Decrease by 1 every 2 seconds (at 0 from 100 after 3.3 minutes).
	///     2 Doors open: Decrease by 1 every 1.5 seconds (at 0 after 2.5 minutes).
	/// </summary>
	private void OnBatteryTimerTimeout()
	{
		BatteryPercentage--;
		batteryLabel.Text = $"{BatteryPercentage}%";

		//If player has run out of battery, leave them completely vunerable, possible also disable their torch.
		if (BatteryPercentage != 0) return;

		batteryTimer.Stop();
		(GetTree().CurrentScene.GetNode("Phone") as Phone)?.Disable();
		OpenLeft();
		OpenRight();
	}

	/// <summary>
	///     Called when a door has been closed or opened, so that battery decrease can be recalculated.
	/// </summary>
	private void OnDoorsStateChanged()
	{
		batteryTimer.WaitTime = DoorsClosed switch
		{
			0 => 4,
			1 => 2,
			2 => 1.5f,
			_ => batteryTimer.WaitTime
		};

		GD.Print($"Doors state changed [{DoorsClosed}], battery wait time set to {batteryTimer.WaitTime}.");
	}
}
