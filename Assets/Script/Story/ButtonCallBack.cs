using UnityEngine;
using System.Collections;

public class ButtonCallBack : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startCallback()
	{
		Debug.Log("Load first level");
	}

	public void helpCallback()
	{
		Debug.Log("Load Help page");
	}
}
