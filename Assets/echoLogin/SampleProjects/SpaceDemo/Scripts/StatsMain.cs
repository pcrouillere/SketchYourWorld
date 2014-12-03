using UnityEngine;
using System.Collections;

public class StatsMain : MonoBehaviour 
{
	EchoGameObject stats;
	EchoGameObject ring;
	EchoGameObject fadePlate;
	EchoGameObject stars;
	public static GameObject 			soundObj;
	public static AudioSource 			musicSource;
	public static AudioClip 			song1;

//==========================================================================
	void Start()
	{
		EchoFXEvent efx;

		Application.targetFrameRate		= 60;

		soundObj = GameObject.Find ("StatsMain");

		musicSource								= soundObj.AddComponent ( "AudioSource" ) as AudioSource;
		musicSource.clip						= null;
		musicSource.volume						= 1;
		musicSource.maxDistance					= 1024;
		musicSource.minDistance					= 32;
		musicSource.ignoreListenerVolume		= true;

		song1									= Resources.Load ( "oblivion", typeof ( AudioClip ) ) as AudioClip;
		musicSource.clip						= song1;
		musicSource.loop						= false;
		musicSource.Play();

		SoundFX.Init(soundObj);

		stars			= EchoGameObject.Find ("spaceskybox");
		fadePlate		= EchoGameObject.Find ("fadeplate");
		ring			= EchoGameObject.Find ("ring");
		stats			= EchoGameObject.Find ("istats");
		stats.EchoActive ( false );

		efx = EchoFXEvent.Animate_echoRGBA ( stats, new Vector4 ( 0,0,0,0 ), new Vector4 ( 1,1,1,1 ), 0.3f );
		efx.StartDelay ( 4.0f, TurnOnStats );

		efx = EchoFXEvent.Animate_echoScale ( stats, new Vector4 ( 1.0f, 0.0f, 1.0f, 1.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), 0.4f );
		efx.StartDelay ( 4.0f );
		efx.SetEventDone ( FemaleVoice );

		efx = EchoFXEvent.Animate_echoRGBA ( stats, new Vector4 ( 1,1,1,1 ), new Vector4 ( 0,0,0,0 ), 0.24f );
		efx.StartDelay ( 14.0f );

		efx = EchoFXEvent.Animate_echoScale ( stats, new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), new Vector4 ( 0.0f, 1.0f, 1.0f, 1.0f ), 0.25f );
		efx.StartDelay ( 14.0f, EndStatsSound );

		fadePlate.EchoActive	( true );
		efx						= EchoFXEvent.Animate_echoRGBA ( fadePlate, new Vector4 ( 0,0,0,2 ), new Vector4 ( 0,0,0,0 ), 2.0f );
		efx.SetEventDone ( TurnOffFadePlate );

		efx						= EchoFXEvent.Animate_echoRGBA ( fadePlate, new Vector4 ( 0,0,0,0 ), new Vector4 ( 2,2,2,2 ), 2.0f );
		efx.StartDelay ( 15.0f, TurnOnFadePlate );
		efx.SetEventDone ( EndScene );

	}

//==========================================================================
	void TurnOnStats()
	{
		stats.EchoActive ( true );
		SoundFX.PlayAudioClip ( SoundFX.soundBeep1 );
	}

//==========================================================================
	void FemaleVoice( bool iforcestop )
	{
		SoundFX.PlayAudioClip ( SoundFX.soundStats );
	}

//==========================================================================
	void EndStatsSound()
	{
		SoundFX.PlayAudioClip ( SoundFX.soundBeep2 );
	}

//==========================================================================
	void EndScene( bool iforcestop )
	{
		Application.LoadLevel ( "scene1" );
	}

//==========================================================================
	void TurnOffFadePlate( bool iforcestop )
	{
		fadePlate.EchoActive ( false );
	}

//==========================================================================
	void TurnOnFadePlate()
	{
		fadePlate.EchoActive ( true );
	}

//==========================================================================
	void Update()
	{
		ring.transform.Rotate ( new Vector3 ( 0 ,Time.deltaTime ,0 ) );
		stars.transform.Rotate ( new Vector3 ( 0 ,Time.deltaTime * -0.2f ,Time.deltaTime * 0.1f ) );
	}

}
