using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class RotateCamera : EchoGameObject
{
	public static AudioSource 			musicSource;
	public static AudioClip 			song1;
	
	void Start()
	{
		musicSource								= gameObject.AddComponent ( "AudioSource" ) as AudioSource;
		musicSource.clip						= null;
		musicSource.volume						= 1;
		musicSource.maxDistance					= 1024;
		musicSource.minDistance					= 0;
		musicSource.ignoreListenerVolume		= true;

		song1									= Resources.Load ( "echoLogin_action1", typeof ( AudioClip ) ) as AudioClip;
		musicSource.clip						= song1;
		musicSource.loop						= true;
		musicSource.Play();
		
	}
	
	//===========================================================================
	void Update()
	{
		cachedTransform.Rotate ( new Vector3 ( 0 , 0,Time.deltaTime * -8.0f ) );
	}

}
