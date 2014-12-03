//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Flag waving in wind effect.
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
//# _echoSpeed          - Speed of ripple
//# _echoHeight         - Height/size of each ripple
//# _echoAmount         - Amount of ripples
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/FX/Flag"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)								= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )						= ( 0, 0, 0, 0 )
       	_echoScale ("Scale XYZ", Vector )						= ( 1.0, 1.0, 1.0, 1.0 )
  		_echoSpeed ("Wave Speed", Range ( 0.0, 256.0 ) )		= 16
   		_echoHeight ("Wave Height", Range ( 0.0, 32.0 ) )		= 1
   		_echoAmount ("Wave Amount", Range ( 1.0, 32.0 ) )		= 4
   	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry" "IgnoreProjector"="False" "RenderType"="echoLight" }

 		// PASS FOR FRONT ======================================================================
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
			float       _echoSpeed;
			float       _echoHeight;
			float       _echoAmount;

			// Vertex\Frag Shader     =====================
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_RIPPLE
			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}
				
// PASS FOR BACK ======================================================================
	   	Pass 
		{    
			Tags { "LightMode" = "ForwardBase" }
 
      		Cull Front
     		
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
			float       _echoSpeed;
			float       _echoHeight;
			float       _echoAmount;

			// Vertex\Frag Shader     =====================
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_RIPPLE
			#define ECHODEF_FLIPNORMALs
			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}

 	}
 	
	Fallback "echoLogin/Light/Solid/Color"
}
 
