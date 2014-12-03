#pragma strict

// example to show how easy it is to start a dissolve effect 
// also shows u can use a faster shader/material when not dissolving and switch to dissolve when needed 
class SphereBrain extends EchoGameObject
{
	function Start () 
	{
		var efx:EchoFXEvent;
		
//		EchoFXEvent.Scroll_echoUV ( this, Vector4 ( 0.01, 0, 0.02, 0 ), 0 );
		
		efx = EchoFXEvent.Dissolve_echoShader ( this, -0.5, 1.5, 2.0 );
		efx.StartDelay ( 2.0, null );
		efx.SetEventStart ( SwapCallback );

		efx = EchoFXEvent.Dissolve_echoShader ( this, 1.5, -0.5, 2.0 );
		efx.StartDelay ( 5.0, null );
		efx.SetEventDone ( SwapCallback );
	}
	
	function SwapCallback()
	{
		SwapMaterial();
	}
	
}
