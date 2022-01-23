using Godot;
using System;
using System.Collections.Generic;

public class Doors : Spatial
{
    public bool LeftOpened = true;
    public bool RightOpened = true;
    public bool VentLit = false;

    public Spatial leftDoor;
    public Spatial rightDoor;

    public override void _Ready()
    {
        leftDoor = GetNode<Spatial>("LeftDoor");
        rightDoor = GetNode<Spatial>("RightDoor");
    }

    //Temporary code.
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
}
