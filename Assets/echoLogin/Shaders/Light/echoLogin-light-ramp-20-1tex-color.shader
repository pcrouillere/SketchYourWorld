//$-----------------------------------------------------------------------------
//@ Lighted Ramp/Toon Shader - 
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//# _echoRGBA          	- RGB Multiply
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/Ramp/20-1tex-color"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)											= "black" {} 
      	_echoRampDir("Directional Ramp Texture", 2D ) 						= "grey"{}
      	_echoRampPoint("Point Ramp Texture", 2D ) 							= "grey"{}
       	_echoUV("UV Offset u1 v1", Vector )									= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )									= ( 1.0, 1.0, 1.0, 1.0 )
		_echoRGBA ( "RGB Multiply", Vector )								= ( 1, 1, 1, 1 )    
  	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry" "IgnoreProjector"="False" "RenderType"="echoLight" }

    	Pass 
		{    
			Tags { "LightMode" = "ForwardBase" }
 
      		Cull Back
     		
			CGPROGRAM

			#define DIRECTIONAL
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma multi_compile SHADOWS_SCREEN SHADOWS_OFF

			#include "echologin_shaderoptions.cginc"

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
#ifndef SHADOWS_OFF			  	
			#include "AutoLight.cginc"
#endif

			#define ECHOFIXEDLIGHT
			#define ECHO_RAMP_SHADER
			#include "../Include/EchoLogin.cginc"
			#include "../Include/EchoLogin-Light.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			sampler2D  	_echoRampDir;
			sampler2D  	_echoRampPoint;
			float4		_echoRGBA;
			
			// Multi use RAMP/TOON Code  =====================
			#define ECHODEF_RAMP_TEXTURE	
			#include "echologin_ramp.cginc"

			ENDCG
		}
	}
	
	Fallback "echoLogin/Light/Solid/Color"
}
 
