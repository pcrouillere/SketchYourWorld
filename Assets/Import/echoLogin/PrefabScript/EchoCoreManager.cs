using UnityEngine;
using System;
using System.IO;

[ExecuteInEditMode()] 
public class EchoCoreManager : MonoBehaviour 
{
	public static RenderTexture echoRT 					  = null;
	private static EchoLight    _first                    = null;
	private static EchoLight    _last                     = null;
	private static bool      	_use4ExtraPoint           = false;
	private static bool      	_useLights           	  = false;
	private static EchoLight 	_mainDirLight             = null;
	private static EchoLight 	_mainPointLight           = null;
	private static EchoLight[]  _extraLights              = null;
	private static int          _extraLightCount          = 0;
	private static Vector4    	_dirLightColor;
	private static Vector4    	_echo4LightColorRH;
	private static Vector4    	_echo4LightColorGH;
	private static Vector4    	_echo4LightColorBH;
//	private static Vector4    	_echo4LightIntensityH;  // NOT USED YET
	private static Vector4    	_echo4LightPosXH;
	private static Vector4    	_echo4LightPosYH;
	private static Vector4    	_echo4LightPosZH;
	private static Vector4    	_echo4LightRangeH;
	private static Vector4    	_echo4LightColorR;
	private static Vector4    	_echo4LightColorG;
	private static Vector4    	_echo4LightColorB;
//	private static Vector4    	_echo4LightIntensity; // NOT USED YET
	private static Vector4    	_echo4LightPosX;
	private static Vector4    	_echo4LightPosY;
	private static Vector4    	_echo4LightPosZ;
	private static Vector4    	_echo4LightRange;
	private static Vector4    	_echoDirLightColor;
	private static Vector4    	_echoDirLightPos;
	private static Vector4    	_echoDirLightDir;
	private static Vector4    	_echoPointLightColor;
	private static Vector4    	_echoPointLightPos;
	private static float      	_echoPointLightRange;
	private static Color      	_echoAmbientLightColor;
	private static Vector3    	_lookAhead;
	private static Transform 	_cameraTransform = null;
	private static EchoLight[]	_found = null;
	private	static Light[] 		_lights;
	private static bool 		_initFlag = false;    

	public bool dynamicAdd 						= true;
	public int maxEchoFXEvents 					= 32;
	
	public bool UnityLights                     = true;
	public static bool useUnityLights           = true;
	public bool DoubleLight		                = true;
	
	public bool AddBeastCode                    = false;
	public bool LightProbes                     = false;
	
	public float lightLookAhead          		= 1.0f;
	public float AmbientLightAdjust             = 0.5f;

	public bool MainDirLight             		= true;
	public float MainDirAdjust                  = 1.0f;
	public bool MainDirOnlyShadows              = false;
	public bool MainDirBump                  	= true;
	public bool MainDirSpec                     = true;
	public bool MainDirSpot                     = false;
	public float MainDirSpotSize                = 0.5f;
	public bool MainDirShadowColorEnable        = false;
	public Color MainDirShadowColor        		= new Color(1,0.5f,0.1f,1);

	public bool MainPointLight                  = false;
	public float MainPointAdjust                = 1.0f;
	public bool MainPointBump                	= false;
	public bool MainPointSpec                   = false;
	public bool MainPointFalloff                = false;
	public bool MainPointRampFade               = true;
	
	public bool FourPointLights                 = false;
	public float FourPointAdjust                = 1.0f;
	public bool FourPointSpec                   = false;
	
	private Color _mainDirShadowColor           = new  Color(0,0,0,0);
	private float _mainDirSpotSize              = 0.0f;
	
	//============================================================
	void Awake()
	{
		_initFlag = true;
		try
		{
			EchoFXEvent.PoolAlloc ( maxEchoFXEvents, dynamicAdd );
		}
		catch
		{
			return;
		}
	}
	
	//============================================================
	void OnEnable()
	{
		_initFlag = true;
	}
	
	//============================================================
	void OnLevelWasLoaded()
	{
		_initFlag = true;
	}
	
	//============================================================
	void OnDisable()
	{
		_initFlag 	= false;
		_last 		= null;
		_first 		= null;
		
		EchoGameObject._systemInitFlag = false;
	}
	
	void OnDestroy()
	{
		_initFlag 	= false;
		_last 		= null;
		_first 		= null;
		
		EchoGameObject._systemInitFlag = false;
		EchoFXEvent.poolListID = -1;
	}
	
//============================================================
	public static void AddList( EchoLight iel )
	{
		if ( iel.inList )
			return;
		
		if ( _first == null )
		{
			_first 		= iel;
			_last 		= iel;
			_last.next  = null;
		}
		else
		{
			_last.next  = iel;
			iel.prev  	= _last;
			_last 		= iel;
			_last.next  = null;
		}
		
		_use4ExtraPoint = true;
		iel.inList 		= true;
	}

//============================================================
	public static void RemoveList ( EchoLight iel )
	{
		if ( !iel.inList )
			return;

		iel.inList = false;
		
		if ( _first == null )
			return;

		if ( iel.prev != null )
			iel.prev.next = iel.next;
		
		if ( iel.next )
			iel.next.prev = iel.prev;

		if ( iel == _first && iel == _last )
		{
			_use4ExtraPoint = false;
			_last 			= null;
			_first 			= null;
			Set4ExtraLightsBlack();
		}
		else
		{
			if ( iel == _first )
			{
				_first = iel.next;
			}
			
			if ( iel == _last )
			{
				_last = iel.prev;
			}
		}
	}

//============================================================
	bool SetupLevelLoad()
	{
		bool rc = false;

		useUnityLights = UnityLights;
		
		ResetShaderVariables();
		SetAllLightsBlack();
		rc = InitLights();

		if ( _useLights )
			SetLightInfo();
		
		return ( rc );
	}

//============================================================
	bool InitLights()
	{
		Light light;
		EchoLight el;
		
		if ( _cameraTransform == null && Camera.main != null   )
			_cameraTransform = Camera.main.transform;
		
		if ( _found == null )
		 	_found = new EchoLight[4];
		
		_lights = UnityEngine.Object.FindObjectsOfType( typeof (Light)) as Light[];

		_mainPointLight = null;
		_mainDirLight   = null;
		_useLights 		= false;
		_use4ExtraPoint = false;
		_extraLights    = null;
		_first          = null;
		_last           = null;
		_use4ExtraPoint = false;
		_last 			= null;
		_first 			= null;
		Set4ExtraLightsBlack();
		
		if ( _extraLights == null )
			_extraLights = new EchoLight[4];
		
		if ( MainDirLight || MainPointLight || FourPointLights )
			_useLights = true;
		else
			return ( false );

		
		// loop thru unity lights and turn them info EchoLights
		if ( _lights != null && _lights.Length > 0 )
		{
			for ( int loop = 0; loop < _lights.Length; loop++ )
			{
				light = _lights[loop];
			
// SCOTTFIND   this may be unity 4.2 only
#if !UNITY_3_5
#if !UNITY_4_1
				if ( light.alreadyLightmapped ) 
					continue;
#endif
#endif
				if ( light.renderMode != LightRenderMode.ForceVertex || ( _lights.Length <= 2 && FourPointLights == false ) )
				{
					if ( light.type == LightType.Point )
					{
						_mainPointLight = light.gameObject.GetComponent<EchoLight>();
						
						if ( _mainPointLight == null )
							_mainPointLight = light.gameObject.AddComponent<EchoLight>();
						
						_mainPointLight.Init();
					}
					else
					{
						_mainDirLight = light.gameObject.GetComponent<EchoLight>();
						
						if ( _mainDirLight == null )
							_mainDirLight = light.gameObject.AddComponent<EchoLight>();
						
						_mainDirLight.Init();
					}
				}
				else
				{
					el = light.gameObject.GetComponent<EchoLight>();
						
					if ( el == null )
						el = light.gameObject.AddComponent<EchoLight>();

					el.inList = false;
					el.Init();
				}
			}
		}
		
		return ( true );
	}

	//============================================================
	public static void Set4ExtraLightsBlack()
	{
		int loop;

		for ( loop = 0; loop < 4; loop++ )
		{
			_echo4LightColorR[loop]	= 0.0f;
			_echo4LightColorG[loop]	= 0.0f;
			_echo4LightColorB[loop]	= 0.0f;
			
			_echo4LightColorRH[loop]	= 1.0f;
			_echo4LightColorGH[loop]	= 1.0f;
			_echo4LightColorBH[loop]	= 1.0f;
		}

		Shader.SetGlobalColor ("_echo4LightColorR", _echo4LightColorR );
		Shader.SetGlobalColor ("_echo4LightColorG", _echo4LightColorG );
		Shader.SetGlobalColor ("_echo4LightColorB", _echo4LightColorB );
	}
	
	//============================================================
	public static void SetAllLightsBlack()
	{
		Set4ExtraLightsBlack();
		
		_echoDirLightColor =  new Vector4(0,0,0,0);
		Shader.SetGlobalColor ("_echoDirLightColor", _echoDirLightColor );
		
		_echoPointLightColor =  new Vector4(0,0,0,0);
		Shader.SetGlobalColor ("_echoPointLightColor", _echoPointLightColor );
	}

	//============================================================
	public static void ResetShaderVariables()
	{
		_echo4LightColorRH		= new Vector4(-1,-1,0,0);
		_echo4LightColorGH		= new Vector4(-1,-1,0,0);
		_echo4LightColorBH		= new Vector4(-1,-1,0,0);
//		_echo4LightIntensityH	= new Vector4(-1,-1,0,0);
		
		_echo4LightPosXH		= new Vector4(-9999.5f,128.128f,1,-1);
		_echo4LightPosYH		= new Vector4(-9999.5f,128.128f,1,-1);
		_echo4LightPosZH		= new Vector4(-9999.5f,128.128f,1,-1);
		_echo4LightRangeH		= new Vector4(-9999.5f,128.128f,1,-1);
		
		_echo4LightColorR		= new Vector4(0,0,0,0);
		_echo4LightColorG		= new Vector4(0,0,0,0);
		_echo4LightColorB		= new Vector4(0,0,0,0);
//		_echo4LightIntensity	= new Vector4(0,0,0,0);

		_echo4LightPosX			= new Vector4(-9999.5f,128.128f,9,-1);
		_echo4LightPosY			= new Vector4(-9999.5f,128.128f,9,-1);
		_echo4LightPosZ			= new Vector4(-9999.5f,128.128f,9,-1);
		_echo4LightRange		= new Vector4(-9999.5f,128.128f,9,-1);
		
		_echoDirLightColor  	= new Vector4 (0,0,0,0);
		_echoDirLightPos  		= new Vector4 (-9999.5f,9999.5f,1,-1);
		_echoDirLightDir  		= new Vector4 (-9999.5f,9999.5f,1,-1);
		_echoPointLightColor	= new Vector4 (0,0,0,0);
		_echoPointLightPos		= new Vector4 (-9999.5f,9999.5f,1,-1);
		_echoPointLightRange	= -1;

		_echoAmbientLightColor	= new Vector4 (-1,0,0,0);
	}
	
	//============================================================
	void Start()
	{
//		Shader.WarmupAllShaders();
	}
	
	//============================================================
	void Update()
	{
		if ( _initFlag )
		{
			_initFlag = false;
			SetupLevelLoad();
		}
		
#if UNITY_EDITOR
		if ( Application.isPlaying == false )
			SetupLevelLoad();
#endif
		if ( _useLights )
		{
			SetLightInfo();
		}
	}


	//============================================================
	void LateUpdate () 
	{
		if ( maxEchoFXEvents < 1 )
			return;
		
		try
		{
			EchoFXEvent.ProcessAllInUpdate();		
		}
		catch
		{
			return;
		}
	}
	
	
	//============================================================
	public static void TurnOffLight( EchoLightType elt )
	{
		switch ( elt )
		{
		case EchoLightType.MAIN_DIRECTIONAL:
			_echoDirLightColor =  new Vector4(0,0,0,0);
			Shader.SetGlobalColor ("_echoDirLightColor", _echoDirLightColor );
		break;
			
		case EchoLightType.MAIN_POINT:
			_echoPointLightColor =  new Vector4(0,0,0,0);
			Shader.SetGlobalColor ("_echoPointLightColor", _echoPointLightColor );
			break;
			
		default:
			break;
		}
	}

	//============================================================
	// Check 2 Vec4 for not being equal in a speedy way
	//============================================================
	public static bool Vector4NotEqual ( Vector4 ivec1, Vector4 ivec2 ) 
	{
		float num;
		
		num = ivec1.x - ivec2.x;
		num = (num > 0.0f) ? num : -num;

		if ( num > 0.001f )
			return ( true );

		num =ivec1.y - ivec2.y;
		num = (num > 0.0f) ? num : -num;
		
		if ( num > 0.001f )
			return ( true );
		
		num =ivec1.z - ivec2.z;
		num = (num > 0.0f) ? num : -num;
		
		if ( num > 0.001f )
			return ( true );
		
		num =ivec1.w - ivec2.w;
		num = (num > 0.0f) ? num : -num;
		
		if ( num > 0.001f )
			return ( true );

		return ( false );
	}


	//============================================================
	public void SetLightInfo()
	{
		Vector4 vec4;
		EchoLight el;
		int loop;
		float intensity;
		int index;
		int fpos;
		float bigdist;
		
		if ( _mainDirLight != null )
		{
			if ( _mainDirLight.lightOn )
			{
				intensity = _mainDirLight.uLight.intensity * MainDirAdjust;
				vec4 = _mainDirLight.uLight.color * intensity;
				vec4.w = intensity;
			}
			else
				vec4 = new Vector4 ( 0.0f, 0.0f, 0.0f, 0.0f );
			
			if ( Vector4NotEqual ( vec4, _echoDirLightColor ) )
			{
				_echoDirLightColor = vec4;
				Shader.SetGlobalColor ("_echoDirLightColor", _echoDirLightColor );
			}
			
			vec4.x = _mainDirLight.cachedTransform.position.x;
			vec4.y = _mainDirLight.cachedTransform.position.y;
			vec4.z = _mainDirLight.cachedTransform.position.z;
			vec4.w = 1.0f;
			
			if ( Vector4NotEqual ( vec4, _echoDirLightPos ) )
			{
				_echoDirLightPos = vec4;
				Shader.SetGlobalColor ("_echoDirLightPos", _echoDirLightPos );
			}
								
			vec4.x = -_mainDirLight.cachedTransform.forward.x;
			vec4.y = -_mainDirLight.cachedTransform.forward.y;
			vec4.z = -_mainDirLight.cachedTransform.forward.z;
			vec4.w = 1.0f;
			
			if ( Vector4NotEqual ( vec4, _echoDirLightDir ) )
			{
				_echoDirLightDir = vec4;
				Shader.SetGlobalColor ("_echoDirLightDir", _echoDirLightDir.normalized );
			}
			
			if ( MainDirSpot )
			{
				if ( MainDirSpotSize != _mainDirSpotSize )
				{
					_mainDirSpotSize = MainDirSpotSize;
					Shader.SetGlobalFloat ("_echoDirSpotSize", MainDirSpotSize );
				}
			}
			
			if ( MainDirShadowColorEnable )
			{
				//if ( MainDirShadowColor != _mainDirShadowColor )
				if ( Vector4NotEqual ( MainDirShadowColor, _mainDirShadowColor ) )
				{
					_mainDirShadowColor = MainDirShadowColor;
					Shader.SetGlobalColor ("_echoShadowColor", MainDirShadowColor );
				}
			}
		}
		
		
		
		if ( _mainPointLight != null )
		{
			if ( _mainPointLight.lightOn )
			{
				intensity = _mainPointLight.uLight.intensity * MainPointAdjust;
				vec4 = _mainPointLight.uLight.color * intensity;
				vec4.w = intensity;
			}
			else
				vec4 = new Vector4 ( 0.0f, 0.0f, 0.0f, 0.0f );
			
			if ( Vector4NotEqual ( vec4, _echoPointLightColor ) )
			{
				_echoPointLightColor = vec4;
				Shader.SetGlobalColor ("_echoPointLightColor", _echoPointLightColor );
			}
			
			vec4.x = _mainPointLight.cachedTransform.position.x;
			vec4.y = _mainPointLight.cachedTransform.position.y;
			vec4.z = _mainPointLight.cachedTransform.position.z;
			vec4.w = 1.0f;
			
			if ( Vector4NotEqual ( vec4, _echoDirLightPos ) )
			{
				_echoPointLightPos = vec4;
				Shader.SetGlobalColor ("_echoPointLightPos", _echoPointLightPos );
			}
			
			if ( _mainPointLight.uLight.range != _echoPointLightRange )
			{
				_echoPointLightRange = _mainPointLight.uLight.range;
				Shader.SetGlobalFloat ("_echoPointLightRange", _echoPointLightRange );
			}
		}

		
		if ( Camera.main != null )
			_cameraTransform = Camera.main.transform;
		
		if ( _use4ExtraPoint && _cameraTransform != null )
		{
		
			_lookAhead = _cameraTransform.position + _cameraTransform.forward * lightLookAhead;
					
			_extraLightCount = 0;
			
			for ( el = _first; el != null; el = el.next )
			{
				el.dist = Vector3.Distance ( _lookAhead, el.cachedTransform.position );
				
				fpos = 0;
				
				if ( _extraLightCount < 4 )
				{
					_extraLights[_extraLightCount] 	= el;
					_extraLightCount++;
				}
				else
				{
					// search thru found lights
					for ( loop = 0; loop < _extraLightCount; loop++ )
					{
						if ( el.dist < _extraLights[loop].dist )
						{
							_extraLights[loop].findex = loop;
							_found[fpos] = _extraLights[loop];
							fpos++;
						}
					}
					
					bigdist = 0;
					index = -1;
					
					for ( loop = 0; loop < fpos; loop++ )
					{
						if ( _found[loop].dist > bigdist )
						{
							index = _found[loop].findex;
							bigdist = _found[loop].dist;
						}
					}
					
					if ( index >= 0 )
					{
						_extraLights[index] 			= el;
					}
				}
			}
			
			for ( loop = 0; loop < _extraLightCount; loop++ )
			{
				el = _extraLights[loop];
				
				intensity = el.uLight.intensity * FourPointAdjust;
				_echo4LightColorRH[loop]	= Mathf.Clamp ( el.uLight.color.r * intensity, 0, 8 );
				_echo4LightColorGH[loop]	= Mathf.Clamp ( el.uLight.color.g * intensity, 0, 8 );
				_echo4LightColorBH[loop]	= Mathf.Clamp ( el.uLight.color.b * intensity, 0, 8 );
//				_echo4LightIntensityH[loop]	= intensity;
				
				_echo4LightPosXH[loop]		= el.cachedTransform.position.x;
				_echo4LightPosYH[loop]		= el.cachedTransform.position.y;
				_echo4LightPosZH[loop]		= el.cachedTransform.position.z;
				_echo4LightRangeH[loop]		= el.uLight.range;
			}
			
			for ( ; loop < 4; loop++ )
			{
				_echo4LightColorRH[loop]	= 0.0f;
				_echo4LightColorGH[loop]	= 0.0f;
				_echo4LightColorBH[loop]	= 0.0f;
				_echo4LightRangeH[loop]		= 0.0f;
				_echo4LightPosXH[loop]		= 0.0f;
				_echo4LightPosYH[loop]		= 0.0f;
				_echo4LightPosZH[loop]		= 0.0f;
			}
			
			if ( Vector4NotEqual ( _echo4LightColorRH, _echo4LightColorR ) )
			{
				_echo4LightColorR = _echo4LightColorRH;
				Shader.SetGlobalColor ("_echo4LightColorR", _echo4LightColorR );
			}

			if ( Vector4NotEqual ( _echo4LightColorGH, _echo4LightColorG ) )
			{
				_echo4LightColorG = _echo4LightColorGH;
				Shader.SetGlobalColor ("_echo4LightColorG", _echo4LightColorG );
			}

			if ( Vector4NotEqual ( _echo4LightColorBH, _echo4LightColorB ) )
			{
				_echo4LightColorB = _echo4LightColorBH;
				Shader.SetGlobalColor ("_echo4LightColorB", _echo4LightColorB );
			}
			
			// NOT USED YET
//			if ( _echo4LightIntensityH != _echo4LightIntensity )
//			{
//				_echo4LightIntensity = _echo4LightIntensityH;
//				Shader.SetGlobalColor ("_echo4LightColorIntensity", _echo4LightIntensity );
//			}

			if ( Vector4NotEqual ( _echo4LightPosXH, _echo4LightPosX ) )
			{
				_echo4LightPosX = _echo4LightPosXH;
				Shader.SetGlobalColor ("_echo4LightPosX", _echo4LightPosX );
			}

			if ( Vector4NotEqual ( _echo4LightPosYH, _echo4LightPosY ) )
			{
				_echo4LightPosY = _echo4LightPosYH;
				Shader.SetGlobalColor ("_echo4LightPosY", _echo4LightPosY );
			}

			if ( Vector4NotEqual ( _echo4LightPosZH, _echo4LightPosZ ) )
			{
				_echo4LightPosZ = _echo4LightPosZH;
				Shader.SetGlobalColor ("_echo4LightPosZ", _echo4LightPosZ );
			}

			if ( Vector4NotEqual ( _echo4LightRangeH, _echo4LightRange ) )
			{
				_echo4LightRange = _echo4LightRangeH;
				Shader.SetGlobalColor ("_echo4LightRange", _echo4LightRange );
			}

		}

		if ( Vector4NotEqual ( RenderSettings.ambientLight, _echoAmbientLightColor ) )
		{
			_echoAmbientLightColor = RenderSettings.ambientLight; 
			Shader.SetGlobalColor ("_echoAmbientLightColor", _echoAmbientLightColor * AmbientLightAdjust );
			Shader.SetGlobalColor ("_echoAmbientLightColorF", _echoAmbientLightColor * AmbientLightAdjust );
		}
	}
}
