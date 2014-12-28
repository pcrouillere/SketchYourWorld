using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//-------------------------------------------------------------------------------------
class LineRenObj
{
	public LineRenderer lr;
	public GameObject   go;
	public Transform    gotransform;
	public float 		width1;
	public float 		width2;
	public float 		sectionCount;
};

//===========================================================================
// Example of how to use the EchoLinkedList 
// Look at EchoFXEvent.cs for anopther example for EchoLinkedList use
//===========================================================================
class Phaser : EchoLinkedList
{
	static				int 				poolListID;  // make one of this for each class extended from EchoPoolList
	static 				Material			laserMat;
	static              int 				curShieldHit = 0;
	public				List<LineRenObj> 	lineListSlow = new List<LineRenObj>();
	public				LineRenObj[] 		lineList;
	public				int 				lineListCount;
	EchoFXEvent		 	efx1;
	EchoFXEvent		 	efx2;
	int					laser1ID;
	float				dur;
	float				per;
	float				time;
	int					stage;
	int                 shieldHitID;
	EchoGameObject      ego;

//--------------------------------------------------------------------------
	public Phaser()
	{
		// Setup each item for this classes PoolList
		PoolListItemInit ( poolListID );

		// Add to the "not in use" pool list
		PoolAddListInactive ( this );
	}

//--------------------------------------------------------------------------
// Call this with number of max Phasers at once at startup
// when using pool class you should not make objects on your own
// this function will be create all objects needed
//--------------------------------------------------------------------------
	public static void AllocPool ( int iNumPoolObjs, Material ilasermat )
	{
		Phaser flb;

		laserMat = ilasermat;

		// we have to call AddPoolList for each class extended from EchoPoolList
		// ONCE At Start so they have thier own set of variables to keep 
		// track of stuff and keep track of this classes poolListID
		poolListID = PoolListAdd();

		while ( iNumPoolObjs-- > 0 )
		{
			flb = new Phaser();
			flb.ego = new GameObject().AddComponent<EchoGameObject>();
			flb.ego.Init ( new GameObject(), true, false );
			flb.ego.EchoActive ( false );
			flb.laser1ID 	= flb.LaserAdd ( 0.1f, 0.02f, new Color ( 0.1f, 0.2f, 0.8f, 1.0f ), new Color ( 0.8f, 0.6f, 0.6f, 0.5f ), 6 );
			flb.LaserFinalize();
		}
	}

//--------------------------------------------------------------------------
	public static void ProcessAllInUpdate()
	{
		EchoLinkedList first	= PoolGetFirstActive ( poolListID );
		EchoLinkedList last		= PoolGetLastActive ( poolListID );
		EchoLinkedList next;
		EchoLinkedList epl;

		// loop thru active events only 
		for ( epl = first; epl != last; epl = next )
		{
			next = epl.active_next;  // store this in case epl removes itself
			epl.ProcessInUpdate();	
		}
	}

//--------------------------------------------------------------------------
	public static void Shoot ( EchoGameObject goex_shootparent, Vector3 emitposition, Vector3 shootdirection )
	{
		LineRenObj		 lro;
		RaycastHit		 rhit;
		int				 loop;

		if ( Physics.Raycast ( emitposition, shootdirection, out rhit, 128 ) )
		{
			Phaser flb = PoolGetFreeInactive ( poolListID ) as Phaser;

			flb.shieldHitID  = curShieldHit;
			curShieldHit = ( curShieldHit + 1 ) % 4;

			SatilliteBrain.ShieldHitOn ( flb.shieldHitID, shootdirection );

			flb.ego.transform.parent = goex_shootparent.transform;

			lro = flb.lineList [ flb.laser1ID ];

			lro.gotransform.position = emitposition;

			for ( loop = 0; loop < lro.sectionCount; loop++ )
			{
				lro.lr.SetPosition ( loop, Vector3.Lerp ( lro.gotransform.position, rhit.point, ( loop + 1.0f ) / lro.sectionCount ) );
			}

#if !UNITY_3_5
		lro.go.SetActive ( true );	
#else
		lro.go.active = true;
#endif

			flb.dur 	= 0.1f;
			flb.time	= 0;
			flb.stage	= 0;

			SoundFX.PlayAudioClip ( SoundFX.soundPhaser );
		}
	}

//===========================================================================
	public override void ProcessInUpdate()
	{
		per = time / dur;
		time += Time.deltaTime;

		switch ( stage )
		{
			case 0:
				LaserScale ( laser1ID, per );
				if ( per >= 1.0f )
				{
					time = 0;
					dur = 1.0f;
					stage++;
				}
				break;	

			case 1:
				LaserScale ( laser1ID, Random.Range ( 0.8f, 1.2f ) );
				if ( per >= 1.0f )
				{
					time = 0;
					dur = 0.1f;
					stage++;
				}
				break;	

			case 2:
				LaserScale ( laser1ID, 1.0f-per );
				if ( per >= 1.0f )
				{
					ego.EchoActive ( false );
					LaserOff ( laser1ID );
					SatilliteBrain.ShieldHitOff ( shieldHitID );
					PoolMoveActive2Inactive(this);
				}
				break;	
		}

	}

//===========================================================================
// use sectioncount == 2 for most lines, if they look squiggly use more
//===========================================================================
	public int LaserAdd ( float iwidth1, float iwidth2, Color icolor1, Color icolor2, int isectioncount = 2 )
  	{
		LineRenObj lro;

		lro								= new LineRenObj();
		lro.go							= new GameObject();
//		lro.go.transform.position		= emitposition;
		lro.go.layer					= ego.gameObject.layer;

#if !UNITY_3_5
		lro.go.SetActive ( false );
#else
		lro.go.active               	= false;
#endif

      	lro.lr							= lro.go.AddComponent<LineRenderer>();
		lro.lr.useWorldSpace            = true;
		lro.lr.sharedMaterial			= laserMat;
		lro.width1                      = iwidth1;
		lro.width2                      = iwidth2;
		lro.gotransform                 = lro.go.transform;
		lro.sectionCount                = isectioncount;

		lro.lr.SetVertexCount ( isectioncount );
		lro.lr.SetWidth ( iwidth1, iwidth2 );
		lro.lr.SetColors ( icolor1, icolor2 );

		lineListSlow.Add ( lro );
		lineListCount++;

		return ( lineListCount-1 ); // returns index/id to this Line
  	}

//===========================================================================
	public void LaserFinalize()
	{
		int loop;

		lineList = new LineRenObj[lineListCount];

		for ( loop = 0; loop < lineListCount; loop++ )
		{
			lineList[loop] = lineListSlow[loop];
		}

		lineListSlow.Clear();
	}

//===========================================================================
	public void LaserOn ( int id, Vector3 emitposition, Vector3 destposition )
	{
		LineRenObj		 lro = lineList[id];
		int				 loop;

		lro.gotransform.position = emitposition;

		for ( loop = 0; loop < lro.sectionCount; loop++ )
		{
			lro.lr.SetPosition ( loop, Vector3.Lerp ( lro.gotransform.position, destposition , ( loop + 1.0f ) / lro.sectionCount ) );
		}

#if !UNITY_3_5
		lro.go.SetActive ( true );
#else
		lro.go.active = true;
#endif

	}

//===========================================================================
	public void LaserOff ( int id )
	{
#if !UNITY_3_5
		lineList[id].go.SetActive ( false );
#else
		lineList[id].go.active = false;
#endif
	}

//===========================================================================
	public void LaserScale ( int id, float scale )
	{
		LineRenObj lro = lineList[id];

		lro.lr.SetWidth ( lro.width1 * scale, lro.width2 * scale );
	}
}