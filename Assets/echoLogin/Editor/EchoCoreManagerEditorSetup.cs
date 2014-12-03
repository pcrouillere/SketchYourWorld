using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[CustomEditor (typeof(EchoCoreManager))]
public class EchoCoreManagerEditorSetup : Editor 
{
		private GUISkin _coreSkin;
		private static bool _needCompile = false;
		private SerializedObject echoShader;
		private SerializedProperty
			dynamicAdd,
			maxEchoFXEvents,
			unityLights,
			doubleLight,
			addBeastCode,
			ambientLightAdjust,
			lightLookAhead,
			mainDirLight,
			mainDirAdjust,
			mainDirOnlyShadows,
			mainDirSpot,
			mainDirSpotSize,
			mainDirBump,
			mainDirSpec,
			mainDirShadowColorEnable,
			mainDirShadowColor,
			mainPointLight,
			mainPointAdjust,
			mainPointFalloff,
			mainPointRampFade,
			mainPointBump,
			mainPointSpec,
			fourPointLights,
			fourPointAdjust,
			fourPointSpec,
			lightProbes;
	
	//============================================================
	void OnEnable () 
	{
		 _coreSkin = Resources.Load("echoLoginCoreSkin") as GUISkin;
		
		echoShader 			= new SerializedObject(target);
		
		dynamicAdd          = echoShader.FindProperty("dynamicAdd");
		maxEchoFXEvents     = echoShader.FindProperty("maxEchoFXEvents");
		
		unityLights         = echoShader.FindProperty("UnityLights");
		doubleLight         = echoShader.FindProperty("DoubleLight");
		addBeastCode        = echoShader.FindProperty("AddBeastCode");
		lightLookAhead      = echoShader.FindProperty("lightLookAhead");
		ambientLightAdjust 	= echoShader.FindProperty("AmbientLightAdjust");

		mainDirLight 		= echoShader.FindProperty("MainDirLight");
		mainDirAdjust 		= echoShader.FindProperty("MainDirAdjust");
		mainDirOnlyShadows	= echoShader.FindProperty("MainDirOnlyShadows");
		mainDirSpot 		= echoShader.FindProperty("MainDirSpot");
		mainDirSpotSize 	= echoShader.FindProperty("MainDirSpotSize");
		mainDirSpec 		= echoShader.FindProperty("MainDirSpec");
		mainDirBump 		= echoShader.FindProperty("MainDirBump");
		mainDirShadowColorEnable	= echoShader.FindProperty("MainDirShadowColorEnable");
		mainDirShadowColor	= echoShader.FindProperty("MainDirShadowColor");

		mainPointLight 		= echoShader.FindProperty("MainPointLight");
		mainPointAdjust 	= echoShader.FindProperty("MainPointAdjust");
		mainPointSpec 		= echoShader.FindProperty("MainPointSpec");
		mainPointBump 		= echoShader.FindProperty("MainPointBump");
		mainPointFalloff	= echoShader.FindProperty("MainPointFalloff");
		mainPointRampFade	= echoShader.FindProperty("MainPointRampFade");

		fourPointLights		= echoShader.FindProperty("FourPointLights");
		fourPointAdjust 	= echoShader.FindProperty("FourPointAdjust");
		fourPointSpec		= echoShader.FindProperty("FourPointSpec");

		lightProbes			= echoShader.FindProperty("LightProbes");
		
		EditorApplication.playmodeStateChanged = AutoCompileShaders;
	}
	
	//============================================================
	public void CompileShaders()
	{
		EchoCoreManager.ResetShaderVariables();
		
		_needCompile = false;
		
		StreamWriter sw = new StreamWriter("Assets/echoLogin/Shaders/Light/echologin_shaderoptions.cginc");
		
		if ( doubleLight.boolValue )
		{
			sw.WriteLine ("#define ECHO_DOUBLELIGHT_ON");
			sw.WriteLine ("#define ECHO_DOUBLELIGHT1 * 2");
			sw.WriteLine ("#define ECHO_DOUBLELIGHT3 * fixed3(2,2,2)");
			sw.WriteLine ("#define ECHO_DOUBLELIGHT4 * fixed4(2,2,2,2)");
		}
		else
		{
			sw.WriteLine ("#define ECHO_DOUBLELIGHT1 ");
			sw.WriteLine ("#define ECHO_DOUBLELIGHT3 ");
			sw.WriteLine ("#define ECHO_DOUBLELIGHT4 ");
		}
		
		if ( addBeastCode.boolValue ) 
		{
			sw.WriteLine ("#define ECHO_ADDBEAST_CODE");
			if ( lightProbes.boolValue )
			{
				sw.WriteLine ("#define ECHO_SHLIGHT_ON");
			}
		}

		if ( !mainDirLight.boolValue && !mainPointLight.boolValue && !fourPointLights.boolValue && !lightProbes.boolValue )
		{
			sw.WriteLine ("#define ECHO_DIR_ON");
			sw.WriteLine ("#define ECHO_DIRSPEC_OFF");
			sw.WriteLine ("#define ECHO_DIRDLIGHT_ON");
			
			sw.Flush();
			sw.Close();
		
    		AssetDatabase.Refresh();
			return;
		}
		
		if ( mainDirLight.boolValue )
		{
			sw.WriteLine ("#define ECHO_DIR_ON");
			
			if ( addBeastCode.boolValue && mainDirOnlyShadows.boolValue ) 
				sw.WriteLine ("#define ECHO_DIR_ONLYSHADOW");
			
			if ( mainDirSpec.boolValue )
				sw.WriteLine ("#define ECHO_DIRSPEC_ON");
			else
				sw.WriteLine ("#define ECHO_DIRSPEC_OFF");
			
			if ( mainDirBump.boolValue )
				sw.WriteLine ("#define ECHO_DIRBUMP_ON");
			else
				sw.WriteLine ("#define ECHO_DIRDLIGHT_ON");
			
			if ( mainDirSpot.boolValue )
				sw.WriteLine ("#define ECHO_DIRSPOT_ON");
				
			
			if ( mainDirShadowColorEnable.boolValue )
			{
				sw.WriteLine ("#define ECHO_DIRSHADOWCOLOR_ON");
			}
		}

		if ( mainPointLight.boolValue )
		{
			sw.WriteLine ("#define ECHO_POINT_ON");

			if ( mainPointFalloff.boolValue )
			{
				sw.WriteLine ("#define ECHO_POINTFALLOFF_ON");
				if ( mainPointRampFade.boolValue )
				{
					sw.WriteLine ("#define ECHO_POINT_RAMPFADE");
				}
			}
			
			if ( mainPointBump.boolValue )
				sw.WriteLine ("#define ECHO_POINTBUMP_ON");
			else
				sw.WriteLine ("#define ECHO_POINTDLIGHT_ON");

			if ( mainPointSpec.boolValue )
				sw.WriteLine ("#define ECHO_POINTSPEC_ON");
			else
				sw.WriteLine ("#define ECHO_POINTSPEC_OFF");
		}

		if ( fourPointLights.boolValue )
		{
			sw.WriteLine ("#define ECHO_4POINT_ON");
			
			if ( fourPointSpec.boolValue )
					sw.WriteLine ("#define ECHO_4POINTSPEC_ON");
		}


		sw.Flush();
		sw.Close();
		
    	AssetDatabase.Refresh();
	}
	
	//============================================================
	public void AutoCompileShaders()
	{
		if ( _needCompile == true )
			CompileShaders();
	}
	
	//============================================================
	public override void OnInspectorGUI()
	{
		String bstring;
		
		GUI.skin = _coreSkin;
			
		EditorGUILayout.Space();
		EditorGUILayout.LabelField(" EchoFXEvents", _coreSkin.customStyles[2]  );
		EditorGUILayout.PropertyField ( maxEchoFXEvents, new GUIContent("Pre-Allocate Events") );
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField ( dynamicAdd, new GUIContent("Add Events As Needed") );
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField(" Options For Lit Shaders", _coreSkin.customStyles[2]  );
		EditorGUILayout.Space();

		EditorGUILayout.LabelField(" Use Unity Lights", _coreSkin.customStyles[0] );
		EditorGUILayout.PropertyField ( unityLights, new GUIContent("Enabled") );
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField(" Ambient Light", _coreSkin.customStyles[0] );
		EditorGUILayout.Slider ( ambientLightAdjust, 0.0f, 2.0f, new GUIContent("Multiply Intensity") );
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUI.BeginChangeCheck();

		EditorGUILayout.LabelField(" Double Light in Frag Shader", _coreSkin.customStyles[0] );
		EditorGUILayout.PropertyField ( doubleLight, new GUIContent("Enabled") );
		EditorGUILayout.Space();

		EditorGUILayout.LabelField(" Add Beast Code", _coreSkin.customStyles[0] );
		EditorGUILayout.PropertyField ( addBeastCode, new GUIContent("Enabled") );

		if ( addBeastCode.boolValue ) 
		{
			EditorGUILayout.PropertyField ( lightProbes, new GUIContent("Use LightProbes") );
			EditorGUILayout.Space();
	}

		//
		//  DIR LIGHT
		//
		EditorGUILayout.LabelField(" Main Directional Light", _coreSkin.customStyles[0] );
		
		EditorGUILayout.PropertyField ( mainDirLight, new GUIContent("Enabled") );
		if ( mainDirLight.boolValue )
		{
			if ( EditorGUI.EndChangeCheck() )
				_needCompile = true;
			
			EditorGUILayout.Slider ( mainDirAdjust, 0.0f, 2.0f, new GUIContent("Multiply Intensity") );

			EditorGUI.BeginChangeCheck();
			
			if ( addBeastCode.boolValue == true )
				EditorGUILayout.PropertyField ( mainDirOnlyShadows, new GUIContent("Only Shadow ( Beast )")  );
			
			EditorGUILayout.PropertyField ( mainDirSpot, new GUIContent("Use As Spotlight")  );
			
			if ( EditorGUI.EndChangeCheck() )
				_needCompile = true;

			if ( mainDirSpot.boolValue )
			{
				EditorGUILayout.Slider ( mainDirSpotSize, 0.0f, 1.0f, new GUIContent("Spot Beam Size") );
			}
			
			EditorGUI.BeginChangeCheck();
			
			EditorGUILayout.PropertyField ( mainDirShadowColorEnable, new GUIContent("Enable Shadow Tint")  );
			
			if ( EditorGUI.EndChangeCheck() )
				_needCompile = true;
			
			if ( mainDirShadowColorEnable.boolValue )
			{
				EditorGUILayout.PropertyField ( mainDirShadowColor, new GUIContent("Shadow Color")  );
			}
			
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("   Affect Shaders Of Type ",_coreSkin.customStyles[1]);
			EditorGUILayout.PropertyField ( mainDirSpec, new GUIContent("   Specular/Rim/Ramp")  );
			EditorGUILayout.PropertyField ( mainDirBump, new GUIContent("   Bumpmap")  );
		}
		
		if ( EditorGUI.EndChangeCheck() )
			_needCompile = true;
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		//
		//  POINT LIGHT
		//
		EditorGUILayout.LabelField(" Main Point Light", _coreSkin.customStyles[0] );

		EditorGUI.BeginChangeCheck();

		EditorGUILayout.PropertyField ( mainPointLight, new GUIContent("Enabled") );
		if ( mainPointLight.boolValue )
		{
			if ( EditorGUI.EndChangeCheck() )
				_needCompile = true;

			EditorGUILayout.Slider ( mainPointAdjust, 0.0f, 2.0f, new GUIContent("Multiply Intensity") );
			
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField ( mainPointFalloff, new GUIContent("Light Falloff") );
			if ( mainPointFalloff.boolValue )
			{
				EditorGUILayout.PropertyField ( mainPointRampFade, new GUIContent("Ramp Fade") );
			}
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("   Affect Shaders Of Type ",_coreSkin.customStyles[1]);
			EditorGUILayout.PropertyField ( mainPointSpec, new GUIContent("   Specular/Rim/Ramp") );
			EditorGUILayout.PropertyField ( mainPointBump, new GUIContent("   Bumpmap") );

		}

		if ( EditorGUI.EndChangeCheck() )
			_needCompile = true;
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		//
		//  4 EXTRA POINT LIGHTS
		//
		EditorGUILayout.LabelField(" 4 Extra Point Lights", _coreSkin.customStyles[0] );

		EditorGUI.BeginChangeCheck();

		EditorGUILayout.PropertyField ( fourPointLights, new GUIContent("Enabled") );
		
		if ( EditorGUI.EndChangeCheck() )
				_needCompile = true;

		EditorGUI.BeginChangeCheck();

		if ( fourPointLights.boolValue )
		{
			EditorGUILayout.Slider ( fourPointAdjust, 0.0f, 2.0f, new GUIContent("Multiply Intensity") );
			EditorGUILayout.Slider ( lightLookAhead, 0.0f, 128.0f, new GUIContent("Look Ahead") );
			EditorGUILayout.Space();
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.LabelField("   Affect Shaders Of Type ",_coreSkin.customStyles[1]);
			EditorGUILayout.PropertyField ( fourPointSpec, new GUIContent("   Specular") );
		}

		if ( EditorGUI.EndChangeCheck() )
			_needCompile = true;

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		echoShader.ApplyModifiedProperties();
		
		if ( _needCompile )
		{
			bstring = ">>> Compile Shaders <<<";
			GUI.backgroundColor = new Color ( 1.0f, 0.2f, 0.0f );
		}
		else
			bstring = "Compile Shaders";
		
		
		if ( GUILayout.Button(bstring))
        {
			CompileShaders();
        }
	}
};

