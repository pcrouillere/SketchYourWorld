//$-----------------------------------------------------------------------------
//@ Lighted Shader		- The fastest textured shader of this group.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scales Shader/Mesh 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/41-control-terrain-2tex"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)				= "black" {} 
    	_DetailTex ("Detail Tex", 2D)				= "black" {} 
    	_ControlTex ("Control", 2D)				= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
      	_echoSplatColor1 ("Splat Color 1", Color ) = (  1.0, 1.0, 1.0, 1.0 )
      	_echoSplatColor2 ("Splat Color 2", Color ) = (  1.0, 1.0, 1.0, 1.0 )
      	_echoSplatColor3 ("Splat Color 3", Color ) = (  1.0, 1.0, 1.0, 1.0 )
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
			
			sampler2D 	_DetailTex;
			float4		_DetailTex_ST;

			sampler2D 	_ControlTex;

			fixed4      _echoSplatColor1;
			fixed4      _echoSplatColor2;
			fixed4      _echoSplatColor3;
						
			// Vertex\Frag Shader     =====================
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_SPLATCONTROL
			#define ECHODEF_SPLATDETAIL
			//#define ECHODEF_FIXED4TEX
			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}	
	}
	
	Fallback "echoLogin/Light/Solid/Color"
}
 
