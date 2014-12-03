#pragma strict

// example of cell animation on a polygon face
class CellBrain extends EchoGameObject
{
	function Start () 
	{
		// smooth blend cell animation
		EchoFXEvent.CellAnimateBlend_echoUV ( this, ELC.CELL4, ELC.CELL4, 4, 0, 16, 0.55, 0 );
		
		// normal cell animation
		//EchoFXEvent.CellAnimate_echoUV ( this, ELC.CELL4, ELC.CELL4, 4, 0, 16, 0.55, 0 );
	}
	
}
