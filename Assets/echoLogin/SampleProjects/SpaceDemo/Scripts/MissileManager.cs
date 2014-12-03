using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class MissileManager : EchoPoolManager
{
	static EchoPoolList      mypool;

//===========================================================================
	void Start()
	{
		// i do this so i can call the static function below ( makes things easier )
		// if you make your functions non static just need to get a reference to this script
		mypool = echoObjectPool;
	}

//===========================================================================
	public static void Launch ( Vector3 istartpos, Vector3 idirection, Transform itarget, float ispeed )
	{
		MissileBrain brain = mypool.GetFree() as MissileBrain;

		if ( brain != null )
		{
			brain.Launch ( istartpos, idirection, itarget, ispeed );
		}
	}
}