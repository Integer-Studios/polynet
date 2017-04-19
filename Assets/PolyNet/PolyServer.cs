using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PolyServer {

	public static int port;
	public static bool isActive = false;
	private static int reliableChannelId, socketId;
	private static Dictionary<int, PolyNetPlayer> players = new Dictionary<int, PolyNetPlayer> ();

	public static void start (int sPort) {
		port = sPort;
		NetworkTransport.Init();
		ConnectionConfig config = new ConnectionConfig();
		reliableChannelId  = config.AddChannel(QosType.Reliable);
		HostTopology topology = new HostTopology(config, 10);
		socketId = NetworkTransport.AddHost(topology, port);
		Debug.Log ("PolyNet Server Started on Port: "+ port +", socketId: " + socketId);
		isActive = true;
	}

	public static void update() {
		int recHostId, recConnectionId, recChannelId;
		byte[] recBuffer = new byte[1024];
		int bufferSize = 1024;
		int dataSize;
		byte error;
		NetworkEventType recNetworkEvent = NetworkTransport.Receive (out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
		PolyNetPlayer player;
		switch (recNetworkEvent) {
		case NetworkEventType.Nothing:
			break;
		case NetworkEventType.ConnectEvent:
			onConnect (new PolyNetPlayer (recConnectionId));
			break;
		case NetworkEventType.DataEvent:
			if (players.TryGetValue (recConnectionId, out player))
				onRecieveMessage (recBuffer, player);
			else
				playerNotFoundException (recConnectionId);
			break;
		case NetworkEventType.DisconnectEvent:
			if (players.TryGetValue (recConnectionId, out player))
				onDisconnect (player);
			else
				playerNotFoundException (recConnectionId);
			break;
		}
	}

	public static void sendMessage(byte[] buffer, PolyNetPlayer player) {
		byte error; NetworkTransport.Send(socketId, player.connectionId, reliableChannelId, buffer, 1024, out error);
	}

	private static void onRecieveMessage(byte[] buffer, PolyNetPlayer player) {
		PacketHandler.serverHandlePacket (buffer, player);
	}

	private static void onConnect(PolyNetPlayer p) {
		Debug.Log ("Player connected with ID: " + p.connectionId);
		players.Add (p.connectionId, p);
		PacketHandler.queuePacket (new Packet (), new PolyNetPlayer[]{p});
	}

	private static void onDisconnect(PolyNetPlayer p) {
		Debug.Log ("Player Disconnected with id: " + p.connectionId);
	}

	private static void playerNotFoundException(int connectionID) {
		Debug.Log ("No Player found for connection ID: " + connectionID + " ignoring packet...");
	}
}
