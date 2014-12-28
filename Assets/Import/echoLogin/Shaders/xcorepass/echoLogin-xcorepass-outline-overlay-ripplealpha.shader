//-----------------------------------------------------------------------------
// Outline		- Outline shader used by framework.
//-----------------------------------------------------------------------------
Shader "Hidden/echoLogin-Outline-Overlay-RippleAlpha"
{
   	Properties 
	{
     	_echoGlowColor ("Outline Color", Color )				= ( 1,1,1,1)
      	_echoGlowScale ("Outline Scale", Range ( 1.01, 1.5 ) )	= 1.1
   		_echoSpeed ("Flare Speed", Range ( 0.0, 1024.0 ) )		= 16
   		_echoHeight ("Flare Height", Range ( 0.0, 1.0 ) )		= 1
   		_echoAmount ("Flare Amount", Range ( 1.0, 16.0 ) )		= 4
   	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Overlay" "IgnoreProjector"="True" }
		
    	Pass 
		{    
			Name "COREPASS-OVERLAYRIPPLEALPHA"    
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
			#include "../Include/EchoLogin-Light.cginc"

			fixed4      _echoGlowColor;
			fixed      	_echoSpeed;
			fixed      	_echoGlowScale;
			fixed       _echoHeight;
			fixed       _echoAmount;
			
           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
           	};

           	struct Varys
            {
                float4 pos		: SV_POSITION;
                fixed2 texuv    : TEXCOORD0;
            };
            
 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys 	v;
		
				v.pos = mul( UNITY_MATRIX_MVP, ad.vertex * float4 ( _echoGlowScale, _echoGlowScale, _echoGlowScale, 1.0 ) ); 
				
				v.texuv = ad.texcoord.xy;
				
				return v;
			}
		
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = _echoGlowColor;
				fixed ripple; 
				
				ripple =  sin ( length ( v.texuv ) * _echoAmount - _echoSpeed * _Time ) * _echoHeight;
				
				fcolor.w *= clamp ( ripple, 0, 1 );
				
				return fcolor;
			}

			ENDCG
		}
	}
}
 
