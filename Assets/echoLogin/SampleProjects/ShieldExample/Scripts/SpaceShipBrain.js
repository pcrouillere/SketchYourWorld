#pragma strict

class SpaceShipBrain extends EchoGameObject
{
	public var hitPoints:int;
	
		//=============================================================================
	function OnCollisionEnter ( col:Collision )
	{
		var efx:EchoFXEvent;
	
		// cachedTransform is part of EchoGameObject
		efx = EchoFXEvent.ShakeGameObject ( cachedTransform, 0.01, 0.3 );
		efx.AddFilter ( EchoFilter.SINE, 0.5, 1.0, 0.0 );

		hitPoints--;
		
		// using EchoActive because it works same in unity 3.5 and 4.0
		if ( hitPoints <= 0 )
			EchoActive ( false );
	}
		
	//=============================================================================
	function Start() 
	{
		hitPoints = 8;
	}

}

