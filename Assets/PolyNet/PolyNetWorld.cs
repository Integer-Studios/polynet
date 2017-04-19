using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyNetWorld {

	public PolyNetChunk chunkTemp;

	public PolyNetWorld() {
		chunkTemp = new PolyNetChunk ();
	}

	public void addPlayerToWorld (PolyNetPlayer player) {
		// finding which chunk would eventually require knowing where the player is (from save)
		chunkTemp.addPlayerToListeners (player);
	}

	public static PolyNetWorld worldTemp = new PolyNetWorld();
	public static PolyNetWorld getWorld() {
		return worldTemp;
	}
}
