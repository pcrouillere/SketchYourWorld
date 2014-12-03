using UnityEngine;
using System.Collections;

public class Level2 : MonoBehaviour 
{
	float time = 0.0f;
	
	// Use this for initialization
	void Start ()
	{
		Debug.Log("Start Level2");
	}
	
	// Update is called once per frame
	void Update () 
	{
		time += Time.deltaTime;
		
		if ( time > 2.0f )
		{
			// If using EchoFXEvent and have DontDestroyOnLoad set to _EchoCoreManager
			// you need to stop all EchoFXEvents before loading a new scene.
			EchoFXEvent.StopAllEvents();
			Application.LoadLevel("Level3");
		}
	}
}
