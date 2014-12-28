//$-----------------------------------------------------------------------------
//@ Additive shader - Solid Color Rim Lighting shader
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//# _echoUV             - The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _Color          	- Color 
//# _echoHit      		- Amount of rim lighting
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Additive/FX/RimLit-1Color"
{
	Properties 
   	{
      	_echoUV("UV Offset u1 v1", Vector )						= ( 0, 0, 0, 0 )
 	  	_Color ( "Color", Color )								= ( 1, 1, 1, 1 )    
 		_echoMix ( "Rim Light Amount", Range ( 1.0, 0.0 ) )		=	0.5   
       	_echoScale ("Scale XYZ W=1", Vector )					= ( 1.0, 1.0, 1.0, 1.0 )
  	}
   	  
 	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

    	Pass 
		{    
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

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			sampler2D	_BurnTex;
			float4		_BurnTex_ST;
			float       _echoMix;
			fixed4      _Color;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal   : NORMAL;
           	};

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
                fixed rimlit    : TEXCOORD2;
            };
            
 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys 	v;
				float 	dotprod;
				
				dotprod = 1.0 - dot ( ad.normal, EchoObjViewDir ( ad.vertex ) );
				v.rimlit = smoothstep ( _echoMix, 1.0, dotprod );

				v.rimlit        = v.rimlit;
   				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );
    			v.pos 			= mul ( UNITY_MATRIX_MVP, ad.vertex * _echoScale );
       			
				return v;
			}
		
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = _Color;
				
				fcolor.w *= v.rimlit; 
				
				return fcolor;
			}

			ENDCG
		}
 	}
}
