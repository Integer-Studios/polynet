using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PolyNetManager : MonoBehaviour {

	public string serverAddress;
	public int serverPort;
	public int clientPort;
	public bool isClient;
	public int playerId;
	public float chunkSize;
	public int chunkLoadRadius;
	public PolyNetIdentity playerPrefab;

	// Use this for initialization
	void Awake () {
		if (isClient)
			PolyClient.start (clientPort, serverPort, serverAddress);
		else
			PolyServer.start (serverPort);

		StartCoroutine (socketListenerUpdate ());
		StartCoroutine (packetWriterUpdate ());
	}

	void Start() {
		PolyNetWorld.initialize (chunkSize, chunkLoadRadius, playerPrefab);
	}

	private IEnumerator socketListenerUpdate() {
		while (true) {
			if (isClient)
				PolyClient.update ();
			else
				PolyServer.update ();
			yield return new WaitForSeconds(0f);
		}
	}

	private IEnumerator packetWriterUpdate() {
		while (true) {
			PacketHandler.update ();
			yield return new WaitForSeconds(0f);
		}
	}

}
