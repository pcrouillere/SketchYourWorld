//$-----------------------------------------------------------------------------
//@ Lighted Shader		- 2 texture relfection
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
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/Relfective/20-2Layer-2Tex"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)				= "black" {} 
     	_EnvMap ("Reflection", 2D)				= "blue"  {}
       	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
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
			
			sampler2D 	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			sampler2D	_EnvMap;
			float4		_EnvMap_ST;


 			// Vertex\Frag Shader     =====================
			#define ECHODEF_REFLECTION 
			#define ECHODEF_REFLECTION_STXY _EnvMap_ST.xy
			#define ECHODEF_REFLECTION_STZW _EnvMap_ST.zw
			#define ECHODEF_FIXED4TEX
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_CUSTOMCODE  fcolor.xyz = lerp ( fcolor.xyz, tex2D ( _EnvMap, v.tc2  ).xyz, fcolor.www );fcolor.w = 1.0;
			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}
		
	}
	Fallback "echoLogin/Light/Solid/Color"
}
 
