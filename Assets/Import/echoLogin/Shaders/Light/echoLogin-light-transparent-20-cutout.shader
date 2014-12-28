//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Uses alpha channel for lightmapping.
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
Shader "echoLogin/Light/Transparent/20-cutout"
{
   	Properties 
	{
    	_MainTex ("Texture Alpha is cutout", 2D)		= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
    	_echoMix("Mix", Range ( -0.1, 1.0 ) )			= 0.5
  	}
   	
	//=========================================================================
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="False" "RenderType"="TransparentCutout"}

		// for front face
    	Pass 
		{    
			Name "FRONT"
			Tags { "LightMode" = "ForwardBase" }
       	 	
       	 	ZWrite Off
      	 	Blend SrcAlpha OneMinusSrcAlpha
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
			float4	  	_MainTex_ST;
			float4 		_MainTex_TexelSize;
			fixed		_echoMix;

			// Vertex\Frag Shader     ===================== 	
			#define ECHODEF_FIXED4TEX
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_CUSTOMCODE fcolor.w = step ( _echoMix, fcolor.w );
			#define ECHODEF_CUSTOMRETURN return fixed4 ( fcolor.xyz * lcolor ECHO_DOUBLELIGHT3, fcolor.w );
			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}

		// for back face
    	Pass 
		{    
			Name "BACK"
			Tags { "LightMode" = "ForwardBase" }
       	 	
       	 	ZWrite Off
      	 	Blend SrcAlpha OneMinusSrcAlpha
      		Cull front
     		
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
			float4	  	_MainTex_ST;
			float4 		_MainTex_TexelSize;
			fixed		_echoMix;

			// Vertex\Frag Shader     ===================== 	
			#define ECHODEF_FIXED4TEX
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_FLIPNORMAL
			#define ECHODEF_CUSTOMCODE fcolor.w = step ( _echoMix, fcolor.w );
			#define ECHODEF_CUSTOMRETURN return fixed4 ( fcolor.xyz * lcolor ECHO_DOUBLELIGHT3, fcolor.w );
			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}
 	}
 	
	Fallback "echoLogin/Light/Solid/Color"
}
 
