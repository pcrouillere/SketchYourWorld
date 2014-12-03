//-----------------------------------------------------------------------------
// Outline		- Outline shader used by framework.
//-----------------------------------------------------------------------------
Shader "Hidden/echoLogin-Outline-Overlay-Glow"
{
   	Properties 
	{
     	_echoGlowColor ("Glow Color", Color )						= ( 0, 1, 1, 1 )
      	_echoGlowStrength ("Glow Strength", Range ( 0.0, 8.0 ) )	= 1
      	_echoGlowScale ("Glow Scale", Range ( 1.01, 2.0 ) ) 		= 1.1
   	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Overlay" "IgnoreProjector"="True" }
		
		//=========================================================================
		// OUTLINE OVERLAY
		//=========================================================================
    	Pass 
		{
			Name "COREPASS-GLOWOVERLAY"    
     	 	ZWrite Off
      	 	Blend SrcAlpha One
      		Cull Off
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
			#include "../Include/EchoLogin.cginc"
			#include "../Include/EchoLogin-Light.cginc"

			float      	_echoGlowScale;
			fixed4      _echoGlowColor;
			float       _echoGlowStrength;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal   : NORMAL;
           	};

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                fixed alpha     : TEXCOORD2;
            };
                        
  			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys 	v;
				float 	dotprod;

				v.alpha = ( sin ( _Time.z * _echoGlowStrength ) * 0.5 ) + 0.5;

				v.pos = mul( UNITY_MATRIX_MVP, ad.vertex * float4 ( _echoGlowScale, _echoGlowScale, _echoGlowScale, 1.0 ) ); 
     			
  				return v;
			}
		
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = _echoGlowColor;
				
				fcolor.w = v.alpha*_echoGlowColor.w; 
				
				return fcolor;
			}

			ENDCG
		}
	}
}
 
