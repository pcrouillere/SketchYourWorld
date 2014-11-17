using UnityEngine;
using System.Collections;

public class ObjectBehavior : MonoBehaviour {
	public bool isSelected = false;
	public int speed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isSelected) {
			if (Input.GetKey("m")) {
				transform.Translate(Vector3.right*speed, Camera.main.transform);
			}
			else if(Input.GetKey("l")){
				transform.Translate(Vector3.forward*speed, Camera.main.transform);
			}
			else if(Input.GetKey("k")){
				transform.Translate(Vector3.left*speed, Camera.main.transform);
			}
			else if(Input.GetKey("j")){
				transform.Translate(Vector3.back*speed, Camera.main.transform);
			}
			else if(Input.GetKey("d")) {
				transform.Rotate(Vector3.back);
			}
			else if(Input.GetKey("t")) {
				transform.Rotate(Vector3.forward);
			}
			else if(Input.GetKey("v")) {
				Debug.Log("+");
				transform.localScale += new Vector3(0.1F, 0.1F, 0.1F);
			}
			else if(Input.GetKey("w")) {
				Debug.Log("-");
				if(transform.localScale.x >= 0.3) {
					transform.localScale -= new Vector3(0.1F, 0.1F, 0.1F);
				}
			}
		}
	}
	void OnMouseDown() {
		Debug.Log("I have been clicked");
		isSelected = !isSelected;
	}

}
