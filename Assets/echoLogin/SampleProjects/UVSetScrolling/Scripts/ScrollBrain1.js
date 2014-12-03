#pragma strict

//-----------------------------------------------------------------------------
class ScrollBrain1 extends EchoGameObject
{
//=============================================================================
	function Start () 
	{
		// allocate 512 UV sets - this should be done on startup
		// look at scroll texture to see how it looks
		// and how model is UV mapped to starting point
		UVSetMakeScrollH ( 0.5f - ELC.CELL16, 512 );
		
		EchoFXEvent.CellAnimate_UVSet ( this, 0, 512, 60.0, 0, 0.0f );
	}
	
	
}