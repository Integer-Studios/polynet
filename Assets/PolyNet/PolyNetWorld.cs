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
		p.identity = i;
		spawnObject (i);
	}

	public static void removePlayer(PolyNetPlayer p) {
		p.unloadChunks ();
		despawnObject (p.identity);
		GameObject.Destroy (p.identity.gameObject);
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

	public static GameObject getPrefab(int prefabId) {
		GameObject i;
		if (prefabs.TryGetValue (prefabId, out i))
			return i;
		else
			return null;
	}

	public static PolyNetIdentity getObject(int instanceId) {
		PolyNetIdentity i;
		if (objects.TryGetValue (instanceId, out i))
			return i;
		else
			return null;
	}

	public static void spawnObject(PolyNetIdentity i) {
		spawnObject (i, nextInstanceId);
		nextInstanceId++;
	}

	public static void spawnObject(PolyNetIdentity i, int instanceId) {
		i.initialize (instanceId);
		if (PolyServer.isActive)
			getChunk (i.transform.position).spawnObject (i);
		objects.Add (instanceId, i);
	}

	public static void despawnObject(PolyNetIdentity i) {
		if (PolyServer.isActive)
			getChunk (i.transform.position).despawnObject (i);
		objects.Remove (i.instanceId);
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