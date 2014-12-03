using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class EndTextBrain : EchoGameObject 
{
	static EchoGameObject egoNiceDay;

	//--------------------------------------------------------------------------
	public void StartEndText()
	{
		EchoFXEvent efx;

#if !UNITY_3_5
		Camera.main.gameObject.SetActive ( false );
#else
		Camera.main.gameObject.active = false;
#endif

		SpaceDemoMain.guiCam.camera.clearFlags		= CameraClearFlags.SolidColor;
		SpaceDemoMain.guiCam.camera.backgroundColor	= new Color ( 0,0,0,0 );
		SpaceDemoMain.dataPanel1.EchoActive ( false );
		SpaceDemoMain.dataPanel1_Title.EchoActive ( false );
		SpaceDemoMain.dataPanel3.EchoActive ( false );

		egoNiceDay = EchoGameObject.Find("niceday");	
		egoNiceDay.EchoActive ( true );

		EchoActive ( true );

		efx = EchoFXEvent.Dissolve_echoShader ( this, -0.3f, 1.3f, 0.8f );
		efx.AddFilter ( EchoFilter.SINE );
		efx.StartDelay ( 6.0f, PlayBurnSound );
		efx.SetEventDone ( PlayAlienScream );

		efx = EchoFXEvent.Animate_echoRGBA ( egoNiceDay, new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), new Vector4 ( 0.0f, 0.0f, 0.0f, 0.0f ), 1.2f );
		efx.StartDelay ( 11.5f );
		efx.SetEventDone ( EndDemo );
	}

	//--------------------------------------------------------------------------
	public void PlayBurnSound()
	{
		SoundFX.PlayAudioClip ( SoundFX.soundEndText );
	}

	//--------------------------------------------------------------------------
	public void PlayAlienScream( bool iforcestop )
	{
		SoundFX.PlayAudioClip ( SoundFX.soundAlien );
	}

	//--------------------------------------------------------------------------
	public void EndDemo( bool iforcestop )
	{
		Application.LoadLevel ( "Intermission");
	}
}
