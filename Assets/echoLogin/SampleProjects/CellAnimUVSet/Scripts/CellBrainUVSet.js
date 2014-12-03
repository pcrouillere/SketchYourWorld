#pragma strict

// This is an example of cell animation on a polygon face using preallocated UV Sets. 
// This is faster than using per-object shader properties because it will not result in additional draw calls
//-----------------------------------------------------------------------------
class CellBrainUVSet extends EchoGameObject
{
	private var _time:float;
	
//=============================================================================
	function Start () 
	{
		// allocate 4 UV sets - this should be done on startup
		UVSetMake ( ELC.CELL2, ELC.CELL2, 2, 4 );
		
		// this is one way to do it automaticaly using EchoFXEvent
		// if you uncomment this uncomment the return in update
		//EchoFXEvent.CellAnimate_UVSet ( this, Random.Range ( 0, 4 ), 4, 0.15, 0 );
	}
	
//=============================================================================
	function Update()
	{
		_time+=Time.deltaTime;
		
		if ( _time > 0.1 )
		{
			_time = 0;
			// or you can set the UVSet yourself  
			UVSet ( Random.Range ( 0, 4 ) );
		}
	
	} 
	
}