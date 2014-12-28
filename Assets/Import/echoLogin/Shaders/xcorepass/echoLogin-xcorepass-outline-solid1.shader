//-----------------------------------------------------------------------------
// Outline		- Outline shader used by framework.
//-----------------------------------------------------------------------------
Shader "Hidden/echoLogin-Outline-Solid"
{
   	Properties 
	{
     	_echoGlowColor ("Outline Color", Color )				= ( 1,1,1,1)
      	_echoGlowScale ("Outline Scale", Range ( 1.01, 1.5 ) )	= 1.1
   	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Geometry-1" "IgnoreProjector"="True" "RenderType"="Transparent" }
		
		//=========================================================================
		// OUTLINE SOLID
		//=========================================================================
    	Pass 
		{
			Name "COREPASS-OUTLINESOLID1"    
     	 	ZWrite Off
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

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal   : NORMAL;
           	};

           	struct Varys
            {
                half4 pos		: SV_POSITION;
            };

  			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys 	v;

				v.pos = mul( UNITY_MATRIX_MVP, ad.vertex * float4 ( _echoGlowScale, _echoGlowScale, _echoGlowScale, 1.0 ) ); 
				//v.pos.z += 0.01;
   			     			
  				return v;
			}
		
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				return _echoGlowColor;
			}

			ENDCG
		}
	}
}
 
