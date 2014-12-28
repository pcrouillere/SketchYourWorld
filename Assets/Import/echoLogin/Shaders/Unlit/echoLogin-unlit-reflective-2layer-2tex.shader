//$-----------------------------------------------------------------------------
//@ Reflective shader	- The base texture plus reflection texture.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, u2, v2 ) 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/Reflective/2Layer-2Tex"
{
   	Properties 
	{
 		_MainTex ("Texture Base Alpha = shows layer1 amount", 2D)	= "blue" {}
     	_EnvMap ("Reflection", 2D)	= "blue"  {}
       	_echoUV ("UV Offset u1 v1 u2 v2", Vector )				= ( 0, 0, 0.5, 0.5 )
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
			float4      _MainTex_ST;
			float4 		_MainTex_TexelSize;
			sampler2D	_EnvMap;
			float4		_EnvMap_ST;

#ifndef LIGHTMAP_OFF
			sampler2D   unity_Lightmap;
			float4   	unity_LightmapST;
#endif

         	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
#ifndef LIGHTMAP_OFF
			  	float4 texcoord1: TEXCOORD1;
#endif
			  	float3 normal   : NORMAL;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
                half2 tc2		: TEXCOORD1;
#ifndef LIGHTMAP_OFF
                half2 tc4		: TEXCOORD2;
#endif
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;
				
    			v.pos = mul ( UNITY_MATRIX_MVP, ad.vertex );
    			
 				float3 reflection = reflect ( normalize ( mul ( UNITY_MATRIX_MV , ad.vertex ) ), float3 ( normalize ( mul ( (float3x3)UNITY_MATRIX_MV , ad.normal ) ) ) );
	
				reflection.z += 1.0;
	
				float num = ( sqrt ( reflection.x * reflection.x + reflection.y * reflection.y + reflection.z * reflection.z ) * 2.0 );
 
				v.tc2.x = _EnvMap_ST.xy * ( reflection.x / num ) + _echoUV.z + _EnvMap_ST.z;
				v.tc2.y = _EnvMap_ST.xy * ( reflection.y / num ) + _echoUV.w + _EnvMap_ST.w;

   				v.tc1 	= ( _MainTex_ST.xy * ad.texcoord.xy ) + _MainTex_ST.zw;

#ifndef LIGHTMAP_OFF
   				v.tc4 	= ( unity_LightmapST.xy * ad.texcoord1.xy ) + unity_LightmapST.zw;
#endif
	
#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
				{
					v.tc1.y = 1.0-v.tc1.y;
					v.tc2.y = 1.0-v.tc2.y;
				}
#endif
				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor;
				fixed3 tcolor;
				
				fcolor = tex2D ( _MainTex, v.tc1 );   	// main text
				tcolor = tex2D ( _EnvMap, v.tc2 ).xyz; 	//  layer 1
				
				fcolor.xyz  = lerp ( fcolor.xyz, tcolor, fcolor.w );
//				fcolor.w  	= 1.0;

#ifndef LIGHTMAP_OFF
				fcolor.xyz *= DecodeLightmap ( tex2D ( unity_Lightmap, v.tc4 ) );
#endif
				
    			return fcolor;
			}

			ENDCG
		}
 	}
}
