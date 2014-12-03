//$-----------------------------------------------------------------------------
//@ Transparent Shader	- Textured with vertex color and _echoRGBA color.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//# _echoRGBA         	- Vector4 ( r, g, b, a ) 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Transparent/20-Color"
{
	Properties 
   	{
		_MainTex ("Texture", 2D) 		= "gray" {}
 		_TransparencyLM ("Transparency LM", 2d )	= "black"{}
     	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
 		_echoRGBA ( "RGBA Multiply", Vector )			= ( 1, 1, 1, 1 )    
  	}
      
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Transparent+1" "IgnoreProjector"="False" "RenderType"="Transparent" }

    	Pass 
		{    
			Name "BASE"
      	 	ZWrite Off
      	 	Blend SrcAlpha OneMinusSrcAlpha
      		Cull Back
     	
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
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

			float4		_echoRGBA;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float4 color	: COLOR;
#ifndef LIGHTMAP_OFF
			  	float4 texcoord1: TEXCOORD1;
#endif
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
			  	fixed4 vcolor	: TEXCOORD1;
#ifndef LIGHTMAP_OFF
                half2 tc3		: TEXCOORD3;
#endif
            };

 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;
				
 				v.vcolor		= ad.color * _echoRGBA;
    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex * _echoScale );
  				v.tc1 			= ( _MainTex_ST.xy * ad.texcoord.xy ) + _echoUV.xy + _MainTex_ST.zw;

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
				return v;
			}
 	
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
#ifndef LIGHTMAP_OFF
				fixed4 fcolor = tex2D ( _MainTex, v.tc1 ) * v.vcolor;
				return fixed4 ( fcolor.xyz * DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) ), fcolor.w );
#else
    			return tex2D ( _MainTex, v.tc1 ) * v.vcolor;
#endif
			}

			ENDCG
		}
 	}
}
