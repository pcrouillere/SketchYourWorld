using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//$-----------------------------------------------------------------------------
//@ This keeps 2 lists of pre-allocated active and inactive objects  
//&-----------------------------------------------------------------------------
public class EchoPoolList
{
	public EchoGameObject active_last	= null;
	public EchoGameObject active_first	= null;
	public EchoGameObject inactive_first	= null;
	public EchoGameObject inactive_last	= null;
	private GameObject _go1;
	private GameObject _go2;
	private GameObject _go3;
	private GameObject _go4;

//===========================================================================
	public EchoPoolList ()
	{
		_go1 = new GameObject("Pool1");
		_go2 = new GameObject("Pool2");
		_go3 = new GameObject("Pool3");
		_go4 = new GameObject("Pool4");

		// setup active list
		active_first			= _go1.AddComponent<EchoGameObject>();
		active_last				= _go2.AddComponent<EchoGameObject>();

		active_first.echoListNext	= active_last;
		active_first.echoListPrev	= null;

		active_last.echoListNext	= null;
		active_last.echoListPrev	= active_first;

		// setup inactive list
		inactive_first	= _go3.AddComponent<EchoGameObject>();
		inactive_last	= _go4.AddComponent<EchoGameObject>();

		inactive_first.echoListNext	= inactive_last;
		inactive_first.echoListPrev	= null;

		inactive_last.echoListNext	= null;
		inactive_last.echoListPrev	= inactive_first;
	}

//$-----------------------------------------------------------------------------
//@ Adds new object to pool 
//@
//@ NOTE: This is normally handled by EchoGameObject. No user need to call this.
//@
//@ Parameters:
//@
//# iec 		- EchoGameObject object to add
//&-----------------------------------------------------------------------------
	public void AddNewObject ( EchoGameObject iec )
	{
		iec.echoListNext					= inactive_last;
		iec.echoListPrev					= inactive_last.echoListPrev;

		inactive_last.echoListPrev.echoListNext	= iec;
		inactive_last.echoListPrev				= iec;
	}

	public  EchoGameObject Inactive2Active()
	{
		return GetFree();
	}
	
//$-----------------------------------------------------------------------------
//@ Gets an inactive object for use from the object pool 
//@
//@ NOTE: This should be called from your class derived from EchoPoolManager.
//@
//&-----------------------------------------------------------------------------
	public  EchoGameObject GetFree()
	{
	   EchoGameObject ec = inactive_first.echoListNext;

		if ( ec == inactive_last )
			return ( null );

		ec.echo_PoolActiveList = true;

		// remove from inactive list
		ec.echoListPrev.echoListNext	= ec.echoListNext;
		ec.echoListNext.echoListPrev	= ec.echoListPrev;

		// add to active
		ec.echoListNext		= active_last;
		ec.echoListPrev		= active_last.echoListPrev;

		active_last.echoListPrev.echoListNext	= ec;
		active_last.echoListPrev				= ec;

		return ( ec );
	}

	public void Active2Inactive ( EchoGameObject iec )
	{
		ReturnPool ( iec );
	}
	
//$-----------------------------------------------------------------------------
//@ Moves object from active to inactive list 
//@
//@ NOTE: This is normally handled by EchoGameObject. No user need to call this.
//@
//@ Parameters:
//@
//# iec 		- EchoGameObject object to move
//&-----------------------------------------------------------------------------
	public void ReturnPool ( EchoGameObject iec )
	{
		if ( iec.echo_PoolActiveList == false )
			return;

		iec.echo_PoolActiveList = true;

		// remove from active list
		iec.echoListPrev.echoListNext	= iec.echoListNext;
		iec.echoListNext.echoListPrev	= iec.echoListPrev;

		// add to inactive
		iec.echoListNext		= inactive_last;
		iec.echoListPrev		= inactive_last.echoListPrev;

		inactive_last.echoListPrev.echoListNext	= iec;
		inactive_last.echoListPrev				= iec;
	}
}