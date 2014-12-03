using UnityEngine;
using System.Collections;
                                              
public class MissileBrain : EchoGameObject
{
	float 				speed;
	[HideInInspector]
	Transform			targetTransform;
	ParticleSystem  	echoParticleSystem;
	ParticleSystem  	missileExplo;
	GameObject          particleObject;

//===========================================================================
// Example of how to override OnDisable  base.OnDisable should be 1st thing called
//===========================================================================
	override public void OnDisable()
	{
		base.OnDisable();
	}

//===========================================================================
	void OnTriggerEnter ( Collider col )
	{
		if ( renderer.enabled )
		{
			missileExplo.Play ( true );
			SoundFX.PlayAudioClip ( SoundFX.soundExplo );
		}

		renderer.enabled = false;
	}	

//===========================================================================
	override public void EchoPoolObjectInit()
	{
		particleObject 		= transform.Find("ParticleSmokeTrail").gameObject;
		echoParticleSystem 	= particleObject.GetComponent<ParticleSystem>();
		missileExplo 		= gameObject.GetComponent<ParticleSystem>();
	}

//===========================================================================
	public void Launch ( Vector3 istartpos, Vector3 idirection, Transform itargettransform, float ispeed )
	{
		EchoActive ( true );
		cachedTransform.position			= istartpos;
		cachedTransform.rotation			= Quaternion.LookRotation ( idirection );	
		speed								= ispeed;
		targetTransform						= itargettransform;
		echoParticleSystem.Play();
	}

//===========================================================================
	void Update()
	{
		if ( renderer.enabled )
		{
			Quaternion qRotation;
		
			cachedTransform.Translate ( Vector3.forward * Time.deltaTime * speed );	

			qRotation = Quaternion.LookRotation ( targetTransform.position - cachedTransform.position );

			cachedTransform.rotation = Quaternion.Slerp ( cachedTransform.rotation, qRotation, Time.deltaTime * 0.6f );
		}
		else
		{
			if ( !missileExplo.isPlaying )
			{
				echoParticleSystem.Stop();
				EchoActive ( false );
			}
		}
	}
}


