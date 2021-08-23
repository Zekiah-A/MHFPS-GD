using Godot;
using System;

public class Actor : KinematicBody
{


    public enum States
    {
         Idle = 0,
         Suspicious,
         Hostile,
         Hide
    }
}

