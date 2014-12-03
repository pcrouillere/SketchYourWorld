using UnityEngine;
using System.Collections;

class SoundFX
{
	public static AudioSource[] 		fxSource;
	public static AudioClip 			soundExplo;
	public static AudioClip 			soundPhaser;
	public static AudioClip 			soundBeep1;
	public static AudioClip 			soundBeep2;
	public static AudioClip 			soundShieldOn;
	public static AudioClip 			soundShieldDown;
	public static AudioClip 			soundEndText;
	public static AudioClip 			soundAlien;
	public static AudioClip 			soundWarp;
	public static AudioClip 			soundStats;
	private static int                  _curSound = 0;

//--------------------------------------------------------------------------------
	public static void Init( GameObject isobj)
	{
		int loop;

		soundExplo			= Resources.Load ( "distantexplo1", typeof ( AudioClip ) )  as AudioClip;
		soundPhaser			= Resources.Load ( "phaser", typeof ( AudioClip ) )  as AudioClip;
		soundBeep1			= Resources.Load ( "beep1", typeof ( AudioClip ) )  as AudioClip;
		soundBeep2			= Resources.Load ( "beep2", typeof ( AudioClip ) )  as AudioClip;
		soundShieldOn		= Resources.Load ( "shieldon", typeof ( AudioClip ) )  as AudioClip;
		soundShieldDown		= Resources.Load ( "shielddown", typeof ( AudioClip ) )  as AudioClip;
		soundEndText		= Resources.Load ( "endtextburn", typeof ( AudioClip ) )  as AudioClip;
		soundAlien			= Resources.Load ( "alien", typeof ( AudioClip ) )  as AudioClip;
		soundWarp			= Resources.Load ( "warp", typeof ( AudioClip ) )  as AudioClip;
		soundStats			= Resources.Load ( "female_stats", typeof ( AudioClip ) )  as AudioClip;

		fxSource = new AudioSource[6];

		for ( loop = 0; loop < 6; loop++ )
		{
			fxSource[loop]							= isobj.AddComponent ( "AudioSource" ) as AudioSource;
			fxSource[loop].clip		  			    = null;
			fxSource[loop].volume					= 1;
			fxSource[loop].maxDistance				= 1024;
			fxSource[loop].minDistance				= 32;
			fxSource[loop].ignoreListenerVolume	    = true;
		}

	}

//--------------------------------------------------------------------------------
	public static void PlayAudioClip ( AudioClip clip, float volume = 1.0f ) 
	{
		AudioSource s;

		s = fxSource[ _curSound  ];

		_curSound = (  _curSound + 1 ) % 6;

		s.clip = clip;
		s.volume = volume;
		
#if !UNITY_FLASH
		s.priority = 0;
#endif
		s.Play();
	}

}