//$-----------------------------------------------------------------------------
//@ Reflective shader	- The fastest reflective shader of this group.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/Reflective/00-fastest"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)				= "black" 
       	_echoUV ("UV Offset u1 v1", Vector )		= ( 0.5, 0.5, 0, 0 )
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

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4		_MainTex_TexelSize;

#ifndef LIGHTMAP_OFF
			sampler2D   unity_Lightmap;
			float4   	unity_LightmapST;
#endif

			#include "UnityCG.cginc"
			#include "../Include/EchoLogin.cginc"

         	struct VertInput
            {
                float4 vertex			: POSITION;
                float2 texcoord			: TEXCOORD0;
#ifndef LIGHTMAP_OFF
			  	float4 texcoord1		: TEXCOORD1;
#endif
			  	float3 normal			: NORMAL;
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
			Varys vert ( VertInput ad )
			{
				Varys v;
				
    			v.pos = mul ( UNITY_MATRIX_MVP, ad.vertex );
    			
 				float3 reflection = reflect ( normalize ( mul ( UNITY_MATRIX_MV , ad.vertex ) ), float3 ( normalize ( mul ( (float3x3)UNITY_MATRIX_MV , ad.normal ) ) ) );
	
				reflection.z += 1.0;
	
				float num = ( sqrt ( reflection.x * reflection.x + reflection.y * reflection.y + reflection.z * reflection.z ) * 2.0 );

				v.tc1.x = _MainTex_ST.xy * ( reflection.x / num ) + _echoUV.x + _MainTex_ST.zw;
				v.tc1.y = _MainTex_ST.xy * ( reflection.y / num ) + _echoUV.y + _MainTex_ST.zw ;

#ifndef LIGHTMAP_OFF
   				v.tc3 	= ( unity_LightmapST.xy * ad.texcoord1.xy ) + unity_LightmapST.zw;
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
				return fixed4 ( tex2D ( _MainTex, v.tc1 ).xyz * DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) ), 1.0 );
#else
    			return tex2D ( _MainTex, v.tc1 );
#endif
			}

			ENDCG
		}
 	}
}
