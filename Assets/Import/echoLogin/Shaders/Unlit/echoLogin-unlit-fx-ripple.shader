//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Ripple effect in a flag or cloth in the wind. 
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoSpeed          - Speed of ripple
//# _echoHeight         - Height/size of each ripple
//# _echoAmount         - Amount of ripples
//# _echoCenterX        - X/U position in texture to start ripple ( 0 - 1 )
//# _echoCenterY        - Y/V position in texture to start ripple ( 0 - 1 )
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/FX/Ripple"
{
	Properties 
	{
		_MainTex ("Texture", 2D)								= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )						= ( 0, 0, 0, 0 )
		_echoRGBA ( "RGB Multiply", Vector )	= ( 1, 1, 1, 1 )    
   		_echoSpeed ("Wave Speed", Range ( -256.0, 256.0 ) )		= 16
   		_echoHeight ("Wave Height", Range ( 0.0, 16.0 ) )		= 1
   		_echoAmount ("Wave Amount", Range ( 1.0, 64.0 ) )		= 4
   		_echoCenterX ("Wave Center X", Range ( -2.0, 2.0 ) )	= 0.0
   		_echoCenterY ("Wave Center Y", Range ( -2.0, 2.0 ) )	= 0.0
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

			float       _echoSpeed;
			float       _echoHeight;
			float       _echoAmount;
			float       _echoCenterX;
			float       _echoCenterY;
			fixed4      _echoRGBA;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float4 color	: COLOR;
			  	float3 normal   : NORMAL;
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
				
				if ( _echoHeight != 0.0 )
				{
					float ripple;
					ripple 			= EchoRipple ( ad.texcoord, _echoAmount, _echoSpeed, _echoHeight * ( ad.color.w *.05 ), _echoCenterX, _echoCenterY  );
					ad.vertex.xyz 	=  ad.vertex.xyz + ( half3(ripple,ripple,ripple) * ad.normal );
				}
			
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
				fixed3 fcolor = tex2D ( _MainTex, v.tc1 ).xyz * _echoRGBA;
#ifndef LIGHTMAP_OFF
			  	fcolor *= DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) );
#endif
    			return fixed4 ( fcolor , 1 );
			}

			ENDCG
		}
 	}
 }
