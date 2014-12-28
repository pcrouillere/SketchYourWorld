using UnityEngine;
using System;
using System.Collections;

public enum EchoLightType
{
	OFF,
	MAIN_DIRECTIONAL,
	MAIN_POINT,
	FOUR_POINT
}

[ExecuteInEditMode()] 
public class EchoLight : MonoBehaviour
{
	[System.NonSerializedAttribute]
	public EchoLight    	next;
	[System.NonSerializedAttribute]
	public EchoLight    	prev;
	[System.NonSerializedAttribute]
	public Light  			uLight 			= null;   			// reference to unity light if used
	public EchoLightType	type 			= EchoLightType.OFF;     			// 0 = off, 1 = main dir, 2 = main point, 3 = 4 extra point lights
	[System.NonSerializedAttribute]
	public float        	dist            = 0;
	[System.NonSerializedAttribute]
	public Transform 		cachedTransform = null;
	[System.NonSerializedAttribute]
	public bool             lightOn         = false;
	[System.NonSerializedAttribute]
	public bool             inList 			= false;
	[HideInInspector]
	public int              findex          = -1;

	void OnEnable()
	{
		Init();
	}
		
	void OnDisable()
	{
		lightOn = false;
		
		EchoCoreManager.TurnOffLight ( type );
		
		if ( type == EchoLightType.FOUR_POINT )
			EchoCoreManager.RemoveList ( this );
	}

	void OnDestroy()
	{
		lightOn = false;

		if ( type == EchoLightType.FOUR_POINT )
			EchoCoreManager.RemoveList ( this );
	}

	public void Init( )
	{
		if ( type == EchoLightType.FOUR_POINT )
			EchoCoreManager.RemoveList ( this );
		
		if ( uLight == null )
			uLight = GetComponent<Light>();
		
		if ( uLight == null )
			return;

		cachedTransform = uLight.transform;
		dist 			= 0;
		
#if !UNITY_3_5
		lightOn = gameObject.activeSelf;
#else
		lightOn = gameObject.active;
#endif
		
		if ( uLight.renderMode == LightRenderMode.ForceVertex )
		{
			type = EchoLightType.FOUR_POINT;
			
			if ( lightOn )
				EchoCoreManager.AddList ( this );
		}
		else
		{
			switch ( uLight.type )
			{
			case LightType.Point:
				type = EchoLightType.MAIN_POINT;
				break;
		
			default:
				type = EchoLightType.MAIN_DIRECTIONAL;
				break;
			}
		}

#if UNITY_EDITOR
		if ( Application.isPlaying && EchoCoreManager.useUnityLights == false )
			uLight.enabled = false;
#else
		if ( EchoCoreManager.useUnityLights == false )
			uLight.enabled = false;
#endif
		
	}
}

