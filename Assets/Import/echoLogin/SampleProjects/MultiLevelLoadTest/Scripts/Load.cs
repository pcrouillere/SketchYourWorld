using UnityEngine;
using System.Collections;

public class Load : MonoBehaviour
{
	float time = 0.0f;
	public GameObject dd;
	
	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad ( dd );
	}
	
	// Update is called once per frame
	void Update () 
	{
		time += Time.deltaTime;
		
		if ( time > 1.0f )
		{
			Application.LoadLevel("Level1");
		}
	}
}
