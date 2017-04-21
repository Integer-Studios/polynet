using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyNetIdentity : PolyNetBehaviour {

	private PolyNetChunk chunk;
	public int prefabId;
	public int instanceId;
	public bool isStatic = true;
	public Dictionary<int, PolyNetBehaviour> behaviours;

	public void initialize (int i) {
		instanceId = i;
		chunk = PolyNetWorld.linkChunk (this);
	}

	public void routeBehaviourPacket(PacketBehaviour p) {
		PolyNetBehaviour b;
		if (behaviours.TryGetValue (p.scriptId, out b))
			b.handleBehaviourPacket (p);
		else
			Debug.Log ("Invalid script id on behaviour packet: " + p.scriptId + ". Ignoring packet.");
	}

	public void sendBehaviourPacket(PacketBehaviour p) {
		chunk.sendPacket (p);
	}

	private void Start() {
		behaviours = new Dictionary<int,PolyNetBehaviour> ();
		int nextId = 0;
		foreach (PolyNetBehaviour b in GetComponents<PolyNetBehaviour>()) {
			b.scriptId = nextId;
			b.identity = this;
			behaviours.Add (nextId, b);
			nextId++;
		}
	}

	private void Update() {
		if (isStatic || !PolyServer.isActive)
			return;
		//		TODO check if this works (ironic)
		if (PolyNetWorld.getChunkIndex (transform.position).x != chunk.index.x || PolyNetWorld.getChunkIndex (transform.position).z != chunk.index.z) {
			chunk = PolyNetWorld.migrateChunk (this, chunk);
		}
	}

}
