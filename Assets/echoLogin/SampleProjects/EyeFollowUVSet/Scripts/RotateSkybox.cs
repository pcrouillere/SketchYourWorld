using UnityEngine;
using System.Collections;

public class RotateSkybox : EchoGameObject {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		cachedTransform.Rotate ( new Vector3 (  0.0f ,Time.deltaTime * 0.5f, 0.0f ) );
	}
}
