//$-----------------------------------------------------------------------------
//@ Lighted Shader		- 2 Textures and normal mapping
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
Shader "echoLogin/Light/Relfective/20-2Layer-2Tex-BumpMap"
{
   	Properties 
	{
    	_MainTex ("Texture Alpha=Bumpmap", 2D)			= "black" {} 
    	_NormalMap ("Normal Map", 2D)					= "black" {} 
     	_EnvMap ("Reflection", 2D)	= "blue"  {}
       	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
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
			#define ECHO_BUMPMAP_SHADER
			#include "../Include/EchoLogin.cginc"
			#include "../Include/EchoLogin-Light.cginc"
			
			sampler2D 	_NormalMap;
			sampler2D 	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			sampler2D	_EnvMap;
			float4		_EnvMap_ST;
			
			// Vertex and Frag BUMP Shader     ===================== 	
			#define ECHODEF_REFLECTION
			#include "echologin_litbumpmap.cginc"

			ENDCG

		}

	}

	Fallback "echoLogin/Light/Solid/Color"
}
 
