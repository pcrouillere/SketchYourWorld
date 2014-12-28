//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Textured shader that uses Vertex Color and _echoRGBA coloring.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         		- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoRGBA         		- Vector4 ( r, g, b, a ) 
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/20-Color"
{
	Properties 
	{
		_MainTex ("Texture", 2D)				= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
		_echoRGBA ( "RGB Multiply", Vector )	= ( 1, 1, 1, 1 )    
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
			float4		_echoRGBA;


           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
#ifndef LIGHTMAP_OFF
			  	float4 texcoord1: TEXCOORD1;
#endif
			  	float4 color	: COLOR;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
			  	fixed3 vcolor   : TEXCOORD1;
#ifndef LIGHTMAP_OFF
                half2 tc3		: TEXCOORD3;
#endif
            };

			//=============================================
			Varys vert ( VertInput  ad )
			{
				Varys v;

				v.vcolor		= ad.color.xyz * _echoRGBA.xyz;
    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= ( _MainTex_ST.xy * ad.texcoord.xy ) + _echoUV.xy + _MainTex_ST.zw;

#ifndef LIGHTMAP_OFF
   				v.tc3 	  		= ( unity_LightmapST.xy * ad.texcoord1.xy ) + unity_LightmapST.zw;
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
#ifndef LIGHTMAP_OFF
			  	return fixed4 ( tex2D ( _MainTex, v.tc1 ).xyz * v.vcolor * DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) ), 1.0 );
#else
    			return fixed4 ( tex2D ( _MainTex, v.tc1 ).xyz * v.vcolor, 1.0 );
#endif
			}

			ENDCG
		}
 	}
 }
