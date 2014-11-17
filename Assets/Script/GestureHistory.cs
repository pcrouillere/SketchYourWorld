using UnityEngine;
using System.Collections;

public class GestureHistory : MonoBehaviour {

	void OnTap(TapGesture gesture) { 
		if (gesture.Selection) {
			Debug.Log("test");
			if ( gesture.Selection.name == "Accueil" ) {
				Debug.Log ("Accueil has been taped");
			}
		}
	}
}
