using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyNetChunk {

	private List<PolyNetIdentity> objects;
	private List<PolyNetPlayer> players;
	public ChunkIndex index;

	public PolyNetChunk(ChunkIndex i) {
		index = i;
		objects = new List<PolyNetIdentity> ();
		players = new List<PolyNetPlayer> ();
	}

	public void addObject(PolyNetIdentity i) {
		objects.Add (i);
	}

	public void removeObject(PolyNetIdentity i) {
		objects.Remove (i);
	}

	public void addPlayer(PolyNetPlayer i) {
		players.Add (i);
		PolyNetPlayer[] rec = new PolyNetPlayer[]{ i };
		foreach(PolyNetIdentity o in objects) {
			PacketHandler.queuePacket (new PacketObjectSpawn (o), rec);
		}
	}

	public void removePlayer(PolyNetPlayer i) {
		players.Remove (i);
		PolyNetPlayer[] rec = new PolyNetPlayer[]{ i };
		foreach(PolyNetIdentity o in objects) {
			PacketHandler.queuePacket (new PacketObjectDespawn (o), rec);
		}
	}

	public void sendPacket(Packet p) {
		PacketHandler.queuePacket (p, players.ToArray());
	}

}
