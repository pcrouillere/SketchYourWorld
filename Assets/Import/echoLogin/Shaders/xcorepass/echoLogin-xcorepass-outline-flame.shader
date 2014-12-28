//-----------------------------------------------------------------------------
// Outline		- Outline shader used by framework.
//-----------------------------------------------------------------------------
Shader "Hidden/echoLogin-Outline-Flame"
{
   	Properties 
	{
     	_echoGlowColor ("Outline Color", Color )				= ( 1,0.5,0.5,1)
      	_echoGlowScale ("Outline Scale", Range ( 0.0, 0.1 ) )	= 0.01
   		_echoSpeed ("Flare Speed", Range ( 0.0, 64.0 ) )		= 32
   		_echoHeight ("Flare Height", Range ( 0.0, 1.0 ) )		= 0.1
   		_echoAmount ("Flare Amount", Range ( 1.0, 128.0 ) )		= 32
   	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent" }

    	Pass 
		{    
			Name "COREPASS-FLAME"    
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

				dotprod = max ( 0.0, abs ( dot ( EchoObjViewDir ( ad.vertex ), ad.normal ) ) );
				v.rimlit = clamp ( dotprod * 1.2, 0.0, 1.0 );

				ripple 			= EchoRipple ( ad.texcoord, _echoAmount, _echoSpeed, _echoHeight , 0.0, 0.0  );
		
				dotprod = step ( 0.65, 1.0 - dotprod );
				
				ad.vertex.xyz 	=  ad.vertex.xyz + ( float3 ( ripple, ripple, ripple ) * ad.normal * dotprod );
				
				v.pos = mul( UNITY_MATRIX_MV, ad.vertex); 
     			normal = mul( (float3x3)UNITY_MATRIX_IT_MV, ad.normal);  
     			normal.z = 0.01;
     			v.pos = v.pos + float4(normalize(normal),0) * _echoGlowScale;
     			v.pos = mul(UNITY_MATRIX_P, v.pos);

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
 
