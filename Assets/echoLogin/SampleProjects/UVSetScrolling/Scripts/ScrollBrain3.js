#pragma strict

//-----------------------------------------------------------------------------
class ScrollBrain3 extends EchoGameObject
{
	var efx:EchoFXEvent;
	var delay:float;
	var framerate:float = 120.0f;

//=============================================================================
	function Start () 
	{
		UVSetMakeScrollV ( 1.0f - ELC.CELL4, 256 );

		efx = EchoFXEvent.CellAnimate_UVSet ( this, 0, 256, 120.0f, 0, 0.0 );
		
		efx.UVSetSpeed ( framerate );
		efx.UVSetDamp ( 2 );   // will reverse at 2x the framerate 0 == instant
	}
		
//=============================================================================
	function OnMouseDown()
	{
		framerate = 0.0;
		efx.UVSetSpeed ( framerate );
	}
		
//=============================================================================
	function OnMouseUp()
	{
		framerate = 120.0;
		efx.UVSetSpeed ( framerate );
	}

//=============================================================================
	function Update()
	{
		delay += Time.deltaTime;
		
		if ( delay > 6.0f && !Mathf.Approximately ( framerate, 0.0 ) )
		{
			delay = 0;
			framerate *= -1;
			efx.UVSetSpeed ( framerate );
		}
	}
}