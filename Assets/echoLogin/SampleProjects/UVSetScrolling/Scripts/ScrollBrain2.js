#pragma strict

//-----------------------------------------------------------------------------
class ScrollBrain2 extends EchoGameObject
{
//=============================================================================
	function Start () 
	{
		// 0.5 is center of texture UV minus a 1.0/8.0 ( cell8 )
		UVSetMakeScrollH ( 0.5f - ELC.CELL8, 256 );
		
		EchoFXEvent.CellAnimate_UVSet ( this, 0, 256, -120, 0, 0.0f );
	}
	
	
}