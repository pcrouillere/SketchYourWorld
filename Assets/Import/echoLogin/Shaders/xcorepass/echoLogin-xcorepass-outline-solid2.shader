//-----------------------------------------------------------------------------
// Outline		- Outline shader used by framework.
//-----------------------------------------------------------------------------
Shader "Hidden/echoLogin-Outline-Solid"
{
   	Properties 
	{
     	_echoGlowColor ("Outline Color", Color )				= ( 1,1,1,1)
      	_echoGlowScale ("Outline Scale", Range ( 0.0, 0.1 ) )	= 0.05
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
			Name "COREPASS-OUTLINESOLID2"    
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

				v.pos = mul ( UNITY_MATRIX_MV, ad.vertex); 
  	 			float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, ad.normal ); 
  	 			normal.z = 0.01; 
     			v.pos = v.pos + float4 ( normalize (normal) ,0 ) * _echoGlowScale;
     			v.pos = mul ( UNITY_MATRIX_P, v.pos );
    			     			
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
 
