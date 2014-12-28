using UnityEngine;
using System.Collections;

public class DragCounter : MonoBehaviour {

	public int DragCount;

	// Use this for initialization
	void Start () {
		DragCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (DragCount == 6) {
			Application.LoadLevel("End");	
				}
	}

	public void incrementeDragCount()
	{
		DragCount++;
		Debug.Log (DragCount);
		GetComponent<AudioSource> ().Play ();

	}
	
}
