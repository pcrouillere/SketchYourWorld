using UnityEngine;
using System.Collections;

public class PulseRGBA : EchoGameObject
{
	EchoFXEvent efx = null;
	int glowCount = 4;

	// Use this for initialization
	void Start () 
	{
		efx = EchoFXEvent.Animate_echoRGBA ( this, new Vector4 ( 0.5f,0.5f,0.5f,0.5f ), new Vector4 ( 4,4,4,4 ), 0.75f );
		efx.AddFilter ( EchoFilter.SINE,0.5f, 1.0f, 0.0f );
		efx.SetEventDone ( RepeatGlow );
	}
	
	void RepeatGlow( bool forcestop )
	{
		if ( !forcestop )
		{
			// keep event going 
			efx = EchoFXEvent.Animate_echoRGBA ( this, new Vector4 ( 0.5f,0.5f,0.5f,0.5f ), new Vector4 ( 4,4,4,4 ), 0.75f );
			efx.AddFilter ( EchoFilter.SINE,0.5f, 1.0f, 0.0f );
			if ( glowCount > 1 )
				efx.SetEventDone ( RepeatGlow );
			glowCount--;
		}
		else
		{
			// fade back to normal gracfully
			efx = EchoFXEvent.Animate_echoRGBA ( this, ShaderGet_echoRGBA(), new Vector4 ( 1,1,1,1 ), 0.2f );
		}
	}
	
	void OnMouseDown()
	{
		EchoFXEvent.StopEvent ( efx );
	}
	
}
