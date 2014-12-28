using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class KryBeam : EchoGameObject
{
	public LineRenderer 	lr;
	public float 			width1;
	public float 			width2;
	public float            hitscale;
};

//###########################################################################
// Builds the most powerfull weapon the the known universe
//###########################################################################
[RequireComponent (typeof (EchoGameObject))]
class KrylocianLaser : MonoBehaviour
{
	public Material		kMat = null;
	public Color      	innerLineColor1 = new Color ( 1.0f, 1.0f, 1.0f, 1.0f );
	public Color      	innerLineColor2 = new Color ( 1.0f, 0.0f, 0.0f, 1.0f );
	public Color      	outerLineColor1 = new Color ( 1.0f, 1.0f, 1.0f, 1.0f );
	public Color      	outerLineColor2 = new Color ( 1.0f, 0.0f, 0.0f, 1.0f );
	public static EchoGameObject  	ego;
	private static KryBeam[] kryBeam;
	private static float _per1;
	private static float _time1;
	private static float _dur1;
	private static float _per2;
	private static float _time2;
	private static float _dur2;
	private static int stage1;
	private static int stage2;
	private static EchoGameObject _fusionObj;
	private static EchoGameObject _furnace;
	private static EchoGameObject _screens;
	private static EchoGameObject _scroll;

//===========================================================================
	void Start()
	{
		KryBeam kb;
		int loop;

		ego		= GetComponent<EchoGameObject>();

		_fusionObj = EchoGameObject.Find("blast");
		_furnace = EchoGameObject.Find("furnace");
		_screens = EchoGameObject.Find("screens");
		_scroll = EchoGameObject.Find("scroll");

		kryBeam = new KryBeam[2];	
	
		for ( loop = 0; loop < 2; loop++ )
		{
			kb = new GameObject("WTF").AddComponent<KryBeam>();
			kb.Init ( kb.gameObject, false, false, false, false );		

		  	kb.lr = kb.gameObject.AddComponent<LineRenderer>();
			kb.lr.useWorldSpace         = true;
			kb.lr.sharedMaterial		= kMat;
			kb.gameObject.layer			= ego.gameObject.layer;

			if ( loop == 0 )
			{
				kb.width1 	= 0.08f;	
				kb.width2 	= 0.1f;	
				kb.hitscale = 0.01f;
				kb.lr.SetVertexCount ( 16 );
				kb.lr.SetWidth ( kb.width1, kb.width1 * kb.hitscale );
				kb.lr.SetColors ( innerLineColor1, innerLineColor2 );
			}
			else
			{
				kb.width1 	= 1.0f;	
				kb.width2 	= 1.2f;	
				kb.hitscale = 0.0f;
				kb.lr.SetVertexCount ( 16 );
				kb.lr.SetWidth ( kb.width1, kb.width1 * kb.hitscale );
				kb.lr.SetColors ( outerLineColor1, outerLineColor2 );
			}
			
			kb.EchoActive ( false );
			
			kryBeam[loop] = kb;
		}
	}

//--------------------------------------------------------------------------
	public static void BurnThemBurnThemAll ( Vector3 iunknowinghelplesstarget )
	{
		int loop;
		RaycastHit		 rhit;
		EchoFXEvent      efx;
		
		if ( Physics.Raycast ( ego.cachedTransform.position, iunknowinghelplesstarget - ego.cachedTransform.position, out rhit, 256 ) )
		{
			stage1					= 0;
			stage2					= 0;

			_dur1 = 0.1f;
			_time1 = 0.0f;

			_dur2 = 0.3f;
			_time2 = 0.0f;

			kryBeam[0].EchoActive ( true );
			for ( loop = 0; loop < 16; loop++ )
			{
				kryBeam[0].transform.position = ego.cachedTransform.position;	
				kryBeam[0].lr.SetPosition ( loop, Vector3.Lerp ( kryBeam[0].transform.position, rhit.point, ( loop + 1.0f ) / 16.0f ) );
				kryBeam[1].transform.position = ego.cachedTransform.position;	
				kryBeam[1].lr.SetPosition ( loop, Vector3.Lerp ( kryBeam[1].transform.position, rhit.point, ( loop + 1.0f ) / 16.0f ) );
			}
			
	    	efx = EchoFXEvent.ShakeGameObject ( Camera.main.transform, 0.05f, 6.0f );
			efx.AddFilter ( EchoFilter.SINE, 0.5f );

			
			_fusionObj.EchoActive ( true );
			EchoFXEvent.Fusion_echoShader	( _fusionObj, 0, 1.3f, 0.1f, 1.0f );
			efx = EchoFXEvent.Fusion_echoShader ( _fusionObj, 1.4f, 1.4f, 0.2f, 5.0f );
			efx.StartDelay ( 1.0f );

			efx = EchoFXEvent.Animate_echoScale ( _fusionObj, new Vector4 ( 1,1,1,1 ), new Vector4 ( 3,1.7f,1,1 ), 5.0f );
			efx.StartDelay ( 1.1f );

			efx = EchoFXEvent.Animate_echoRGBA ( _fusionObj, new Vector4 ( 1,1,1,1 ), new Vector4 ( 0,0,0,0 ), 5.0f );
			efx.StartDelay ( 5.0f );
			efx.SetEventStart ( TurnOffSatillite );
		}
	}

//===========================================================================
	public static void TurnOffSatillite()
	{
		SatilliteBrain.myego.RendererSet ( false, false );
		_furnace.EchoActive ( false );
		_screens.EchoActive ( false );
		_scroll.EchoActive ( false );
	}

//===========================================================================
	public void ProcessInnerLine()
	{
		_per1 = _time1 / _dur1;
		_time1 += Time.deltaTime;

		switch ( stage1 )
		{
			case 0:
				kryBeam[0].lr.SetWidth ( kryBeam[0].width1 * _per1, kryBeam[0].width1 * _per1 * kryBeam[0].hitscale );
				if ( _per1 >= 1.0f )
				{
					_time1 	= 0.0f;
					_dur1 	= 5.0f;
					stage1++;
					kryBeam[1].EchoActive ( true );
				}
				break;

			case 1:
				kryBeam[0].lr.SetWidth ( Random.Range ( kryBeam[0].width1, kryBeam[0].width2 ), Random.Range ( kryBeam[0].width1, kryBeam[0].width2 ) * kryBeam[0].hitscale );

				if ( _per1 >= 1.0f )
				{
					_time1 	= 0.0f;
					_dur1 	= 0.1f;
					stage1++;
				}
				break;

			case 2:
				_per1 = 1.0f - _per1;
				kryBeam[0].lr.SetWidth ( kryBeam[0].width1 * _per1, kryBeam[0].width1 * _per1 * kryBeam[0].hitscale );
				if ( _per1 >= 1.0f )
				{
					kryBeam[0].EchoActive ( false );
					stage1++;
				}
				break;

			case 3:
				break;
		}
	}

//===========================================================================
	public void ProcessOuterLine()
	{
		_per2 = _time2 / _dur2;
		_time2 += Time.deltaTime;

		switch ( stage2 )
		{
			case 0:
				kryBeam[1].lr.SetWidth ( kryBeam[1].width1 * _per2, kryBeam[1].width1 * _per2 * kryBeam[1].hitscale );
				if ( _per2 >= 1.0f )
				{
					_time2 	= 0.0f;
					_dur2 	= 5.0f;
					stage2++;
				}

				break;

			case 1:
				kryBeam[1].lr.SetWidth ( Random.Range ( kryBeam[1].width1, kryBeam[1].width2 ), Random.Range ( kryBeam[1].width1, kryBeam[1].width2 ) * kryBeam[1].hitscale );

				if ( _per2 >= 1.0f )
				{
					_time2 	= 0.0f;
					_dur2 	= 0.1f;
					stage2++;
				}
				break;

			case 2:
				_per2 = 1.0f - _per2;
				kryBeam[1].lr.SetWidth ( kryBeam[1].width1 * _per2, kryBeam[1].width1 * _per2 * kryBeam[1].hitscale );
				if ( _per2 >= 1.0f )
				{
					kryBeam[1].EchoActive ( false );
					stage2++;
				}
				break;

			case 3:
				break;
		}
	}

//===========================================================================
	void Update()
	{
#if !UNITY_3_5
		if ( kryBeam[0].gameObject.activeSelf )
#else
		if ( kryBeam[0].gameObject.active )
#endif
			ProcessInnerLine();

#if !UNITY_3_5
		if ( kryBeam[1].gameObject.activeSelf )
#else
		if ( kryBeam[1].gameObject.active )
#endif
			ProcessOuterLine();
	}
}