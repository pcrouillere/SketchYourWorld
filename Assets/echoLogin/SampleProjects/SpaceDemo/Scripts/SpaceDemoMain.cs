using UnityEngine;
using System.Collections;
                                              
public class SpaceDemoMain : MonoBehaviour 
{
	public GameObject  		particleSmoke;
	public Material 		laserMat;
	public static GameObject 			soundObj;
	public static AudioSource 			musicSource;
	public static AudioClip 			song1;
	public static int[] 				charLookup;
	public static EchoGameObject[] 		laserEmit;
	public static EchoGameObject[] 		missileLaunch;
	public static EchoGameObject 		dataPanel1;
	public static EchoGameObject 		dataPanel1_Title;
	public static EchoGameObject 		dataPanel2;
	public static EchoGameObject 		dataPanel3;
	public static EchoGameObject[] 		dataPanel3_chars;
	public static EchoGameObject 		dataPanel4;
	public static EchoGameObject      	fadePlate;
	public static EchoGameObject        guiCam;
	public static EchoGameObject        endText;
	private float						_ceAcc;
	private float						_ceCellDelay;
	private float						_ceCellSize;
	private Vector4						_ceUVAdd;
	private Vector4						_ceUVBase;
	private float						_ceTime;
	private int							_oldFPS;
	private EchoFXEvent 				_efx;
	private int                         _ticks = 0;
	
	//============================================================
	void Start () 
	{
		int loop;

		Application.targetFrameRate		= 60;

		guiCam = EchoGameObject.Find ("CameraGui");
		endText = EchoGameObject.Find ("endtext");
		soundObj = GameObject.Find ("SpaceDemoMain");
		
		// lookup array for characters on texture ( example use in DisplayFPS method )
		charLookup = new int [ 256 ];

		for ( loop = 0; loop < 256; loop++ )
			charLookup[loop] = -1;

		// 0-9;
		for ( loop = 0; loop < 12; loop++ )
			charLookup[loop+48] = loop;

		// a-l
		for ( loop = 0; loop < 12; loop++ )
			charLookup[loop+65] = loop+12;

		// m-x
		for ( loop = 0; loop < 12; loop++ )
			charLookup[loop+77] = loop+24;

		// y-z
		for ( loop = 0; loop < 2; loop++ )
			charLookup[loop+89] = loop+36;

		// !"#$
		for ( loop = 0; loop < 10; loop++ )
			charLookup[loop+33] = loop+38;

		// =====
		// setup sound and start playing song
		// =====

		SoundFX.Init(soundObj);

		musicSource								= soundObj.AddComponent ( "AudioSource" ) as AudioSource;
		musicSource.clip						= null;
		musicSource.volume						= 1;
		musicSource.maxDistance					= 1024;
		musicSource.minDistance					= 32;
		musicSource.ignoreListenerVolume		= true;

		song1									= Resources.Load ( "Disruptor", typeof ( AudioClip ) ) as AudioClip;
		musicSource.clip						= song1;
		musicSource.loop						= false;

		// =====
		// Setup laser and missile examples
		// =====
		Phaser.AllocPool ( 4, laserMat );

		fadePlate				= EchoGameObject.Find ("fadeplate");

		// tech screen
		dataPanel1_Title		= EchoGameObject.Find ("DataPanel1_Title");
		dataPanel1				= EchoGameObject.Find ("DataPanel1_crt");
		dataPanel2				= EchoGameObject.Find ("DataPanel2");
		dataPanel3				= EchoGameObject.Find ("DataPanel3");

		dataPanel3_chars		= new EchoGameObject[6];
		for ( loop = 0; loop < 6; loop++ )
		{
			dataPanel3_chars[loop] = EchoGameObject.Find("DataPanel3_char"+loop);
			dataPanel3_chars[loop].UVSetMake ( ELC.CELL16, ELC.CELL16, 8, 48 );
		}

		dataPanel4		= EchoGameObject.Find ("DataPanel4");

		// =====
		// empty objects that are starting points for laser shots
		// =====
		laserEmit = new EchoGameObject[4];

		for ( loop = 0; loop < 4; loop++ )
		{
			laserEmit[loop] = EchoGameObject.Find ( "LaserEmit"+loop );
		}

		// =====
		// setup missile emit points
		// =====
		missileLaunch = new EchoGameObject[8];

		for ( loop = 0; loop < 8; loop++ )
		{
			missileLaunch[loop] = EchoGameObject.Find ( "MissileLaunch" + loop );
		}

		fadePlate.EchoActive ( true );
		EchoFXEvent.Animate_echoRGBA ( SpaceDemoMain.fadePlate, new Vector4 ( 2,2,2,2 ), new Vector4 ( 2,2,2,2 ), 0.1f );
	}
	
	//============================================================
	public void StartDemo()
	{
		// =====
		// example of how to setup a custom user event
		// =====
		_ceAcc			= 0.0f;
	 	_ceCellDelay	= 0.2f;
	 	_ceCellSize		= 1.0f / 16.0f;
		_ceUVAdd		= new Vector4 ( 0.0f, -0.4f, 0.0f, 0.0f );
		_ceUVBase		= new Vector4 ( 0,0,0,0 );
		_ceTime			= 0.0f;

		EchoFXEvent.StartEventCustom ( dataPanel1, 0.0f, 0.0f, CustomEventCrtScroll );

		// =====
		// start various shader effects going
		// =====
		dataPanel1_Title.ShaderSet_echoRGBA ( Vector4.zero );
		dataPanel1_Title.ShaderSet_echoScale ( Vector4.zero );
		dataPanel1_Title.ShaderPropertiesSubmit();

		_efx = EchoFXEvent.Animate_echoRGBA ( dataPanel1_Title, new Vector4 ( 0.0f, 0.0f, 0.0f, 0.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), 0.15f );
		_efx.StartDelay ( 2.0f );

		_efx = EchoFXEvent.Animate_echoScale ( dataPanel1_Title, new Vector4 ( 0.0f, 1.0f, 1.0f, 1.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), 0.15f );
		_efx.StartDelay ( 2.2f );

		dataPanel1.ShaderSet_echoRGBA (  Vector4.zero );
		dataPanel1.ShaderSet_echoScale ( Vector4.zero );
		dataPanel1.ShaderPropertiesSubmit();

		_efx = EchoFXEvent.Animate_echoRGBA ( dataPanel1, new Vector4 ( 0.0f, 0.0f, 0.0f, 0.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), 0.15f );
		_efx.StartDelay ( 2.5f );

		_efx = EchoFXEvent.Animate_echoScale ( dataPanel1, new Vector4 ( 1.0f, 0.0f, 1.0f, 1.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), 0.17f );
		_efx.StartDelay ( 2.5f, PlayDataPanel1Sound );

		_efx = EchoFXEvent.Animate_echoRGBA ( SpaceDemoMain.fadePlate, new Vector4 ( 2,2,2,2 ), new Vector4 ( 0,0,0,0 ), 3.0f );
		_efx.AddFilter ( EchoFilter.SINE );
		_efx.SetEventDone ( TurnOffFadePlate );

		musicSource.Play();
		Camera.main.animation.Play("echoLoginCamera");	
	}

	//============================================================
	public void PlayDataPanel1Sound()
	{
		SoundFX.PlayAudioClip ( SoundFX.soundBeep1 );
	}

	//============================================================
	public void TurnOffFadePlate( bool iforcestop )
	{
		fadePlate.EchoActive ( false );
	}
	
	//============================================================
	// Example for a Custom Event Callback ( data screen in upper right of demo )
	//============================================================
	bool CustomEventCrtScroll ( EchoGameObject igoex, float ipercentdone, float iratio, float itime )
	{
		if ( _ceAcc >= _ceCellSize )
		{
			_ceTime -= Time.deltaTime;

			if ( _ceTime <= 0.0f )
			{
				_ceTime = _ceCellDelay;
				_ceAcc 	= _ceAcc - _ceCellSize;
			}

			return ( false );
		}

		_ceUVBase 	+= _ceUVAdd * Time.deltaTime;
		_ceAcc 		+= Mathf.Abs ( _ceUVAdd.y * Time.deltaTime );

		igoex.ShaderSet_echoUV ( _ceUVBase, 0 );

		if ( ipercentdone >= 1.0f )
		{
			return ( true );
		}

		return ( false );
	}

	//============================================================
	public static void TurnOnShieldHUDWarning()
	{
		EchoFXEvent dfx;
		
		SatilliteBrain.myego.Outline ( true );

		dataPanel4.EchoActive ( true );
		EchoFXEvent.Animate_echoRGBA ( dataPanel4, new Vector4 ( 0.0f, 0.0f, 0.0f, 0.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), 0.14f );
		EchoFXEvent.Animate_echoScale ( dataPanel4, new Vector4 ( 0.0f, 1.0f, 1.0f, 1.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), 0.15f );

		dfx = EchoFXEvent.Animate_echoRGBA ( dataPanel4, new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), new Vector4 ( 0.0f, 0.0f, 0.0f, 0.0f ), 0.14f );
		dfx.StartDelay ( 2.0f);
		dfx = EchoFXEvent.Animate_echoScale ( dataPanel4, new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), new Vector4 ( 0.0f, 1.0f, 1.0f, 1.0f ), 0.15f );
		dfx.StartDelay ( 2.0f);
		
		dfx.SetEventDone ( TurnOffDataPanel4 );

		SoundFX.PlayAudioClip ( SoundFX.soundBeep1 );
	}

	//============================================================
	public static void TurnOffDataPanel4 ( bool iforcestop )
	{
		SatilliteBrain.myego.Outline ( false );
		dataPanel4.EchoActive ( false );
	}
	
	//============================================================
	public static void TurnOnDataPanel2()
	{
		EchoFXEvent dfx;

		dataPanel2.EchoActive ( true );
		EchoFXEvent.CellAnimate_echoUV ( dataPanel2, ELC.CELL8, ELC.CELL8, 4, 0, 4, 0.025f, 10 );
		EchoFXEvent.Animate_echoRGBA ( dataPanel2, new Vector4 ( 0.0f, 0.0f, 0.0f, 0.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), 0.15f );
		EchoFXEvent.Animate_echoScale ( dataPanel2, new Vector4 ( 1.0f, 0.0f, 1.0f, 1.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), 0.18f );
		dfx = EchoFXEvent.CellAnimate_echoUV ( dataPanel2, ELC.CELL8, ELC.CELL8, 4, 4, 16, 0.256f, 0 );
		dfx.StartDelay ( 0.5f );
	}

	//============================================================
	public static void TurnOffDataPanel2()
	{
		EchoFXEvent dfx;
		
		dataPanel2.EchoActive ( true );

		EchoFXEvent.CellAnimate_echoUV ( dataPanel2, ELC.CELL8, ELC.CELL8, 4, 0, 4, 0.025f, 10 );
		dfx = EchoFXEvent.Animate_echoRGBA ( dataPanel2, new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), new Vector4 ( 0.0f, 0.0f, 0.0f, 0.0f ), 0.18f );
		dfx.StartDelay ( 0.25f );

		dfx = EchoFXEvent.Animate_echoScale ( dataPanel2, new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), new Vector4 ( 1.0f, 0.0f, 1.0f, 1.0f ), 0.15f );
		dfx.StartDelay ( 0.2f );
	}
	
	//===========================================================================
	// this shows how to display a score/fps w/o doing string stuff which
	// will cause garbage colleciton and cause your game to hiccup
	//===========================================================================
	void DisplayFPS()
	{
		int loop;
		int num = (int)Mathf.Ceil ( FPS.fps );
		
		if ( num  > 60 )
			num = 60;
		
		if ( _oldFPS != num )
		{
			_oldFPS = num;
			
			dataPanel3_chars[0].UVSet ( charLookup['F'] );
			dataPanel3_chars[1].UVSet ( charLookup['P'] );
			dataPanel3_chars[2].UVSet ( charLookup['S'] );
			dataPanel3_chars[3].UVSet ( charLookup[':'] );

			for ( loop = 4; loop < 6; loop++ )
				dataPanel3_chars[loop].ShaderSetCell_echoUV ( 0, ELC.CELL16, ELC.CELL16, 8 );

			while ( num > 0 )
			{
				loop = 5;

				while ( num > 0 )
				{
					dataPanel3_chars[loop].UVSet  ( num%10 );
					loop--;
					num /= 10;
				}
			}
		}
	}

	//============================================================
	void Update () 
	{
		_ticks++;

		if ( _ticks <= 30 )
		{
			// if i dont do this, sometimes the animation is not synced with music right
			if ( _ticks == 30 )
				StartDemo();
			else
				return;			
		}

		FPS.ProcessInUpdate();

		DisplayFPS();

		// process laser and missle systems
		Phaser.ProcessAllInUpdate();
	}
}