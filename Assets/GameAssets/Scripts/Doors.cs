using Godot;
using System;
using System.Collections.Generic;

public class Doors : Spatial
{
	public bool LeftOpened = true;
	public bool RightOpened = true;
	public bool VentLit = false;
	public int BatteryPercentage;
	
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
		leftDoor.Visible = false;
		LeftOpened = true;
	}
	
	public void CloseLeft()
	{
		leftDoor.Visible = true;
		LeftOpened = false;
	}

	public void OpenRight()
	{
		rightDoor.Visible = false;
		RightOpened = true;

	}

	public void CloseRight()
	{
		rightDoor.Visible = true;
		RightOpened = false;
	}

	public void TorchOnVent()
	{
		VentLit = true;
	}

	public void TorchOffVent()
	{
		VentLit = false;
	}
	
	private void OnBatteryTimerTimeout()
	{
		//TODO: Make this proper
		BatteryPercentage--;
		batteryLabel.Text = $"{BatteryPercentage}%";
	}
}


