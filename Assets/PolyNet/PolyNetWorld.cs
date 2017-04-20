﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyNetWorld {

	private static Dictionary<int, GameObject> prefabs = new Dictionary<int, GameObject>();
	private static Dictionary<int, PolyNetIdentity> objects = new Dictionary<int, PolyNetIdentity>();
	public static Dictionary<ChunkIndex, PolyNetChunk> chunks = new Dictionary<ChunkIndex, PolyNetChunk>();
	private static float chunkSize = 10f;

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

	public static void addPlayerTemp(PolyNetPlayer p) {
		PolyNetChunk chunk;
		if (chunks.TryGetValue (new ChunkIndex (Random.Range(0,2), Random.Range(0,2)), out chunk)) {
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
		return new ChunkIndex ((int)(position.x / chunkSize), (int)(position.z / chunkSize));
	}

	public static void ripPrefabs() {
		PrefabRegistry registry = GameObject.FindObjectOfType<PrefabRegistry> ();
		foreach (PolyNetIdentity g in registry.prefabs) {
			prefabs.Add (g.prefabId, g.gameObject);
		}
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

public struct ChunkIndex {
	public int x;
	public int z;
	public ChunkIndex(int ix, int iz) {
		x = ix;
		z = iz;
	}
}