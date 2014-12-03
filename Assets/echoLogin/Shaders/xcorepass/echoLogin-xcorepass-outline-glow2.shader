//-----------------------------------------------------------------------------
// Outline		- Outline shader used by framework.
//-----------------------------------------------------------------------------
Shader "Hidden/echoLogin-Outline-Glow1"
{
   	Properties 
	{
     	_echoGlowColor ("Outline Color", Color )				= ( 1,1,0,1)
       	_echoGlowScale ("Outline Scale", Range ( 0.0, 0.1 ) )	= 0.05
      	_echoGlowStrength ("Glow Speed", Range ( 1.0, 32.0 ) )	= 8
   	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent" }
		
		//=========================================================================
		// OUTLINE GLOW PASS
		//=========================================================================
    	Pass 
		{
			Name "COREPASS-GLOW2"    
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

				v.pos = mul( UNITY_MATRIX_MV, ad.vertex); 
  	 			float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, ad.normal); 
  	 			normal.z = 0.01; 
     			v.pos = v.pos + float4(normalize(normal),0) * _echoGlowScale;
     			v.pos = mul(UNITY_MATRIX_P, v.pos);
     			
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
 
