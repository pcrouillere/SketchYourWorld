//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Planet shader with rim lighting.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//# _echoRGBA          	- RGB Multiply
//# _Color          	- Color of rim lighting
//# _echoLightSize      - Size of rim lighting on lighted side
//# _echoDarkSize       - Size of rim lighting on dark side 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/FX/Planet-Rimlit"
{
   	Properties 
	{
    	_MainTex ("Texture BackGround", 2D)								= "black" {} 
       	_echoUV("UV Offset u1 v1 u2 v2", Vector )						= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
		_echoRGBA ( "RGB Multiply", Vector )	= ( 1, 1, 1, 1 )    
     	_Color ("Rim Color", Color )								= ( 1,1,1,1)
     	_echoLightSize ("Lighted Side Rim Size", Range ( 1.0, 0.0 ) )	= 0.1
     	_echoDarkSize ("Dark Side Rim Size", Range ( 1.0, 0.0 ) )		= 0.4
 	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry+100" "IgnoreProjector"="False" "RenderType"="echoLight" }

    	Pass 
		{    
			Name "BASE"
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
			float4      _Color;
			float4		_echoRGBA;
			float       _echoLightSize;
			float       _echoDarkSize;

 			// Vertex/frag Shaders     ===================== 	
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_VERTEXCOLOR
			#define ECHODEF_RIMLIT
			#define ECHODEF_CUSTOMRETURN return fixed4 ( lerp ( fcolor * lcolor ECHO_DOUBLELIGHT3, _Color.xyz, v.rimmix ), 1 );
			#include "echologin_LitVertexShader.cginc"
 
			ENDCG
		}
 	}
 	
	Fallback "echoLogin/Light/Solid/Color"

}
 
