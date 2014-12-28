//-----------------------------------------------------------------------------
// Outline		- Outline shader used by framework.
//-----------------------------------------------------------------------------
Shader "Hidden/echoLogin-Outline-Overlay-Rimlit"
{
   	Properties 
	{
     	_echoGlowColor ("Rim Light Color", Color )						= ( 1, 0, 0, 1 )
      	_echoGlowStrength ("Rim Light Strength", Range ( 1.0, 0.0 ) )	= 0.2
   	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Overlay" "IgnoreProjector"="True" }
		
		//=========================================================================
		// RIMLIT OVERLAY 
		//=========================================================================
    	Pass 
		{
			Name "COREPASS-RIMLIT" 
			ZTest Always   
       	 	ZWrite Off
      	 	Blend SrcAlpha OneMinusSrcAlpha
      		Cull Back
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
			#include "../Include/EchoLogin.cginc"
			#include "../Include/EchoLogin-Light.cginc"

			float      	_echoGlowScale;
			float       _echoGlowStrength;
			fixed4      _echoGlowColor;

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
				
				dotprod = 1.0 - dot ( ad.normal, EchoObjViewDir ( ad.vertex ) );
				v.rimlit = smoothstep ( _echoGlowStrength, 1.0, dotprod );

    			v.pos 			= mul ( UNITY_MATRIX_MVP, ad.vertex * float4 ( 1.005, 1.005, 1.005, 1.0 ) );
				v.pos.z -=0.0001;
        			        			
				return v;
			}
		
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = _echoGlowColor;
				
				fcolor.w *= v.rimlit; 
				
				return fcolor;
			}

			ENDCG

		}
	}
}
 
