using Godot;
using System;
using System.Collections.Generic;

public class Doors : Spatial
{
	public bool LeftOpened = true;
	public bool RightOpened = true;
	public bool VentLit = false;
	public int BatteryPercentage;
	//If this value ever goes negative, then something has gone seriously wrong.
	public uint DoorsClosed
	{
		get => doorsClosed;
		set
		{
			doorsClosed = value;
			OnDoorsStateChanged();
		}
	}
	private uint doorsClosed = 0;

	public Spatial leftDoor;
	public Spatial rightDoor;
	public Timer batteryTimer;
	public Label batteryLabel;

	public override void _Ready()
	{
		leftDoor = GetNode<Spatial>("LeftDoor");
		rightDoor = GetNode<Spatial>("RightDoor");
		batteryTimer = GetNode<Timer>("BatteryTimer");
		batteryLabel = GetTree().CurrentScene.GetNode("DetentionUI").GetNode("BatteryPanel").GetNode<Label>("Battery");

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
	/// Battery Mechanics:
	/// 0 Doors open: Decrease by 1 every 4 seconds (at 0 from 100 after 6.6 minutes).
	/// 1 Door open: Decrease by 1 every 2 seconds (at 0 from 100 after 3.3 minutes).
	/// 2 Doors open: Decrease by 1 every 1.5 seconds (at 0 after 2.5 minutes).
	/// </summary>
	private void OnBatteryTimerTimeout()
	{
		BatteryPercentage--;
		batteryLabel.Text = $"{BatteryPercentage}%";

		//If player has run out of battery, leave them completely vunerable, possible also disable their torch.
		if (BatteryPercentage == 0)
		{
			batteryTimer.Stop();
			(GetTree().CurrentScene.GetNode("Phone") as Phone)?.Disable();
			OpenLeft();
			OpenRight();
		}
	}

	/// <summary>
	/// Called when a door has been closed or opened, so that battery decrease can be recalculated.
	/// </summary>
	private void OnDoorsStateChanged()
	{
		switch (DoorsClosed)
		{
			case 0:
				batteryTimer.WaitTime = 4;
				break;
			case 1:
				batteryTimer.WaitTime = 2;
				break;
			case 2:
				batteryTimer.WaitTime = 1.5f;
				break;
		}
		GD.Print($"Doors state changed [{DoorsClosed}], battery wait time set to {batteryTimer.WaitTime}.");
	}
}
