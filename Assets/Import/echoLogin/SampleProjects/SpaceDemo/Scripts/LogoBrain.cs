using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogoBrain : EchoGameObject
{
	public static EchoGameObject egoEcho;
	public static EchoGameObject egoLogin;
	public static EchoGameObject egoSubtitle;
	public static EchoGameObject myego;

	//--------------------------------------------------------------------------
	public static void AnimationStart()
	{
		myego.animation.Play("echoLogin");
	}

	//--------------------------------------------------------------------------
	public static void SubtitleStart()
	{
		EchoFXEvent efx;

		egoSubtitle.EchoActive ( true );

		EchoFXEvent.Animate_echoRGBA ( egoSubtitle, new Vector4 ( 1.0f, 1.0f, 1.0f, 0.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), 3.0f );
		efx = EchoFXEvent.Animate_echoScale ( egoSubtitle, new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ) , new Vector4 ( 1.0f, 1.0f, 24.0f, 1.0f ), 3.0f );
		efx.AddFilter ( EchoFilter.SINE, 0.5f );
	}

	//--------------------------------------------------------------------------
	public static void SubtitleEnd()
	{
		EchoFXEvent efx;
		efx = EchoFXEvent.Animate_echoRGBA ( egoSubtitle, new Vector4 ( 1.0f, 1.0f, 1.0f, 1.0f ), new Vector4 ( 1.0f, 1.0f, 1.0f, 0.0f ), 1.4f );
		efx.SetEventDone ( TurnOffSubtitle );
	}

	//--------------------------------------------------------------------------
	public static void TurnOffSubtitle( bool iforcestop )
	{
		egoSubtitle.EchoActive ( false );
	}

	//--------------------------------------------------------------------------
	public static void BlinkEchoLoginOn()
	{
// using pre allocated UV Sets
		EchoFXEvent.Blink_UVSet ( egoEcho, 0, 1, 1.0f );
      	EchoFXEvent.Blink_UVSet ( egoLogin, 0, 1, 1.0f );

// using shader properties
//		EchoFXEvent.Blink_echoUV ( egoEcho, new Vector2 ( 0.0f, 0.0f ), new Vector2 ( 1.0f / 8.0f, 0.0f ), 1.0f );
//    	EchoFXEvent.Blink_echoUV ( egoLogin, new Vector2 ( 0.0f, 0.0f ), new Vector2 ( 1.0f / 8.0f, 0.0f ), 1.0f );
	}

	//===========================================================================
	void Start()
	{
		myego 			= this;
		egoEcho			= EchoGameObject.Find ("EchoFront");
		egoEcho.UVSetMake ( ELC.CELL4, ELC.CELL4, 4, 2 );

		egoLogin		= EchoGameObject.Find ("LoginFront");
		egoLogin.UVSetMake ( ELC.CELL4, ELC.CELL4, 4, 2 );

		egoSubtitle		= EchoGameObject.Find ("subtitle");
		egoSubtitle.EchoActive ( false );
	}
}
