#pragma strict

class ShieldBrain extends EchoGameObject
{
	public var shieldEnergy:float;
	public var curShieldHit:int; // can be 0-3
	
	//=============================================================================
	function OnCollisionEnter ( col:Collision )
	{
		var hitdir:Vector3;
		
		// hit direction vector
		hitdir = cachedTransform.position - col.transform.position;
		
		// start a timed hit effect 
		ShieldHit ( curShieldHit, hitdir, 0.25f );
		
		// set to use the next shield slot for effect ( 4 max )
		curShieldHit = ( curShieldHit + 1 ) % 4;
		
		shieldEnergy-=0.1f;
		
		if ( shieldEnergy <= 0.0 )
		{
			EchoFXEvent.ShieldDown_echoShader ( this, 0.5f, 2.0f );
		}
		
	}
	
	//=============================================================================
	function Start() 
	{
		shieldEnergy = 1.0;
		curShieldHit = 0;
		Application.targetFrameRate = 60;
	}

	//=============================================================================
	// ishieldhitid values can be 0,1,2 or 3
	// ishootdir is a direction vector
	//=============================================================================
	function ShieldHit ( ishieldhitid:int, ishootdir:Vector3, iduration:float )
	{
		// Here we start 2 effects going, and save thier id\reference in shieldFX1 and shieldFX2 arrays
	
		EchoFXEvent.ShieldHit_echoShader ( this, ishieldhitid, ishootdir, 0.01f, 0.1, iduration );
		EchoFXEvent.Random_echoUV ( this, iduration );
	}

}

