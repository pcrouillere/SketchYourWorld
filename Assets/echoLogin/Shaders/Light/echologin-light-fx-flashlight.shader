//$-----------------------------------------------------------------------------
//@ Lighted Shader		- The fastest textured shader of this group.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoLightSize      - size of the beam
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/FX/Flashlight"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)							= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )					= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
       	_echoLightSize("Size Of Beam", Range ( 0, 1 ) ) 	= 0.5
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
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma multi_compile SHADOWS_SCREEN SHADOWS_OFF

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

			#define ECHOFIXEDLIGHT

			// Vertex\Frag Shader     =====================
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#include "echologin_LitVertexShader.cginc"


			ENDCG
		}
	}
	//Fallback "echoLogin/Light/Solid/Color"
	Fallback "Diffuse"
}
 
