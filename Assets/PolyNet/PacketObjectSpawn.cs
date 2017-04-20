using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PacketObjectSpawn : Packet {

	public int prefabId;
	public int instanceId;
	public Vector3 position;
	public Vector3 scale;
	public Vector3 euler;

	public PacketObjectSpawn() {
		id = 0;
	}

	public PacketObjectSpawn(PolyNetIdentity i) {
		id = 0;
		prefabId = i.prefabId;
		instanceId = i.instanceId;
		position = i.transform.position;
		scale = i.transform.localScale;
		euler = i.transform.eulerAngles;
	}

	public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
		prefabId = reader.ReadInt32 ();
		instanceId = reader.ReadInt32 ();

		position = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
		scale = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
		euler = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());

		PolyNetWorld.clientSpawnObject (prefabId, instanceId, position, scale, euler);
	}

	public override void write(ref BinaryWriter writer) {
		writer.Write (prefabId);
		writer.Write (instanceId);

		writer.Write ((decimal)position.x);
		writer.Write ((decimal)position.y);
		writer.Write ((decimal)position.z);

		writer.Write ((decimal)scale.x);
		writer.Write ((decimal)scale.y);
		writer.Write ((decimal)scale.z);

		writer.Write ((decimal)euler.x);
		writer.Write ((decimal)euler.y);
		writer.Write ((decimal)euler.z);
	}

}
