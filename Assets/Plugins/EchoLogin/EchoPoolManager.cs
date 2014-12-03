using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//$-----------------------------------------------------------------------------
//@ Derive from this to make a pool manager script for an EchoGameObject.
//@
//@ Options:
//@
//# Number Of Pool Objects  - Maximum number of objects in pool
//# Game Object Prefab      - GameObject to instantiate 
//@ 
//@ Example:
//@ 
//%class MissileManager : EchoPoolManager
//%{
//%		static EchoPoolList      mypool;
//%
//@//===========================================================================
//%		void Start()
//%		{
//%				mypool = echoObjectPool;
//%		}
//%
//@//===========================================================================
//%		public static void Launch ( Vector3 istartpos, Vector3 idirection, Transform itarget, float ispeed )
//%		{
//%				MissileBrain brain = mypool.Inactive2Active() as MissileBrain;
//%
//%				if ( brain != null )
//%				{
//%						brain.Launch ( istartpos, idirection, itarget, ispeed );
//%				}
//%		}
//%}
//&-----------------------------------------------------------------------------
public class EchoPoolManager : MonoBehaviour
{
	[HideInInspector]
	public EchoPoolList      echoObjectPool;
	public int               NumberOfPoolObjects		= 2;
	public GameObject        GameObjectPrefab			= null;

//===========================================================================
	void Awake()
	{
		int 			loop;
		GameObject 		go;
		EchoGameObject 	ego;
		Transform       child;
		
		// new pool object to manage the linked list of objects
		echoObjectPool = new EchoPoolList();

		// makes the new object to be used in pool
		for ( loop = 0; loop < NumberOfPoolObjects; loop++ )
		{
			go	= UnityEngine.Object.Instantiate ( GameObjectPrefab ) as GameObject;
			go.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

#if !UNITY_3_5
				go.SetActive ( true );
#else
				go.active = true;
#endif
			
			for ( int index = 0; index < go.transform.childCount; index++ )
			{
				child = go.transform.GetChild(index);
			
#if !UNITY_3_5
				child.gameObject.SetActive ( true );
#else
				child.gameObject.active = true;
#endif
			}
			
			
			// seems if we dont do this, it uses new materials for each new object
			go.renderer.sharedMaterial	= GameObjectPrefab.renderer.sharedMaterial;
			ego							= go.GetComponent<EchoGameObject>();

			// Adds new object to inactive pool list and deactivates it and all children
			ego.EchoPoolInit( echoObjectPool );
			ego.EchoPoolObjectInit();
		}
	}
}