//$-----------------------------------------------------------------------------
//@ Transparent Shader		- Solid color transparent shader that uses vertex color.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//# _Color             - Object color 
//# _echoScale             - Scale mesh in shader
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Transparent/Solid/20-Color"
{
	Properties 
   	{
      	_Color ( "Color", Color )					= ( 1, 1, 1, 1 )    
		_TransparencyLM ("Transparency LM", 2d )		= "black"{}
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
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
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
			#include "../Include/EchoLogin.cginc"

			sampler2D	_MainTex;
			float4      _Color;

          	struct VertInput
            {
                float4 vertex	: POSITION;
			  	float4 color	: COLOR;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
 			  	fixed4 vcolor   : TEXCOORD0;
           	};

 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;
				
     			v.pos		= mul ( UNITY_MATRIX_MVP, ad.vertex * _echoScale );
				v.vcolor	= ad.color * _Color;

				return v;
			}
 	
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
    			return v.vcolor;
			}

			ENDCG
		}
 	}
}
