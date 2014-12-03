//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Solid color shader which also uses vertex color.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoRGBA          - Object color 
//# _echoScale         - Scale Mesh in XYZ, W should always be 1
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/Solid/Color"
{
   	Properties 
	{
 		_echoRGBA ( "Color", Color )			= ( 1, 1, 1, 1 )    
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
		
			float4	  _echoRGBA;
						
			// Vertex/frag Shaders     ===================== 	
			#define ECHODEF_NOTEXTURE
			#define ECHODEF_SOLIDCOLOR _echoRGBA
			#define ECHODEF_NO_TC1
			#define ECHODEF_VERTEXCOLOR
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			//#define ECHODEF_CUSTOMRETURN fcolor.xyz = ( fcolor.xyz + ncolor * fixed3(0.5,0.5,0.5) );

			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}

	//=========================================================================
		Pass 
		{
			Name "ShadowCaster"
	        Tags { "LightMode" = "ShadowCaster" }
	
	        Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
	        Offset 1, 1
	
	        CGPROGRAM
	
	        #pragma vertex vert
	        #pragma fragment frag
	        #pragma multi_compile_shadowcaster
	        #pragma fragmentoption ARB_precision_hint_fastest
	
	        #include "UnityCG.cginc"
	
	        struct v2f 
	        { 
	            V2F_SHADOW_CASTER;
			};
	
			// ============================================= 	
	        v2f vert( appdata_base v )
	        {
	            v2f o;
	            TRANSFER_SHADOW_CASTER(o)
	            return o;
	        }
	
			// ============================================= 	
	        float4 frag( v2f i ) : COLOR
	        {
	            SHADOW_CASTER_FRAGMENT(i)
	        }
	
	        ENDCG
	    }
	    
	    
	//=========================================================================
	    Pass 
	    {
	        Name "ShadowCollector"
	        Tags { "LightMode" = "ShadowCollector" }
	
	        Fog {Mode Off}
	        ZWrite On ZTest LEqual
	
	        CGPROGRAM
	
	        #pragma vertex vert
	        #pragma fragment frag
	        #pragma fragmentoption ARB_precision_hint_fastest
	        #pragma multi_compile_shadowcollector
	
	        #define SHADOW_COLLECTOR_PASS
	
	        #include "UnityCG.cginc"
	
	        struct appdata 
	        {
				float4 vertex : POSITION;
	        };
	
	        struct v2f 
	        {
				V2F_SHADOW_COLLECTOR;
	        };
	
			// ============================================= 	
	        v2f vert (appdata v)
	        {
	            v2f o;
	            TRANSFER_SHADOW_COLLECTOR(o)
	            return o;
	        }
	
			// ============================================= 	
	        fixed4 frag (v2f i) : COLOR
	        {
	            SHADOW_COLLECTOR_FRAGMENT(i)
	        }
	
	        ENDCG
    	}
 	}
}
 
