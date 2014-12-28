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
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/Relfective/10-Fastest"
{
   	Properties 
	{
    	_EnvMap ("Texture", 2D)				= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
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
					
			sampler2D 	_EnvMap;
			float4		_EnvMap_ST;
			float4 		_EnvMap_TexelSize;

 			// Vertex\Frag Shader     =====================
			#define ECHODEF_REFLECTION 
			#define ECHODEF_REFLECTION_STXY _EnvMap_ST.xy
			#define ECHODEF_REFLECTION_STZW _EnvMap_ST.zw
			#define ECHODEF_NO_TC1
			#define ECHODEF_NOTEXTURE
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#define ECHODEF_CUSTOMCODE  fixed3 fcolor = tex2D ( _EnvMap, v.tc2.xy ).xyz;
			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}
		
	}
	Fallback "echoLogin/Light/Solid/Color"
}
 
