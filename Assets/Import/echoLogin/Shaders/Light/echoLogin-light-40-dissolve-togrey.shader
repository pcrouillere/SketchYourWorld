//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Dissolve to greyscale.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ NOTE: The alpha channel contains dissolve data and greyscale data.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//# _echoMix            - Dissolve amount 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/40-Dissolve-ToGrey"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)					= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )			= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )			= ( 1.0, 1.0, 1.0, 1.0 )
	   	_echoMix("Mix", Range ( -0.1, 1.1 ) )			= 0
  	}

	//=========================================================================
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="False" "RenderType"="TransparentCutout"}

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
#if defined (ECHO_ADDBEAST_CODE)
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
#endif

			#include "echologin_shaderoptions.cginc"

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
#ifndef SHADOWS_OFF			  	
			#include "AutoLight.cginc"
#endif
			#include "../Include/EchoLogin.cginc"
			#include "../Include/EchoLogin-Light.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			fixed		_echoMix;

			// Vertex/frag Shaders     ===================== 	
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_FIXED4TEX
			#define ECHODEF_CUSTOMCODE fcolor.xyz = lerp ( fcolor.xyz, fcolor.www, step ( fcolor.w, _echoMix ) ); fcolor.w = 1.0;
			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}
 	}
 	
	Fallback "echoLogin/Light/Solid/Color"
}
 
