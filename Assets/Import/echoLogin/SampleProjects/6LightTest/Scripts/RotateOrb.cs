using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class RotateOrb : EchoGameObject
{
	public Vector3 turnSpeed = new Vector3 ( 0,0,0 );
	public Vector4 scrollUV = new Vector4 ( 0,0,0,0 ); // only x and y are used for this exmaple;
	
	void Start()
	{
		if ( scrollUV.x != 0 || scrollUV.y != 0 )
			EchoFXEvent.Scroll_echoUV ( this, scrollUV, 0 );
	}
	
	//===========================================================================
	void Update()
	{
		cachedTransform.Rotate ( turnSpeed * Time.smoothDeltaTime );
	}

}
