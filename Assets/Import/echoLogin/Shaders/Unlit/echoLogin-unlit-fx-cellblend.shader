//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Fades texture back and forth from different UV positions.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         		- The UV offset of texture Vector4 ( u1, v1, u2, v2 ) 
//# _echoMix         	    - Mix from UV1 to UV2
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/FX/CellBlend"
{
	Properties 
	{
		_MainTex ("Texture", 2D)						= "black" {} 
      	_echoUV("UV Offset u1 v1 u2 v2", Vector )		= ( 0, 0, 0, 0 )
    	_echoMix( "Mix", Range ( 0.0, 1.0 ) )			= 0.0
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

#ifndef LIGHTMAP_OFF
			sampler2D   unity_Lightmap;
			float4   	unity_LightmapST;
#endif

			float4		_echoUV;
			fixed      _echoMix;

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
                half2 tc2		: TEXCOORD1;
#ifndef LIGHTMAP_OFF
                half2 tc3		: TEXCOORD4;
#endif
            };

			// ============================================= 	
			Varys vert ( VertInput  ad )
			{
				Varys v;
			
    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= ( _MainTex_ST.xy * ad.texcoord.xy ) + _echoUV.xy + _MainTex_ST.zw;
   				v.tc2 	  		= ( _MainTex_ST.xy * ad.texcoord.xy ) + _echoUV.zw;
   				
#ifndef LIGHTMAP_OFF
   				v.tc3 	  		= ( unity_LightmapST.xy * ad.texcoord1.xy ) + unity_LightmapST.zw;
#endif

				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor;
				
				fcolor.w   = 1.0;
				fcolor.xyz = lerp ( tex2D ( _MainTex, v.tc1 ).xyz, tex2D ( _MainTex, v.tc2 ).xyz, _echoMix );

#ifndef LIGHTMAP_OFF
			  	fcolor.xyz *= DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) );
#endif

    			return fcolor;
			}

			ENDCG
		}
 	}
}
