using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocknBalls : PolyNetBehaviour {

	// Use this for initialization
	void Start () {
		if (identity.isLocalPlayer) {
			Camera.main.transform.SetParent (transform);
			Camera.main.transform.rotation = Quaternion.identity;
			Camera.main.transform.position = new Vector3 (0f, 3f, -3f);
			StartCoroutine (networkTransform ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!identity.isLocalPlayer)
			return;
		
		transform.Translate (new Vector3 (5 * Time.deltaTime * Input.GetAxis("Horizontal"), 0, 5 * Time.deltaTime * Input.GetAxis("Vertical")));
	}

	public IEnumerator networkTransform() {
		yield return new WaitForSeconds (1f);
		while (true) {
			identity.sendBehaviourPacket (new PacketTransform(this));
			yield return new WaitForSeconds (1f/9f);
		}
	}

	public override void handleBehaviourPacket (PacketBehaviour p) {
		base.handleBehaviourPacket (p);
		if (p.id == 2) {
			if (!identity.isLocalPlayer) {
				PacketTransform t = (PacketTransform)p;
				transform.position = t.position;
				transform.eulerAngles = t.euler;
				transform.localScale = t.scale;
			}
			if (PolyServer.isActive)
				identity.sendBehaviourPacket (p);
		}
	}

}
