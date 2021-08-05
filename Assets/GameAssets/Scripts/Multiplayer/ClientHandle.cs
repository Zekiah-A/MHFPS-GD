using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Utils.Colour;
using Godot;

public class ClientHandle
{
/*
    public delegate void PlayerDamageHandler(object sender, PlayerDamageArgs e);
    public static event PlayerDamageHandler PlayerDamageEvent;
*/
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        GD.Print($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        //HACK: Connect at samne time as TCP connect
        //Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        //HACK: Connect UDP here instead
        try
        {
            Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        }
        catch (Exception _ex)
        {
            GD.Print($"Could not force connect to UDP on TCP connection {_ex}");
        }
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quat _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void UDPTest(Packet _packet)
    {
        string _msg = _packet.ReadString();

        GD.Print($"Received packet via UDP. Contains message: {_msg}");
        ClientSend.UDPTestReceived();
    }
    //TODO: Smoothly lerp/translate between positions (since we only get updates 30 times/sec)
    public static void PlayerPosition(Packet _packet)
    {
        int _toPlayer = _packet.ReadInt();
        Vector3 _newPos = _packet.ReadVector3();
/*
        foreach (PlayerManager _player in GameManager.players.Values)
        {
            if (_player.id == _toPlayer) //TODO: (if we have found the recipient with foreach) - this is slow, go direct and move them!
            {
                GameObject _plrObj = GameManager.players[_player.id].gameObject;
                _plrObj.transform.position = _newPos;
                //TODO: _plrObj.transform.localPosition = Vector3.MoveTowards(_plrObj.transform.localPosition, _newPos, Time.deltaTime * _plrMovement.Speed);
                break;
            }
        }
*/
    }
    //TODO: Smoothly rotate to new position.
    public static void PlayerRotation(Packet _packet)
    {
        int _toPlayer = _packet.ReadInt();
        Quat _newRot = _packet.ReadQuaternion();
/*
        foreach (PlayerManager _player in GameManager.players.Values)
        {
            if (_player.id == _toPlayer) //TODO: (if we have found the recipient with foreach) - this is slow, go direct and move them!
            {
                GameObject _plrObj = GameManager.players[_player.id].gameObject;
                _plrObj.transform.rotation = _newRot;
                break;
            }
        }
*/
    }

    public static void TextChat(Packet _packet)
    {
        int _sender = _packet.ReadInt();
        string _msg = _packet.ReadString();
        Colour _colour = _packet.ReadColour();

        GD.Print(_msg);
/*
        Netplayer_HUD.instance.UpdateTextChat(_msg, _colour);
*/
        GD.Print($"{_colour.R} {_colour.G} {_colour.B} {_colour.A}");
    }

    public static void RigidUpdate(Packet _packet)
    {
        int _rigidID = _packet.ReadInt();
        Vector3 _newPos = _packet.ReadVector3();

        GD.Print($"Rigidbody object with ID {_rigidID} was moved to {_newPos}");

        //TODO: Call GameManager to move the rigidbody object
        GameManager.instance.UpdateRigidbodies(_rigidID, _newPos);
    }

    public static void PlayerDamage(Packet _packet)
    {
        int _playerHit = _packet.ReadInt();
        float _playerHealth = _packet.ReadFloat();

        GameManager.players[_playerHit].health = _playerHealth;
        GD.Print($"Player {_playerHit} was hit and their health is now {_playerHealth} (from pk) {GameManager.players[_playerHit].health} (from playermanager).");

        //TODO: Make an event or action that will call all the effects for damage
/*
        GameManager.players[_playerHit].healthbar.text = _playerHealth.ToString();
*/        
        //HACK: This is terribleeeeeeeeeeeeeeeeeee

        //PlayerDamageEvent?.Invoke(null, new PlayerDamageArgs("Hello"));
        //PlayerDamageEvent += PlayerManager.HandlePlayerDamage;
    }
}///<note>No, an event or acrion is NOT needed, just call a function when the var is changed</note>