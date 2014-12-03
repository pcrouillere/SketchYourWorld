#pragma strict


#pragma strict

// example to show how to make lava on a shiny rock object 
class RockBrain extends EchoGameObject
{
	function Start () 
	{
		var efx:EchoFXEvent;
		
		EchoFXEvent.Scroll_echoUV ( this, Vector4 ( 0.0, 0.0, 0.0, 0.01 ), 0 );
	}
	
	function Update()
	{
		cachedTransform.Rotate ( Vector3 ( 0 ,Time.deltaTime * 0.8f ,0 ) );
	}

}

