using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyNetWorld {

	private static Dictionary<int, GameObject> prefabs = new Dictionary<int, GameObject>();
	private static Dictionary<int, PolyNetIdentity> objects = new Dictionary<int, PolyNetIdentity>();
	public static Dictionary<ChunkIndex, PolyNetChunk> chunks = new Dictionary<ChunkIndex, PolyNetChunk>();
	private static float chunkSize;
	private static int chunkLoadRadius;
	private static int nextInstanceId = 0;
	private static PolyNetIdentity playerPrefab;

	public static void initialize(float s, int r, PolyNetIdentity playerPre) {
		
		chunkSize = s;
		chunkLoadRadius = r;
		playerPrefab = playerPre;
		ripPrefabs ();

		if (PolyServer.isActive) {
			foreach (PolyNetIdentity identity in GameObject.FindObjectsOfType<PolyNetIdentity>()) {
				spawnObject (identity);
			}
		} else if (PolyClient.isActive) {
			foreach (PolyNetIdentity identity in GameObject.FindObjectsOfType<PolyNetIdentity>()) {
				GameObject.Destroy (identity.gameObject);
			}
		}
	}

	public static void addPlayer(PolyNetPlayer p) {
		//load world
		p.refreshLoadedChunks ();
		//spawn player
		GameObject g = GameObject.Instantiate(playerPrefab.gameObject);
		g.transform.position = p.position;
		PolyNetIdentity i = g.GetComponent<PolyNetIdentity> ();
		i.owner = p;
		spawnObject (i);
	}

	public static ChunkIndex getChunkIndex(Vector3 position) {
		return new ChunkIndex ((int)Mathf.Floor(position.x / chunkSize), (int)Mathf.Floor(position.z / chunkSize));
	}

	public static PolyNetChunk getChunk(Vector3 position) {
		ChunkIndex i = getChunkIndex (position);
		PolyNetChunk chunk;
		if (chunks.TryGetValue (i, out chunk))
			return chunk;
		else {
			chunk = new PolyNetChunk (i);
			chunks.Add (i, chunk);
			return chunk;
		}
	}
		
	public static List<PolyNetChunk> getLoadedChunks(Vector3 position) {
		List<PolyNetChunk> chunkList = new List<PolyNetChunk> ();
		ChunkIndex i = getChunkIndex (position);
		ChunkIndex temp = new ChunkIndex(0,0);
		PolyNetChunk chunk;
		for (int z = -1 * chunkLoadRadius + 1; z < chunkLoadRadius; z++) {
			for (int x = -1 * chunkLoadRadius + 1; x < chunkLoadRadius; x++) {
				temp.z = z + i.z;
				temp.x = x + i.x;
				if (chunks.TryGetValue (temp, out chunk))
					chunkList.Add (chunk);
			}
		}
		return chunkList;
	}

	public static void ripPrefabs() {
		PrefabRegistry registry = GameObject.FindObjectOfType<PrefabRegistry> ();
		foreach (PolyNetIdentity g in registry.prefabs) {
			prefabs.Add (g.prefabId, g.gameObject);
		}
	}

	public static PolyNetIdentity getObject(int instanceId) {
		PolyNetIdentity i;
		if (objects.TryGetValue (instanceId, out i))
			return i;
		else
			return null;
	}

	public static void spawnObject(PolyNetIdentity i) {
		i.initialize (nextInstanceId);
		getChunk (i.transform.position).spawnObject (i);
		objects.Add (nextInstanceId, i);
		nextInstanceId++;
	}

	public static void despawnObject(PolyNetIdentity i) {
		getChunk (i.transform.position).despawnObject (i);
		objects.Remove (i.instanceId);
	}

	public static void spawnObject(int prefabId, int instanceId, int ownerPlayerId, Vector3 p, Vector3 s, Vector3 e) {
		GameObject prefab;
		if (prefabs.TryGetValue (prefabId, out prefab)) {
			GameObject instance = GameObject.Instantiate (prefab);
			instance.transform.position = p;
			instance.transform.localScale = s;
			instance.transform.eulerAngles = e;
			PolyNetIdentity identity = instance.GetComponent<PolyNetIdentity> ();
			identity.instanceId = instanceId;
			if (ownerPlayerId == GameObject.FindObjectOfType<PolyNetManager>().playerId)
				identity.isLocalPlayer = true;
			objects.Add (instanceId, identity);
		} else {
			Debug.Log ("Object spawn error: prefab not found for id: " + prefabId + ", ignoring spawn.");
		}
	}

	public static void despawnObject(int instanceId) {
		PolyNetIdentity obj;
		if (objects.TryGetValue(instanceId, out obj)) {
			GameObject.Destroy (obj.gameObject);
			objects.Remove (instanceId);
		} else {
			Debug.Log ("Object despawn error: instance not found for id: " + instanceId + ", ignoring despawn.");
		}
	}

	private static string convertPath(string path) {
		int i1 = path.IndexOf ("Resources/");
		i1 += 10;
		int i2 = path.IndexOf (".");
		int length = i2 - i1;
		return path.Substring (i1, length);
	}

}

public class ChunkIndex {
	public int x;
	public int z;
	public ChunkIndex(int ix, int iz) {
		x = ix;
		z = iz;
	}
	public override bool Equals(object obj) {
		ChunkIndex item = (ChunkIndex)obj;
		if (item == null) {
			return false;
		}

		return x == item.x && z == item.z;
	}

	public override int GetHashCode() {
		return x ^ z;
	}
}