//$-----------------------------------------------------------------------------
//@ Transparent Shader	- Solid color transparent shader.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//# _Color             	- Object color 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Transparent/Solid/10-Fastest"
{
	Properties 
   	{
      	_Color ( "Color", Color )				= ( 1, 1, 1, 1 )    
		_TransparencyLM ("Transparency LM", 2d )	= "black"{}
       	_echoScale ("Scale XYZ", Vector )			= ( 1.0, 1.0, 1.0, 1.0 )
   	}

	//=========================================================================
	SubShader 
	{
 		Tags { "Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent" }

    	Pass 
		{    
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

			sampler2D	_MainTex;
			fixed4       _Color;

          	struct VertInput
            {
                float4 vertex	: POSITION;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
           	};

 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;
				
     			v.pos	= mul ( UNITY_MATRIX_MVP, ad.vertex * _echoScale );

				return v;
			}
 	
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
    			return _Color;
			}

			ENDCG
		}
 	}
}
