//$-----------------------------------------------------------------------------
//@ Additive shader - The shield effect that can show up to 4 hits at one time.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//# _echoUV             - The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoHitVector0   	- Hit 1 directional vector
//# _echoHitVector1   	- Hit 2 directional vector
//# _echoHitVector2   	- Hit 3 directional vector
//# _echoHitVector3   	- Hit 4 directional vector
//# _echoHitMix0      	- Amount 1 of shield effect to apply
//# _echoHitMix1      	- Amount 2 of shield effect to apply
//# _echoHitMix2      	- Amount 3 of shield effect to apply
//# _echoHitMix3      	- Amount 4 of shield effect to apply
//# _echoHitColor       - Color of the shield hit
//# _echoAlpha          - Transparency of shield (should be zero)
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Additive/FX/Shield-Detail"
{
	Properties 
   	{
    	_MainTex ("Texture", 2D )								= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )						= ( 0, 0, 0, 0 )
    	_echoHitVector0 ("Hit Vector 0", Vector ) 				= ( 0,0,0,0 )
    	_echoHitVector1 ("Hit Vector 1", Vector ) 				= ( 0,0,0,0 )
    	_echoHitVector2 ("Hit Vector 2", Vector ) 				= ( 0,0,0,0 )
    	_echoHitVector3 ("Hit Vector 3", Vector ) 				= ( 0,0,0,0 )
    	_echoHitMix0("Hit Mix Center", Range ( 0.0, 1.0 ) )	= 0
    	_echoHitMix1("Hit Mix Center", Range (  0.0, 1.0  ) )	= 0
    	_echoHitMix2("Hit Mix Center", Range (  0.0, 1.0  ) )	= 0
    	_echoHitMix3("Hit Mix Center", Range (  0.0, 1.0  ) )	= 0
    	_echoHitColor ( "Hit Center Color", Color )				= ( 0.1, 0.4, 1.0, 1 )
  		_echoAlpha ( "Alpha", Range ( 0.0, 2.0 ) )				=	0.0   
   	}
   	  
 	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

    	Pass 
		{    
      	 	ZWrite Off
      	 	Blend SrcAlpha One
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
			float4      _echoHitVector0;
			float4      _echoHitVector1;
			float4      _echoHitVector2;
			float4      _echoHitVector3;
			float       _echoHitMix0;
			float       _echoHitMix1;
			float       _echoHitMix2;
			float       _echoHitMix3;
			fixed4		_echoHitColor;
			float		_echoAlpha;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal   : NORMAL;
           	};

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
                fixed mixper    : TEXCOORD1;
                fixed rimlit    : TEXCOORD2;
            };
            
 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys 	v;
				float 	dotprod;
				float 	mixper;
				float3 	normalDir 	= normalize ( mul ( ad.normal, (float3x3)_World2Object ) );
				
				mixper = 0.0;
				v.rimlit = 0.0;

				if ( _echoAlpha > 0.0 )
				{
					dotprod = abs ( dot ( EchoObjViewDir ( ad.vertex ), ad.normal ) );

					v.rimlit = ( 1.0 - clamp ( dotprod * 4.0, 0.0, 1.0 ) ) + _echoAlpha;
						
					mixper += EchoShieldHit ( _echoHitVector0, _echoHitMix0, normalDir );
					mixper += EchoShieldHit ( _echoHitVector1, _echoHitMix1, normalDir );
					mixper += EchoShieldHit ( _echoHitVector2, _echoHitMix2, normalDir );
					mixper += EchoShieldHit ( _echoHitVector3, _echoHitMix3, normalDir );
				}

				v.mixper 		= clamp ( mixper, 0.0, 1.0 );
   				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );
    			v.pos 			= mul ( UNITY_MATRIX_MVP, ad.vertex );
       			
				return v;
			}
		
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor;
				
				fcolor = tex2D ( _MainTex, v.tc1 );
				fcolor.w = ( v.rimlit + v.mixper ); 
				fcolor = lerp ( fcolor, _echoHitColor, v.mixper );
				
				return fcolor;
			}

			ENDCG
		}
 	}
}
