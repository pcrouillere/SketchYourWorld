using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour {
	public GameObject myPerso;
	public GameObject drawingMenu;
	public GameObject interfaceCamera;
	[HideInInspector]public static GlobalManager gManager;
	/* Peut importe le scripte ou on est, gManager va appelé ce scripte la.
	 C'est déclaré de manière statique (c'est pratique).
	 */
	void Awake() {
		gManager = this;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.G)) {
			if(!drawingMenu.activeSelf) {
				drawingMenu.SetActive(true);
				SetMouseLook(false);
			}
			else {
				drawingMenu.SetActive(false);
				SetMouseLook(true);
			}
		}
	}

	public void SetMouseLook(bool b) {
		Camera.main.GetComponent<MouseLook> ().enabled = b;
			/* set active pour game object */
		myPerso.GetComponent<MouseLook> ().enabled = b;
	}

}
