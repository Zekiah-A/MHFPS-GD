using System.Collections;
using System.Collections.Generic;
using Godot;

public class GameManager : Node
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
/*
    public static Dictionary<int, Netrigid> rigidbodies = new Dictionary<int, Netrigid>();
*/
    public Spatial localPlayerPrefab;
    public Spatial playerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            GD.Print("Instance already exists, destroying object!");
/*
            Destroy(this);
*/
        }
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quat _rotation)
    {
        Spatial _player;

        if (_id == Client.instance.myId)
        {
/*
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
*/
        }
        else
        {
/*
            _player = Instantiate(playerPrefab, _position, _rotation);
*/
        }
        // What if they don't hsave one lol(
/*
        _player.GetComponent<PlayerManager>().id = _id;
        _player.GetComponent<PlayerManager>().username = _username;
        players.Add(_id, _player.GetComponent<PlayerManager>());
*/
    }

    public void UpdateRigidbodies(int _rigidId, Vector3 _newPos) //TODO: rotation of rb object
    {
        GD.Print($"Updating {_rigidId}'s position to {_newPos}");
/*
        if (rigidbodies.TryGetValue(_rigidId, out Netrigid _rigid))
        {
            _rigid.gameObject.transform.position = _newPos;
        }
*/
    }
}
