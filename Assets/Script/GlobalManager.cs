using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour {
	//public GameObject myPerso;
	public GameObject drawingMenu;
	public GameObject allMenu;
	public GameObject interfaceCamera;
	public bool canMove = true;
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
		if (SixenseInput.Controllers[1].GetButtonDown(SixenseButtons.ONE)) {
			if(!drawingMenu.activeSelf) {
				drawingMenu.SetActive(true);
			}
			else {
				drawingMenu.SetActive(false);
			}
		}

		if (SixenseInput.Controllers [1].GetButtonDown (SixenseButtons.START)) {
			if(!allMenu.activeSelf) {
				allMenu.SetActive(true);
			}
			else {
				allMenu.SetActive(false);
			}
		
		}
	}


}
