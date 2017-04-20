using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Packet {
	
	public int id;
	public PolyNetPlayer[] recipients;

	public Packet() {
		id = -1;
	}
	public virtual void read(ref BinaryReader reader, PolyNetPlayer sender) {
	}

	public virtual void write(ref BinaryWriter writer) {
	}

	public static Packet getPacket(int id) {
		switch (id) {
		case -1:
			return new Packet ();
		case 0:
			return new PacketObjectSpawn ();
		case 1:
			return new PacketObjectDespawn ();
		default:
			return null;
		}
	}

}