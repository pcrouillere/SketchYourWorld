//$-----------------------------------------------------------------------------
//@ Background Shader 	- 2 Layer background with clouds on alpha of MainTex
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//# _echoUV             - The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoMix            - Amount of Clouds to show
//# _echoMix2           - Amount of Layer1Tex texture to show
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Background/Layers/Clouds-2Layer-2Tex"
{
	Properties 
	{
    	_MainTex ("Texture", 2D)						= "black" {} 
   	   	_Layer1Tex ("Texture Overlay", 2D)				= "black" {} 
    	_echoUV("UV Offset u1 v1 u2 v2", Vector )		= ( 0, 0, 0, 0 )
      	_echoMix("Cloud Mix", Range ( 0, 1 ) ) 			= 0.5
      	_echoMix2 ("Layer Mix", Range ( 0, 1 ) ) 		= 0.5
   	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry+440" "IgnoreProjector"="True" "RenderType"="Background" }

    	Pass 
		{    
			ZWrite Off
      		Cull Back
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			sampler2D	_Layer1Tex;
			float4		_Layer1Tex_ST;
			float4		_echoUV;   
			fixed       _echoMix;
			fixed       _echoMix2;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;

    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = tex2D ( _MainTex, v.tc1 );
				fixed3 lcolor = tex2D ( _Layer1Tex, v.tc1 );
				
				fcolor.xyz = lerp ( lerp ( fcolor.xyz, lcolor, _echoMix2 ), fcolor.www, _echoMix );
				fcolor.w = 1.0;
				
    			return fcolor;
			}

			ENDCG
		}
 	}
}
