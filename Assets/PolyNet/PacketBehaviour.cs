using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PacketBehaviour : Packet {

	public int chunkId;
	public int instanceId;
	public int scriptId;
	public int commandId;

	public PacketBehaviour() {
		id = 1;
	}

	public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
		chunkId = reader.ReadInt32 ();
		instanceId = reader.ReadInt32 ();
		scriptId = reader.ReadInt32 ();
		commandId = reader.ReadInt32 ();
	}

	public override void write(ref BinaryWriter writer) {
		writer.Write (chunkId);
		writer.Write (instanceId);
		writer.Write (scriptId);
		writer.Write (commandId);
	}

	public static void routeToBehaviour(PacketBehaviour p) {
		//find chunk
		//give it the packet
		//it finds the instance
		//give it the packet
		//it finds the script
		//give it the id
		//it finds the command
		//calls it
	}
}
