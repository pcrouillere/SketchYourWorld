#pragma strict

// example to show how easy it is to start an effect 
class Widescreen extends EchoGameObject
{
	function Start () 
	{
		var efx:EchoFXEvent;
		
		EchoFXEvent.Scroll_echoUV ( this, Vector4 ( 0.01, 0, 0.02, 0 ), 0 );
		
		// Sets the bottom layer, this is also how you would set an UV Event
		// to be used by a non echoLogin shader
		efx = EchoFXEvent.Scroll_echoUV ( this, Vector4 ( 0.001, 0, 0, 0 ), 0 );
		efx.Use_MainTex_ST();
	}
	
}

