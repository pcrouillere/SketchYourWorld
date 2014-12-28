//$-----------------------------------------------------------------------------
//@ Lighted Shader	- Dissolve to nothing effect.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ NOTE: The alpha channel contains dissolve data. 
//@
//@ Properties/Uniforms
//@
//# _echoUV         - The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale      - Scale Mesh in XYZ, W should always be 1
//# _echoRGBA       - Value to add or subtract from material rgb ( -2 to +2.0 ) 
//# _echoMix        - Dissolve amount 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/Transparent/30-Dissolve"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)				= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
		_echoRGBA ( "RGB Add", Vector )			= ( 0, 0, 0, 0 )    
    	_echoMix("Mix", Range ( -0.1, 1.0 ) )	= 0
   	}

	//=========================================================================
	SubShader 
	{
 		Tags {"Queue"="AlphaTest" "IgnoreProjector"="False" "RenderType"="TransparentCutout"}

    	Pass 
		{    
			Name "BASE"
	    	Tags { "LightMode" = "ForwardBase" } 

       	 	Blend SrcAlpha OneMinusSrcAlpha
     		Cull Back
     		ZWrite Off
      		
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
			fixed4		_echoRGBA;
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
 	}

	Fallback "echoLogin/Light/Solid/Color"

}
