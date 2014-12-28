//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Three layers from one texture.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV                 - The UV offsets of layers, Vector4 ( u1, v1, u2, v2 ) 
//# _echoV2Start        	- Position of the V offset where 2nd layer starts ( 0-1 )
//# _echoV3Start        	- Position of the V offset where 3nd layer starts ( 0-1 )
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/Layers/3Layer-1Tex"
{
	Properties 
	{
    	_MainTex ("Texture Background", 2D)				= "blue" {} 
      	_echoUV("UV Offset u2 v2 u3 v3", Vector )		= ( 0, 0, 0, 0 )
    	_echoV2Start("Layer 2 V Start", Float )			= 0.25
    	_echoV3Start("Layer 3 V Start", Float )			= 0.50
   	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry" "IgnoreProjector"="False" "RenderType"="echoUnlit" }

    	Pass 
		{    
      		Cull Back
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF

			#include "UnityCG.cginc"
			#include "../Include/EchoLogin.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;

#ifndef LIGHTMAP_OFF
			sampler2D   unity_Lightmap;
			float4   	unity_LightmapST;
#endif

			float       _echoV2Start;
			float       _echoV3Start;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
#ifndef LIGHTMAP_OFF
			  	float4 texcoord1: TEXCOORD1;
#endif
            };

           	struct Varys
            {
            	half4 pos		: SV_POSITION;
                half2 tc0		: TEXCOORD0;
                half2 tc1		: TEXCOORD1;
                half2 tc2		: TEXCOORD2;
#ifndef LIGHTMAP_OFF
                half2 tc3		: TEXCOORD3;
#endif
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;

    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc0 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _MainTex_ST.zw );
   				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy );
   				v.tc2 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.zw );
     			
     			v.tc0.y -= _echoV3Start;
     			v.tc1.y -= _echoV2Start;

#ifndef LIGHTMAP_OFF
   				v.tc3 	  		= ( unity_LightmapST.xy * ad.texcoord1.xy ) + unity_LightmapST.zw;
#endif

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
				{
					v.tc0.y = 1.0-v.tc0.y;
					v.tc1.y = 1.0-v.tc1.y;
					v.tc2.y = 1.0-v.tc2.y;
				}
#endif
    			return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor1 = tex2D ( _MainTex, v.tc2 );
				fixed4 fcolor2 = tex2D ( _MainTex, v.tc1 );
				fcolor1 =  lerp ( tex2D ( _MainTex, v.tc0 ), lerp ( fcolor2, fcolor1, fcolor1.w ), max ( fcolor1.w , fcolor2.w ) );
			
#ifndef LIGHTMAP_OFF
			  	fcolor1.xyz *= DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) );
#endif

				return ( fcolor1 );
			}

			ENDCG
		}
 	}
}
