using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	void OnControllerColliderHit(ControllerColliderHit collision){
		if (collision.gameObject.tag == "Exit") {
			Application.LoadLevel(collision.gameObject.GetComponent<ExitLevel>().levelToLoad);
				
		
		}
		}

	

}