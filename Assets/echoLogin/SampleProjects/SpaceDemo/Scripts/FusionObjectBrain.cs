using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class FusionObjectBrain : EchoGameObject
{
	private Transform _camtran;
	
//===========================================================================
	void Start()
	{
		_camtran 	= Camera.main.transform;
	}

//===========================================================================
	void Update()
	{
		cachedTransform.eulerAngles = new Vector3 ( 0.0f, 0.0f, _camtran.eulerAngles.z );
	}
}