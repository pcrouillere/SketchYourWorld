//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit GUI Shader	
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         		- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _Color         			- Color  
//&-----------------------------------------------------------------------------
Shader "echoLogin/GUI/Unlit"
{
	Properties 
	{
		_MainTex ("Texture", 2D)				= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
		_Color ( "RGB Multiply", Vector )		= ( 1, 1, 1, 1 )    
     	_echoScale ("Scale XYZ", Vector )			= ( 1.0, 1.0, 1.0, 1.0 )
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

			#include "UnityCG.cginc"
			#include "../Include/EchoLogin.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;

			float4		_Color;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float4 color	: COLOR;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
			  	fixed3 vcolor   : TEXCOORD1;
            };

			//=============================================
			Varys vert ( VertInput  ad )
			{
				Varys v;

				v.vcolor		= ad.color.xyz * _Color.xyz;
     			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex * _echoScale );
				v.tc1 	  		= ( _MainTex_ST.xy * ad.texcoord.xy ) + _echoUV.xy + _MainTex_ST.zw;

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
				return v;
			}
 	
			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
    			return fixed4 ( tex2D ( _MainTex, v.tc1 ).xyz * v.vcolor, 1.0 );
			}

			ENDCG
		}
 	}
 }
