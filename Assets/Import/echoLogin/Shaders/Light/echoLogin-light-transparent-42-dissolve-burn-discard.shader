//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Dissolve to nothing with an edge burn effect.
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
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//# _echoRGBA       	- Value to add or subtract from material rgb ( -2 to +2.0 ) 
//# _echoBurnSize       - Size of edge burn
//# _echoBurnColor      - Color of burn
//# _echoMix            - Dissolve amount 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/Transparent/42-Dissolve-Burn-Discard"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)						= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
 		_echoRGBA ( "RGB Add", Vector )					= ( 0, 0, 0, 0 )    
    	_echoBurnSize("BurnSize", Range ( 0.0, 0.1 ) )	= 0.05
    	_echoBurnColor ( "BurnColor", Color )			= ( 0.8, 0.4, 0.0, 1 )
    	_echoMix("Mix", Range ( -0.3, 1.3 ) )			= 0
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
      		
 			CGPROGRAM
			
			#define DIRECTIONAL
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash xbox360
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
			fixed		_echoMix;
			fixed4		_echoRGBA;
			fixed		_echoBurnSize;
			fixed4		_echoBurnColor;

			// Vertex\Frag Shader     ===================== 	
			#define ECHODEF_FIXED4TEX
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_CUSTOMCODE clip ( fcolor.w - _echoMix +_echoBurnSize );
			#define ECHODEF_CUSTOMRETURN fixed tcolor = fcolor.w; fcolor = lerp ( fixed4 ( fcolor.xyz * lcolor ECHO_DOUBLELIGHT3 + _echoRGBA.xyz, tcolor ), _echoBurnColor, step ( fcolor.w, _echoMix ) );fcolor.w = step ( _echoMix -_echoBurnSize, tcolor );return fcolor;
			#include "echologin_LitVertexShader.cginc"
 	
			ENDCG
		}
 	}

	Fallback "echoLogin/Light/Solid/Color"
}
