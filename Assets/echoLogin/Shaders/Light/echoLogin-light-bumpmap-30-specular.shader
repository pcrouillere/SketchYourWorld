//$-----------------------------------------------------------------------------
//@ Lighted Shader		- The fastest textured shader of this group.
//@
//# LIGHT PROBES        - YES ( does not affect bumpmap )
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//# _echoShine          - Shininess
//# _Color          	- Specular Color
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/BumpMap/30-specular"
{
   	Properties 
	{
    	_MainTex ("Texture Alpha=Bumpmap", 2D)			= "black" {} 
    	_NormalMap ("Normal Map", 2D)					= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
     	_echoShine ( "Shininess", Range ( 0.1, 64.0 ) )	= 16
		_Color ("Specular Color", Color ) 				= ( 0.5, 0.5, 0.5, 1.0 )
  	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry" "IgnoreProjector"="False" "RenderType"="echoLight" }

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
			
			#define ECHOFIXEDLIGHT

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
#ifndef SHADOWS_OFF			  	
			#include "AutoLight.cginc"
#endif
			#define ECHO_BUMPMAP_SHADER
			#include "../Include/EchoLogin.cginc"
			#include "../Include/EchoLogin-Light.cginc"

			sampler2D 	_NormalMap;
			sampler2D 	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			float      	_echoShine;
			float4      _Color;

			// Vertex and Frag BUMP Shader     ===================== 	
			#define ECHODEF_SPECULAR
			#include "echologin_litbumpmap.cginc"
		
			ENDCG
		}
	}

	Fallback "echoLogin/Light/Solid/Color"
}
 
