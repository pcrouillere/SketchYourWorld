//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Additive lit shader 
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
Shader "echoLogin/Light/Transparent/05-Additive"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)				= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
   	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="False" "RenderType"="Transparent" }

		UsePass "Hidden/echoLogin/COREPASS-ZWRITE"

		Pass 
		{    
			Tags { "LightMode" = "ForwardBase" }
 
 			ZWrite Off
       	 	Blend SrcAlpha One
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

			// Vertex/Frag Shader     ===================== 	
			#define ECHODEF_FIXED4TEX
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}
		
	}
	Fallback "echoLogin/Light/Solid/Color"
}
 
