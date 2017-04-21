using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocknBalls : PolyNetBehaviour {

	// Use this for initialization
	void Start () {
		if (!FindObjectOfType<PolyNetManager>().isClient)
			StartCoroutine (fuck());
	}
	
	// Update is called once per frame
	void Update () {
		if (PolyServer.isActive)
			transform.Translate (new Vector3 (0, 0, 5 * Time.deltaTime));
	}

	public IEnumerator fuck() {
		yield return new WaitForSeconds (1f);
		while (true) {
			identity.sendBehaviourPacket (new PacketTransform(this));
			yield return new WaitForSeconds (1f/9f);
		}
	}

	public override void handleBehaviourPacket (PacketBehaviour p) {
		base.handleBehaviourPacket (p);
		if (p.id == 2) {
			PacketTransform t = (PacketTransform)p;
			transform.position = t.position;
			transform.eulerAngles = t.euler;
			transform.localScale = t.scale;
		}
	}

}
