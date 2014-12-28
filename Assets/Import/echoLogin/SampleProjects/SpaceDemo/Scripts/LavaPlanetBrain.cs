using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class LavaPlanetBrain : EchoGameObject
{
	//===========================================================================
	void Update()
	{
		cachedTransform.Rotate ( new Vector3 ( 0 ,Time.deltaTime * -1.8f ,0 ) );
	}

}

