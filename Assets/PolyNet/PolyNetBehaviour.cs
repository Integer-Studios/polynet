using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PolyNetBehaviour : MonoBehaviour {

	public int scriptId;
	public PolyNetIdentity identity;

	public virtual void handleBehaviourPacket(PacketBehaviour p) {

	}

	public virtual void writeBehaviourSpawnData(ref BinaryWriter writer) {
		
	}

	public virtual void readBehaviourSpawnData(ref BinaryReader reader) {
		
	}

}
