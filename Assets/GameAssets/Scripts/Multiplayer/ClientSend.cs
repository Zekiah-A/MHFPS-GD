using System;
using System.Collections;
using System.Collections.Generic;
using Utils.Colour;
using Godot;

public partial class ClientSend
{

    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        try
        {
            Client.instance.udp.SendData(_packet);
        }
        catch (Exception _ex)
        {
            GD.Print($"Issue sending UDP data to server: {_ex}");
        }
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(LobbyUIManager.usernameEntry);
            SendTCPData(_packet);
        }
    }

    public static void UDPTestReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.udpTestReceived))
        {
            _packet.Write("Received a UDP packet.");
            SendUDPData(_packet);
        } 
    }

    public static void UpdatePositionReceived(Vector3 _newPos)
    {
        using (Packet _packet = new Packet((int)ClientPackets.updatePositionReceived))
        {
            _packet.Write(_newPos);
            SendUDPData(_packet);
        }
    }

    public static void UpdateRotationReceived(Quaternion _newRot)
    {
        using (Packet _packet = new Packet((int)ClientPackets.updateRotationReceived))
        {
            _packet.Write(_newRot);
            SendUDPData(_packet); 
        }
    }

    public static void TextChatReceived(string _msg, Colour _colour)
    {
        using (Packet _packet = new Packet((int)ClientPackets.textChatReceived))
        {   //write sender and message
/*
            _packet.Write(UiManager.instance.usernameField.text); //TODO: make a dictionary or var for player names, this is stupid
*/
            _packet.Write(_msg);
            _packet.Write(_colour);
            SendUDPData(_packet);
        }
    }

    public static void RigidUpdateReceived(int _rigidId, Vector3 _newPos)
    {
        using (Packet _packet = new Packet((int)ClientPackets.rigidUpdateReceived))
        {
            _packet.Write(_rigidId);
            _packet.Write(_newPos);
            SendUDPData(_packet);
        }
    }
    //HACK: Test values for now
    public static void PlayerDamageReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerDamageReceived))
        {
            _packet.Write(1);//player hit)
            _packet.Write(1.0f);//damagedealt)
            SendTCPData(_packet);
        }
    }
    #endregion
}
