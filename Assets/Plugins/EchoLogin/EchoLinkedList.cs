using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//-----------------------------------------------------------------------------
public class EchoListCallback
{
	public delegate void 			CreateNewObject();
	public event CreateNewObject 	callback;
	public bool  dynamicAdd 	= false;
	
//=============================================================================
	public void AddCallback ( CreateNewObject icallback )
	{
			callback = icallback;
	}
	
//=============================================================================
	public void CallCreateNewObject()
	{
		callback();
	}

};

//$-----------------------------------------------------------------------------
//@ Derive from this to make linked list class which keeps 2 lists of active and
//@ inactive objects.  
//@
//@ See phaser.cs in space demo for example of usage
//@ EchoFXEvent.cs also uses this class to keep track of events
//&-----------------------------------------------------------------------------
public class EchoLinkedList
{
	public static List<EchoLinkedList> active_firstList			= new List<EchoLinkedList>();
	public static List<EchoLinkedList> active_lastList			= new List<EchoLinkedList>();
	public static List<EchoLinkedList> inactive_firstList		= new List<EchoLinkedList>();
	public static List<EchoLinkedList> inactive_lastList		= new List<EchoLinkedList>();
	public static List<EchoListCallback> newObjCallbacks		= new List<EchoListCallback>();
	public static int echoPoolListCount							= 0;
	public EchoLinkedList active_last							= null;
	public EchoLinkedList inactive_last							= null;
	public EchoLinkedList active_next;
	public EchoLinkedList active_prev;
	public EchoLinkedList inactive_next;
	public EchoLinkedList inactive_prev;
	public delegate void 			CreateNewObject();


//--------------------------------------------------------------------------
	public static int PoolListAdd()
	{
		EchoListCallback elc;
		
		elc = new EchoListCallback();
		elc.dynamicAdd = false;
		
		newObjCallbacks.Add ( elc );
		
		active_firstList.Add ( new EchoLinkedList() );
		active_lastList.Add ( new EchoLinkedList() );

		active_firstList[echoPoolListCount].active_next			= active_lastList[echoPoolListCount];
		active_firstList[echoPoolListCount].active_prev			= null;
		active_lastList[echoPoolListCount].active_prev			= active_firstList[echoPoolListCount];
		active_lastList[echoPoolListCount].active_next			= null;

		inactive_firstList.Add ( new EchoLinkedList() );
		inactive_lastList.Add ( new EchoLinkedList() );

		inactive_firstList[echoPoolListCount].inactive_next		= inactive_lastList[echoPoolListCount];
		inactive_firstList[echoPoolListCount].inactive_prev		= null;
		inactive_lastList[echoPoolListCount].inactive_prev		= inactive_firstList[echoPoolListCount];
		inactive_lastList[echoPoolListCount].inactive_next		= null;

		echoPoolListCount++;

		return ( echoPoolListCount-1 );
	}

//$-----------------------------------------------------------------------------
//@ Initialize an item for an existing pool.
//&-----------------------------------------------------------------------------
	public void PoolListItemInit ( int ipoolListID )
	{
		// we do this so we dont have to access a indexed List
		// each time we move or acces an item from a List
		active_last												= active_lastList[ipoolListID];
		inactive_last											= inactive_lastList[ipoolListID];
	}
	
//$-----------------------------------------------------------------------------
//@ Set a callback that will create a new object for linked list
//@ So that the list can dynamicaly grow
//@
//@ Parameters:
//@
//# icallback  		 - Method to call
//&-----------------------------------------------------------------------------	
	public static void NewObjectCallback ( int ipoolListID, EchoListCallback.CreateNewObject icallback )
	{
		newObjCallbacks[ipoolListID].AddCallback ( icallback );
		newObjCallbacks[ipoolListID].dynamicAdd 	= true;
	}
	
//$-----------------------------------------------------------------------------
//@ Returns first item of active pool.
//&-----------------------------------------------------------------------------
	public static EchoLinkedList PoolGetFirstActive ( int ipoolListID )
	{
		return ( active_firstList[ipoolListID].active_next );	
	}

//$-----------------------------------------------------------------------------
//@ Returns last place holder item of active list.
//&-----------------------------------------------------------------------------
	public static EchoLinkedList PoolGetLastActive ( int ipoolListID )
	{
		return ( active_lastList[ipoolListID] );	
	}

//$--------------------------------------------------------------------------
//@ Returns first item of inactive pool.
//&--------------------------------------------------------------------------
	public static EchoLinkedList PoolGetFirstInactive ( int ipoolListID )
	{
		return ( inactive_firstList[ipoolListID].inactive_next );	
	}

//$--------------------------------------------------------------------------
//@ Returns last place holder item of inactive list.
//&--------------------------------------------------------------------------
	public static EchoLinkedList PoolGetLastInactive ( int ipoolListID )
	{
		return ( inactive_lastList[ipoolListID] );	
	}

//$--------------------------------------------------------------------------
//@ Returns free inactive item.
//&--------------------------------------------------------------------------
	public static EchoLinkedList PoolGetFreeInactive ( int ipoolListID )
	{
		EchoLinkedList epl										= inactive_firstList[ipoolListID].inactive_next;
		
		if ( epl == null || epl.inactive_next == null )
		{
			if ( newObjCallbacks[ipoolListID].dynamicAdd )
			{
				newObjCallbacks[ipoolListID].CallCreateNewObject();
				epl	= inactive_firstList[ipoolListID].inactive_next;
			}
			else
				return ( null );
		}

		// remove from inactive list
		epl.inactive_prev.inactive_next							= epl.inactive_next;
		epl.inactive_next.inactive_prev							= epl.inactive_prev;

		// add to active
		epl.active_next											= epl.active_last;
		epl.active_prev											= epl.active_last.active_prev;

		epl.active_last.active_prev.active_next					= epl;
		epl.active_last.active_prev								= epl;

		return epl;
	}

//$--------------------------------------------------------------------------
//@ Adds item to inactive list
//&--------------------------------------------------------------------------
	public static void PoolAddListInactive ( EchoLinkedList epl )
	{
		epl.inactive_next										= epl.inactive_last;
		epl.inactive_prev										= epl.inactive_last.inactive_prev;

		epl.inactive_last.inactive_prev.inactive_next			= epl;
		epl.inactive_last.inactive_prev							= epl;
	}

//$--------------------------------------------------------------------------
//@ Removes item from inactive list
//&--------------------------------------------------------------------------
	public static void PoolRemoveListInactive ( EchoLinkedList epl )
	{
		epl.inactive_prev.inactive_next							= epl.inactive_next;
		epl.inactive_next.inactive_prev							= epl.inactive_prev;
	}

//$--------------------------------------------------------------------------
//@ Adds item to active list
//&--------------------------------------------------------------------------
	public static void PoolAddListActive ( EchoLinkedList epl )
	{
		epl.active_next											= epl.active_last;
		epl.active_prev											= epl.active_last.active_prev;

		epl.active_last.active_prev.active_next					= epl;
		epl.active_last.active_prev								= epl;
	}

//$--------------------------------------------------------------------------
//@ Removes item from active list
//&--------------------------------------------------------------------------
	public static void PoolRemoveListActive ( EchoLinkedList epl )
	{
		epl.active_prev.active_next								= epl.active_next;
		epl.active_next.active_prev								= epl.active_prev;
	}

//$--------------------------------------------------------------------------
//@ Moves item from active to inactive list
//&--------------------------------------------------------------------------
	public static void PoolMoveActive2Inactive ( EchoLinkedList epl )
	{
		epl.active_prev.active_next								= epl.active_next;
		epl.active_next.active_prev								= epl.active_prev;

		epl.inactive_next										= epl.inactive_last;
		epl.inactive_prev										= epl.inactive_last.inactive_prev;

		epl.inactive_last.inactive_prev.inactive_next			= epl;
		epl.inactive_last.inactive_prev							= epl;
	}

//$--------------------------------------------------------------------------
//@ Moves item from inactive to active list
//&--------------------------------------------------------------------------
	public static void PoolMoveInactive2Active ( EchoLinkedList epl )
	{
		epl.inactive_prev.inactive_next							= epl.inactive_next;
		epl.inactive_next.inactive_prev							= epl.inactive_prev;

		epl.active_next											= epl.active_last;
		epl.active_prev											= epl.active_last.active_prev;

		epl.active_last.active_prev.active_next					= epl;
		epl.active_last.active_prev								= epl;
	}

//===========================================================================
//  This is in case we want to Update all objects in a list
// 	w/o having to cast to the extended class
//===========================================================================
	public virtual void ProcessInUpdate()
	{
	}

}