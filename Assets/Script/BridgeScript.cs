using UnityEngine;
using System.Collections;

public class BridgeScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider other) {
		Debug.Log ("qqchose");
		if (other.gameObject.name=="Bridge(Clone)") {
						Destroy (other.gameObject);
			gameObject.GetComponent<MeshRenderer>().enabled=true;
			gameObject.GetComponent<MeshCollider>().isTrigger=false;
				}

	}
}
