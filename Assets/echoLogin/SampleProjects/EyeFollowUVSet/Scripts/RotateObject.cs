using UnityEngine;
using System.Collections;

public class RotateObject : EchoGameObject {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		cachedTransform.Rotate ( new Vector3 (  0.0f ,0.0f,Time.deltaTime * -64.0f ) );
	}
}
