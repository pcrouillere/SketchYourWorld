using UnityEngine;
using System.Collections;

public class LostLevel : MonoBehaviour {
	public string thisLevel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.transform.position.y < -50) {
			if(!gameObject.GetComponent<AudioSource>().isPlaying)
				gameObject.GetComponent<AudioSource>().Play ();		
		}
		if (gameObject.transform.position.y < -80) {
			Application.LoadLevel(thisLevel);	
		}
	}
}
