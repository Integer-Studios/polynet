using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyNetWorld {

	private static Dictionary<int, GameObject> prefabs = new Dictionary<int, GameObject>();
	private static Dictionary<int, PolyNetIdentity> objects = new Dictionary<int, PolyNetIdentity>();
	public static Dictionary<ChunkIndex, PolyNetChunk> chunks = new Dictionary<ChunkIndex, PolyNetChunk>();
	private static float chunkSize = 50f;

	public static void initialize() {
		ripPrefabs ();
		if (PolyServer.isActive) {
			int nextId = 0;
			foreach (PolyNetIdentity identity in GameObject.FindObjectsOfType<PolyNetIdentity>()) {
				identity.initialize (nextId);
				objects.Add (nextId, identity);
				nextId++;
			}
		} else if (PolyClient.isActive) {
			foreach (PolyNetIdentity identity in GameObject.FindObjectsOfType<PolyNetIdentity>()) {
				GameObject.Destroy (identity.gameObject);
			}
		}
	}

	public static PolyNetChunk linkChunk(PolyNetIdentity i) {
		PolyNetChunk chunk;
		ChunkIndex index = getChunkIndex (i.gameObject.transform.position);
		if (!chunks.TryGetValue (index, out chunk)) {
			chunk = new PolyNetChunk (index);
			chunks.Add (index, chunk);
			Debug.Log ("chunk created at index: " + index.x + ", " + index.z);
		}
		chunk.addObject (i);
		Debug.Log ("object added to chunk: " + index.x + ", " + index.z + " with id: " + i.instanceId);
		return chunk;
	}

	public static PolyNetChunk migrateChunk(PolyNetIdentity i, PolyNetChunk c) {
		c.removeObject (i);
		PolyNetChunk n = linkChunk (i);
		n.migrateChunk (i, c);
		return n;
	}

	public static void addPlayerTemp(PolyNetPlayer p) {
		PolyNetChunk chunk;
//		foreach (PolyNetChunk c in chunks.Values) {
//			c.addPlayer (p);
//		}
		if (chunks.TryGetValue (new ChunkIndex (0, 0), out chunk)) {
			chunk.addPlayer (p);
//			GameObject.FindObjectOfType<PrefabRegistry>().StartCoroutine (fakeMove (p));
		}
	}

//	private static IEnumerator fakeMove(PolyNetPlayer p) {
//		yield return new WaitForSeconds(5f);
//		PolyNetChunk chunk;
//		if (chunks.TryGetValue (new ChunkIndex (0, 0), out chunk)) {
//			chunk.removePlayer (p);
//			if (chunks.TryGetValue (new ChunkIndex (1, 0), out chunk)) {
//				chunk.addPlayer (p);
//			}
//		}
//	}

	public static ChunkIndex getChunkIndex(Vector3 position) {
		return new ChunkIndex ((int)Mathf.Floor(position.x / chunkSize), (int)Mathf.Floor(position.z / chunkSize));
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

	public static void spawnObject(int prefabId, int instanceId, Vector3 p, Vector3 s, Vector3 e) {
		GameObject prefab;
		if (prefabs.TryGetValue (prefabId, out prefab)) {
			GameObject instance = GameObject.Instantiate (prefab);
			instance.transform.position = p;
			instance.transform.localScale = s;
			instance.transform.eulerAngles = e;
			PolyNetIdentity identity = instance.GetComponent<PolyNetIdentity> ();
			identity.instanceId = instanceId;
			objects.Add (instanceId, identity);
		} else {
			Debug.Log ("Object spawn error: prefab not found for id: " + prefabId + ", ignoring spawn.");
		}
	}

	public static void despawnObject(int instanceId) {
		PolyNetIdentity obj;
		if (objects.TryGetValue(instanceId, out obj)) {
			GameObject.Destroy (obj.gameObject);
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