//$-----------------------------------------------------------------------------
//@ Reflective shader	- Base texture plus reflection from the alpha channel.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ NOTE:  Puts greyscale image on alpha channel for reflection.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoMixRGBA        - used to tint the reflection 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/Reflective/2Layer-1Tex"
{
   	Properties 
	{
 		_MainTex ("Texture Base Alpha is Reflection", 2D)		= "black" {}
       	_echoUV ("UV Offset u1 v1 u2 v2", Vector )				= ( 0, 0, 0.5, 0.5 )
    	_echoMixRGBA ( "Mix RGBA", Color )						= ( 0.5, 0.5, 0.5, 0.5 )
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

			sampler2D	_EnvMap;
			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;

#ifndef LIGHTMAP_OFF
			sampler2D   unity_Lightmap;
			float4   	unity_LightmapST;
#endif

			fixed4      _echoMixRGBA;

         	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
#ifndef LIGHTMAP_OFF
			  	float4 texcoord1		: TEXCOORD1;
#endif
			  	float3 normal   : NORMAL;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
                half2 tc2		: TEXCOORD1;
#ifndef LIGHTMAP_OFF
                half2 tc3		: TEXCOORD3;
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
	 
				v.tc2.x = _MainTex_ST.xy * ( reflection.x / num + _echoUV.z + _MainTex_ST.z );
				v.tc2.y = _MainTex_ST.xy * ( reflection.y / num + _echoUV.w + _MainTex_ST.w );
				
    			v.tc1 	= ( _MainTex_ST.xy * ad.texcoord.xy ) + _echoUV.xy;

#ifndef LIGHTMAP_OFF
   				v.tc3 	= ( unity_LightmapST.xy * ad.texcoord1.xy ) + unity_LightmapST.zw;
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
				fixed4 fcolor1 = tex2D ( _MainTex, v.tc1  );
				fixed3 fcolor2 = tex2D ( _MainTex, v.tc2  ).www;
				
				fcolor1.w  = 1.0;
				fcolor1.xyz  = lerp ( fcolor1.xyz, fcolor2, _echoMixRGBA.xyz );

#ifndef LIGHTMAP_OFF
				fcolor1.xyz *= DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) );
#endif
				
    			return fcolor1;
			}

			ENDCG
		}
 	}
}
