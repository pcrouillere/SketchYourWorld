//-----------------------------------------------------------------------------
// Outline		- Outline shader used by framework.
//-----------------------------------------------------------------------------
Shader "Hidden/echoLogin-Outline-Overlay-Ripple"
{
   	Properties 
	{
     	_echoGlowColor ("Outline Color", Color )				= ( 1,1,1,1)
      	_echoGlowScale ("Outline Scale", Range ( 1.01, 1.5 ) )	= 1.1
   		_echoSpeed ("Flare Speed", Range ( 0.0, 64.0 ) )		= 16
   		_echoHeight ("Flare Height", Range ( 0.0, 1.0 ) )		= 1
   		_echoAmount ("Flare Amount", Range ( 1.0, 128.0 ) )		= 4
   	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Overlay" "IgnoreProjector"="True" }
		
    	Pass 
		{    
			Name "COREPASS-OVERLAYRIPPLE"    
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

			fixed4      _echoGlowColor;
			float       _echoSpeed;
			float      	_echoGlowScale;
			float       _echoHeight;
			float       _echoAmount;
			float       _echoCenterX;
			float       _echoCenterY;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal   : NORMAL;
           	};

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                fixed rimlit    : TEXCOORD2;
            };
            
 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys 	v;
				float 	dotprod;
				float3 	normal;
				float ripple;
				float4 ripadd;

				dotprod = max ( 0.0, abs ( dot ( EchoObjViewDir ( ad.vertex ), ad.normal ) ) );
				v.rimlit = clamp ( dotprod * 1.2, 0.0, 1.0 );

				ripple 			= clamp ( EchoRipple ( ad.texcoord, _echoAmount, _echoSpeed, _echoHeight , 0.0, 0.0  ), 0.0, 128.0 );
		
				dotprod = step ( 0.65, 1.0 - dotprod );
				
				ripadd.xyz 	=  ad.vertex.xyz + ( float3 ( ripple, ripple, ripple ) * ad.normal * dotprod );
				ripadd.w = 1.0;
				
				v.pos = mul( UNITY_MATRIX_MVP, ( ad.vertex + ripadd ) * float4 ( _echoGlowScale, _echoGlowScale, _echoGlowScale, 1.0 ) ); 
				
				return v;
			}
		
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = _echoGlowColor;
				
				fcolor.w *= v.rimlit; 
				//fcolor.w = 1.0;
				
				return fcolor;
			}

			ENDCG
		}
	}
}
 
