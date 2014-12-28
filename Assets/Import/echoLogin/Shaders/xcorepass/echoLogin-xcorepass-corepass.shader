//-----------------------------------------------------------------------------
// NOT FOR USER USE - go away !! 
//-----------------------------------------------------------------------------
Shader "Hidden/echoLogin"
{
	//=========================================================================
	SubShader 
	{
		//=========================================================================
		// write to zbuffer pass for non solid zwrite shaders
		//=========================================================================
    	Pass 
    	{
			Name "COREPASS-ZWRITE"    
        	ZWrite On
        	ColorMask 0
    	}

		//=========================================================================
		// OUTLINE SPHERE GLOW PASS
		//=========================================================================
    	Pass 
		{
			Name "COREPASS-SPHEREGLOW"    
     	 	ZWrite Off
      	 	Blend SrcAlpha One
      		Cull Off
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest

			float      	_echoGlowScale;
			fixed4      _echoGlowColor;
			float       _echoGlowStrength;
			
			#include "UnityCG.cginc"
			#include "../Include/EchoLogin.cginc"
			#include "../Include/EchoLogin-Light.cginc"

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

				v.rimlit = clamp ( -dot ( EchoObjViewDir ( ad.vertex ), ad.normal ) * _echoGlowStrength, 0.0, 1.0 );
				
    			v.pos	= mul ( UNITY_MATRIX_MVP, ad.vertex * float4 ( _echoGlowScale,_echoGlowScale,_echoGlowScale,1 ) );
    			v.pos.z += 0.01;
     			
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