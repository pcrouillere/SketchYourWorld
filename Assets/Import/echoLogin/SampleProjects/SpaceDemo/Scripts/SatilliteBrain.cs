using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class SatilliteBrain : EchoGameObject
{
	public static int 				hitPoints = 8;
	public static EchoGameObject	egoFurnace;
	public static EchoGameObject 	egoScreens;
	public static EchoGameObject 	egoScroll;
	public static EchoGameObject 	egoShield;
	public static EchoFXEvent[] 	shieldFX1;
	public static EchoFXEvent[] 	shieldFX2;
	static public EchoGameObject    myego;       

//---------------------------------------------------------------------------
	public static void ShieldsUp()
	{
		EchoFXEvent.ShieldOn_echoShader ( egoShield, 0.6f, 0.8f );
		EchoFXEvent.Random_echoUV ( egoShield, 1.1f );
	}

//---------------------------------------------------------------------------
	public static void ShieldsDown()
	{
		EchoFXEvent.ShieldDown_echoShader ( egoShield, 0.6f, 2.0f );
		EchoFXEvent.Random_echoUV ( egoShield, 2.0f );
	}

//---------------------------------------------------------------------------
	public static void ShieldHitOn (  int ishieldhitid, Vector3 ishootdir )
	{
		shieldFX1[ishieldhitid] = EchoFXEvent.ShieldHit_echoShader ( egoShield, ishieldhitid, ishootdir, 0.11f, 0.62f, 0.0f );
		shieldFX2[ishieldhitid] = EchoFXEvent.Random_echoUV ( egoShield );
	}

//---------------------------------------------------------------------------
	public static void ShieldHitOff( int ishieldhitid )
	{
		EchoFXEvent.StopEvent ( shieldFX1[ishieldhitid] );
		EchoFXEvent.StopEvent ( shieldFX2[ishieldhitid] );
	}

//---------------------------------------------------------------------------
	public static void DoDamage( int ihitpoints )
	{
		hitPoints -= ihitpoints;
	}
	
//===========================================================================
	void Start()
	{
		myego = this;
		
		shieldFX1 = new EchoFXEvent[4];
		shieldFX2 = new EchoFXEvent[4];

		egoScreens		= EchoGameObject.Find ("screens");
		egoScroll		= EchoGameObject.Find ("scroll");
		egoFurnace		= EchoGameObject.Find ("furnace");

		egoShield		= EchoGameObject.Find ("shield");

		egoScreens.UVSetMake ( ELC.CELL8, ELC.CELL8, 4, 12 );

		EchoFXEvent.CellAnimate_UVSet ( egoScreens, 0, 12, 8.0f, 0 );
		EchoFXEvent.ScrollSection_echoUV ( egoScroll, new Vector4 ( 0, 0, 0, 0 ), new Vector4 ( ( ELC.CELL16 ) * 7.0f, 0, 0, 0 ), 0, 5.12f );
	}

//===========================================================================
	void Update()
	{
		cachedTransform.Rotate ( new Vector3 ( Time.deltaTime * 8.2f ,Time.deltaTime * 8.2f ,Time.deltaTime * 8.2f ) );
	}
}

