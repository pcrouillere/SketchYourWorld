//$-----------------------------------------------------------------------------
//@ Transparent Shader	- Use alpha channel for cutout.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//# _echoCutoff         - Alpha cutoff value
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Transparent/Cutout/12-Discard"
{
	Properties 
   	{
		_MainTex ("Texture", 2D) = "gray" {}
 		_TransparencyLM ("Transparency LM", 2d )	= "white"{}
     	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
		_echoCutoff ("Cutoff Alpha value", Range ( 0, 1.0 ) ) 	= 0.1   
   	}
 
	//=========================================================================
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

    	Pass 
		{    
      	 	ZWrite On
      	 	Blend SrcAlpha OneMinusSrcAlpha
      		Cull Off
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash xbox360
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF

			#include "UnityCG.cginc"
			#include "../Include/EchoLogin.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4		_MainTex_TexelSize;

#ifndef LIGHTMAP_OFF
			sampler2D   unity_Lightmap;
			float4   	unity_LightmapST;
#endif

			fixed		_echoCutoff;

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

 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;

     			v.pos	= mul ( UNITY_MATRIX_MVP, ad.vertex * _echoScale );
 				v.tc1 	= ( _MainTex_ST.xy * ad.texcoord.xy ) + _echoUV.xy + _MainTex_ST.zw;

#ifndef LIGHTMAP_OFF
   				v.tc3 	= ( unity_LightmapST.xy * ad.texcoord1.xy ) + unity_LightmapST.zw;
#endif

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
				return v;
			}
 	
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = tex2D ( _MainTex, v.tc1 );
		
    			fcolor.w = step ( _echoCutoff, fcolor.w );
    			clip ( fcolor.w );
	
#ifndef LIGHTMAP_OFF
				fcolor.xyz *= DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) );
#endif
				return fcolor;
			}

			ENDCG
		}
 	}
}
