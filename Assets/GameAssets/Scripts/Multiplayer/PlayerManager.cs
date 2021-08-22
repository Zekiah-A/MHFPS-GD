using System.Collections;
using System.Collections.Generic;
using Godot;
//using TMPro;

public class PlayerManager : Node
{
    public int id;
    public string username;
    public float health;
    public bool isDead;

    //public TextMeshPro healthbar;

    /// <summary> Handling a player being damaged event. </summary>
    /*
    public static void HandlePlayerDamage(object sender, PlayerDamageArgs args)
    {
        Debug.Log($"Event called and intercepted, {args}.");  
    }
    */   
}