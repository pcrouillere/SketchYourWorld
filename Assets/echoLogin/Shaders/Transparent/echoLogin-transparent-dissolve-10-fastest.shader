//$-----------------------------------------------------------------------------
//@ Transparent Shader	- Dissolve to nothing effect.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - YES
//@
//@ NOTE: The alpha channel contains dissolve data. 
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- Sets the UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//# _echoRGBA       	- Value to add or subtract from material rgb ( -2 to +2.0 ) 
//# _echoAlpha          - Global alpha value
//# _echoMix            - Dissolve amount 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Transparent/Dissolve/10-Fastest"
{
	Properties 
   	{
		_MainTex ("Texture", 2D) = "gray" {}
 		_TransparencyLM ("Transparency LM", 2d )	= "black"{}
     	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
 		_echoRGBA ( "RGB Add", Vector )			= ( 1, 1, 1, 1 )    
     	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
		_echoAlpha ( "Alpha", Range ( 0, 1 ) )	= 1.0    
    	_echoMix("Mix", Range ( -0.3, 1.3 ) )	= 0
   	}
   
	//=========================================================================
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

    	Pass 
		{    
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

			fixed		_echoAlpha;
			fixed       _echoMix;
			fixed4		_echoRGBA;

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
     			fixed4 fcolor 	= tex2D ( _MainTex, v.tc1 );

				fcolor.xyz 		= lerp ( fcolor.xyz + _echoRGBA.xyz, fixed3( 0.0, 0.0, 0.0 ), step ( fcolor.w, _echoMix ) );
				fcolor.w 		= _echoAlpha;

#ifndef LIGHTMAP_OFF
				fcolor.xyz *= DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) );
#endif

    			return fcolor;
			}

			ENDCG
		}
 	}
}
