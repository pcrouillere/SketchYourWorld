using UnityEngine;
using System.Collections;

public class SwitchToMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Call Menu");
		Application.LoadLevel("Menu");
	}
}
