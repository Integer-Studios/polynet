﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PolyNetIdentity : PolyNetBehaviour {

	public PolyNetChunk chunk;
	public PolyNetPlayer owner;
	public int prefabId;
	public int instanceId;
	public bool isStatic = true;
	public bool isLocalPlayer = false;
	public Dictionary<int, PolyNetBehaviour> behaviours;

	public void initialize (int i) {
		instanceId = i;
	}

	public void Awake() {
		behaviours = new Dictionary<int,PolyNetBehaviour> ();
		int nextId = 0;
		foreach (PolyNetBehaviour b in GetComponents<PolyNetBehaviour>()) {
			b.scriptId = nextId;
			b.identity = this;
			behaviours.Add (nextId, b);
			nextId++;
		}
	}

	public void routeBehaviourPacket(PacketBehaviour p) {
		PolyNetBehaviour b;
		if (behaviours.TryGetValue (p.scriptId, out b))
			b.handleBehaviourPacket (p);
		else
			Debug.Log ("Invalid script id on behaviour packet: " + p.scriptId + ". Ignoring packet.");
	}

	public void sendBehaviourPacket(PacketBehaviour p) {
		if (PolyServer.isActive)
			chunk.sendPacket (p);
		else
			PacketHandler.queuePacket (p, null);
	}

	public void writeSpawnData(ref BinaryWriter writer) {
		PolyNetBehaviour b;
		int i = 0;
		while (behaviours.TryGetValue (i, out b)) {
			b.writeBehaviourSpawnData (ref writer);
			i++;
		}
	}

	public void readSpawnData(ref BinaryReader reader) {
		PolyNetBehaviour b;
		int i = 0;
		while (behaviours.TryGetValue (i, out b)) {
			b.readBehaviourSpawnData (ref reader);
			i++;
		}
	}

	private void Update() {
		if (isStatic || !PolyServer.isActive)
			return;
		if (owner != null)
			owner.position = transform.position;
		
		if (PolyNetWorld.getChunkIndex (transform.position).x != chunk.index.x || PolyNetWorld.getChunkIndex (transform.position).z != chunk.index.z) {
			chunk.migrateChunk (this);
			if (owner != null)
				owner.refreshLoadedChunks ();
		}
	}

}
