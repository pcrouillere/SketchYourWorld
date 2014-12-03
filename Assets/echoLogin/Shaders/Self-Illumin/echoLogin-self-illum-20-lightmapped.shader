//$-----------------------------------------------------------------------------
//@ Sefl Illumin - lightmap	- This will show lightmapping on non lit areas
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	    - The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _EmissionLM             - Strength of light
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Self-Illumin/20-lightmapped"
{
	Properties 
	{
		_MainTex ("Texture", 2D)					= "black" {} 
		_Illum ("Light ( A )", 2D)					= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )			= ( 0, 0, 0, 0 )
    	_EmissionLM ("Emission", Float )			= 16
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

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;

#ifndef LIGHTMAP_OFF
			sampler2D   unity_Lightmap;
			float4   	unity_LightmapST;
#endif

			float4      _echoUV;

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
                half2 tc1		: TEXCOORD0;
#ifndef LIGHTMAP_OFF
                half2 tc3		: TEXCOORD3;
#endif
            };

			// ============================================= 	
			Varys vert ( VertInput  ad )
			{
				Varys v;

    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );
 
#ifndef LIGHTMAP_OFF
   				v.tc3 	  		= ( unity_LightmapST.xy * ad.texcoord1.xy ) + unity_LightmapST.zw;
#endif

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
#ifndef LIGHTMAP_OFF
				fixed3 lmap 	= DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) );
				fixed4 fcolor 	= tex2D ( _MainTex, v.tc1 );
				
				return fixed4 ( lerp ( fcolor.xyz * lmap, fcolor.xyz * fixed3(2,2,2), fcolor.www ), 1.0 );
#else
    			return tex2D ( _MainTex, v.tc1 );
#endif
			}

			ENDCG
		}
 	}
 }
