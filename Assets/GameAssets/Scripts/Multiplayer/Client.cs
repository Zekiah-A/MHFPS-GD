using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Godot;

public partial class Client : Node
{
	public static Client instance = new Client();
	public static int dataBufferSize = 4096;

	public string ip = "127.0.0.1";
	public int port = 19130; //This is def port - make both customisable
	public int myId = 0;
	public TCP tcp;
	public UDP udp;

	private delegate void PacketHandler(Packet _packet);
	private static Dictionary<int, PacketHandler> packetHandlers;
	
/*
	public void Start() //private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			GD.Print("Instance already exists, destroying object!");
			Destroy(this);
		}
	}
*/
	//hack: e
	//private void Start()
	//{
	//    tcp = new TCP();
	//    udp = new UDP();
	//}

	public void ConnectToServer(string _ipField) //public void ConnectToServer() => tcp.Connect();
	{
		tcp = new TCP();
		udp = new UDP();

		var _ipFieldArray = Dns.GetHostAddresses(_ipField); 

		foreach (var i in _ipFieldArray)
		{
			if (_ipField.Length > 0)
			{
				string[] subIP = i.ToString().Split('.');

				if (subIP.Length == 4)
				{
					ip = i.ToString();
					//InitializeClientData();
					//tcp.Connect(); //TODO: fix this double connect bug - not to do with udp, i removed
					//break; //HACK: RemoveAt this since bit would happen anyway at the end!
				}
				else
				{
					//TODO: IPV6
				}
			}
		}
		GD.Print(_ipField);  //UDP doesn't connect.
		InitializeClientData();
		tcp.Connect();
	}

	public partial class TCP
	{
		public TcpClient socket;

		private NetworkStream stream;
		private Packet receivedData;
		private byte[] receiveBuffer;

		public void Connect()
		{
			socket = new TcpClient
			{
				ReceiveBufferSize = dataBufferSize,
				SendBufferSize = dataBufferSize
			};

			receiveBuffer = new byte[dataBufferSize];
			//socket.BeginConnect(instance.ip,new Callable(new Callable(instance.port,ConnectCallback),socket));
		}

		public  void SendData(Packet _packet)
		{
			try
			{
				if(socket != null)
				{
					stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
				}
			}
			catch(Exception _e)
			{
				GD.Print($"Error sending data to server via TCP: {_e}");
			}
		}

		private void ConnectCallback(IAsyncResult _result)
		{
			socket.EndConnect(_result);

			if(!socket.Connected)
			{
				return;
			}

			stream = socket.GetStream();

			receivedData = new Packet();

			stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
		}

		private void ReceiveCallback(IAsyncResult _result)
		{
			try
			{
				int _byteLength = stream.EndRead(_result);
				if (_byteLength <= 0 )
				{
					//TODO: Disconnect
					return;
				}
				
				byte[] _data = new byte[_byteLength];
				Array.Copy(receiveBuffer, _data, _byteLength);

				receivedData.Reset(HandleData(_data));
				stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

				GD.Print($"Sucessfully read buffer {receiveBuffer}");
			}
			catch(Exception _e)
			{
				GD.PrintErr($"Error receiving TCP data: {_e}");
			} // TODO: Disconnect            
		}

		private bool HandleData(byte[] _data)
		{
			int _packetLength = 0;

			receivedData.SetBytes(_data);

			if (receivedData.UnreadLength() >= 4)
			{
				_packetLength = receivedData.ReadInt();
				if(_packetLength <= 0)
				{
					return true;
				}
			}

			while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
			{
				byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
				ThreadManager.ExecuteOnMainThread(() =>
				{
					using (Packet _packet = new Packet(_packetBytes))
					{
						int _packetID = _packet.ReadInt();
						packetHandlers[_packetID](_packet);
					}
				});

				_packetLength = 0;
				if (receivedData.UnreadLength() >= 4)
				{
					_packetLength = receivedData.ReadInt();
					if (_packetLength <= 0)
					{
						return true;
					}
				}
			}

			if (_packetLength <= 1)
			{
				return true;
			}

			return false;
		}
	}

	public partial class UDP
	{
		public UdpClient socket;
		public IPEndPoint endPoint;

		public UDP()
		{
			endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
		}

		public void Connect(int _localPort)
		{
			socket = new UdpClient(_localPort);

			socket.Connect(endPoint);
			socket.BeginReceive(ReceiveCallback, null);

			using (Packet _packet = new Packet())
			{
				SendData(_packet);
			}
		}

		public void SendData(Packet _packet)
		{
			try
			{
				_packet.InsertInt(instance.myId);
				if(socket != null)
				{
					socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
				}
			}
			catch(Exception _e)
			{
				GD.Print($"Error sending data to server via UDP : {_e}");
				//HACK: Try to force and  connect here - it just doesn't
				Connect(((IPEndPoint)instance.tcp.socket.Client.LocalEndPoint).Port);
			}
		}

		private void ReceiveCallback(IAsyncResult _result)
		{
			try
			{
				byte[] _data = socket.EndReceive(_result, ref endPoint);
				socket.BeginReceive(ReceiveCallback, null);

				if (_data.Length < 4)
				{
					//TODO: Disconnect
					return;
				}

				HandleData(_data);
			}
			catch (Exception _ex)
			{
				//TODO: Disconnect
				GD.Print($"Error receiving UDP callback: {_ex}");
				return;
			}
		}

		private void HandleData(byte[] _data)
		{
			using (Packet _packet = new Packet(_data))
			{
				int _packetLength = _packet.ReadInt();
				_data = _packet.ReadBytes(_packetLength);
			}

			ThreadManager.ExecuteOnMainThread(() =>
			{
				using (Packet _packet = new Packet(_data))
				{
					int _packetId = _packet.ReadInt();
					packetHandlers[_packetId](_packet);
				}
			});
		}
	}

	private void InitializeClientData()
	{
		packetHandlers = new Dictionary<int, PacketHandler>()
		{
			{ (int)ServerPackets.welcome, ClientHandle.Welcome },
			{ (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
			{ (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
			{ (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
			{ (int)ServerPackets.textChat, ClientHandle.TextChat},
			{ (int)ServerPackets.udpTest, ClientHandle.UDPTest },
			{ (int)ServerPackets.rigidUpdate, ClientHandle.RigidUpdate},
			{ (int)ServerPackets.playerDamage, ClientHandle.PlayerDamage }
		};
		GD.Print("Initialized packets.");
	}
}
