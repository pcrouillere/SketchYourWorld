using UnityEngine;
using System.Collections;

public class ExitKey : MonoBehaviour {
	public string levelToLoad;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name == "Key") {
			Application.LoadLevel(levelToLoad);		
		}
	
	}
}
