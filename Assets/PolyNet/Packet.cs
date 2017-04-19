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
	//switch to binary reader/writer ref's
	public void read(ref BinaryReader reader) {
		Debug.Log(reader.ReadString ());
		Debug.Log(reader.ReadInt32 ());
		Debug.Log(reader.ReadDecimal());
		Debug.Log(reader.ReadBoolean());
	}

	public void write(ref BinaryWriter writer) {
		writer.Write ("test");
		writer.Write (12);
		writer.Write ((decimal)32.5f);
		writer.Write (true);
	}

}