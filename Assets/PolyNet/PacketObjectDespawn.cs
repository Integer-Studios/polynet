using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PacketObjectDespawn : Packet {

	public int instanceId;

	public PacketObjectDespawn() {
		id = 1;
	}

	public PacketObjectDespawn(PolyNetIdentity i) {
		id = 1;
		instanceId = i.instanceId;
	}

	public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
		instanceId = reader.ReadInt32 ();
		PolyNetWorld.despawnObject (instanceId);
	}

	public override void write(ref BinaryWriter writer) {
		writer.Write (instanceId);
	}
}
