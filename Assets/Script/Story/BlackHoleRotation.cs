using UnityEngine;
using System.Collections;

public class BlackHoleRotation : MonoBehaviour {
	public bool isActive;

	// Use this for initialization
	void Start () {
		isActive = false;
	
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.Rotate (Vector3.right,Time.deltaTime);
		if (gameObject.activeSelf && !isActive) {
			animation.Play("BlackHoleAnimation");
			isActive = true;
				}
	}
}
