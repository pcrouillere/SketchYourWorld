//$-----------------------------------------------------------------------------
//@ Lighted Shader	- Planet shader with clouds.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ NOTE: Clouds are greyscale from alpha channel and each layer has its own UV set.
//@
//@ Properties/Uniforms
//@
//# _echoUV         - Two sets of UV offsets in one Vector4 ( u1, v1, u2, v2 ) 
//# _echoScale      - Scale Mesh in XYZ, W should always be 1
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/FX/Planet-clouds"
{
   	Properties 
	{
    	_MainTex ("Texture A=Clouds", 2D)				= "black" {} 
       	_echoUV ("UV Offset u1 v1 u2 v2", Vector )		= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
 	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry+100" "IgnoreProjector"="False" "RenderType"="echoLight" }

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

			// Vertex/frag Shaders     ===================== 	
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_2NDTEXUV
			#define ECHODEF_CUSTOMUV2TEX fixed3 ncolor = tex2D ( _MainTex, v.tc2 ).www;
			#define ECHODEF_CUSTOMCODE fcolor.xyz = ( fcolor.xyz + ncolor * fixed3(0.5,0.5,0.5) );
			#include "echologin_LitVertexShader.cginc"


			ENDCG
		}
 	}
 	
	Fallback "echoLogin/Light/Solid/Color"

}
 
