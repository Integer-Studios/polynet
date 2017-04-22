﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PolyNetChunk {

	private List<PolyNetIdentity> objects;
	private List<PolyNetPlayer> players;
	public ChunkIndex index;

	public PolyNetChunk(ChunkIndex i) {
		index = i;
		objects = new List<PolyNetIdentity> ();
		players = new List<PolyNetPlayer> ();
	}

	private void addObject(PolyNetIdentity i) {
		objects.Add (i);
		i.chunk = this;
	}

	private void removeObject(PolyNetIdentity i) {
		objects.Remove (i);
		i.chunk = null;
	}

	public void spawnObject(PolyNetIdentity i) {
		addObject (i);
		sendPacket (new PacketObjectSpawn (i));
	}

	public void despawnObject(PolyNetIdentity i) {
		removeObject (i);
		sendPacket (new PacketObjectDespawn (i));
	}

	public void migrateChunk(PolyNetIdentity i) {
		PolyNetChunk newChunk = PolyNetWorld.getChunk(i.transform.position);
		removeObject (i);
		newChunk.addObject (i);
		PolyNetPlayer[] rec = players.Except (newChunk.players).ToArray ();
		PacketHandler.queuePacket (new PacketObjectDespawn (i), rec);
		rec = newChunk.players.Except (players).ToArray ();
		PacketHandler.queuePacket (new PacketObjectSpawn (i), rec);
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
