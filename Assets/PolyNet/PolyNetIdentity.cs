using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyNetIdentity : MonoBehaviour {

	private PolyNetChunk chunk;
	public int prefabId;
	public int instanceId;

	public void initialize (int i) {
		instanceId = i;
		chunk = PolyNetWorld.linkChunk (this);
	}

}
