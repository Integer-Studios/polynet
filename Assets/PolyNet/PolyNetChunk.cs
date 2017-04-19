using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyNetChunk {

	private List<PolyNetPlayer> listeners;

	public void addPlayerToListeners (PolyNetPlayer player) {
		listeners.Add (player);
	}

}
