//$-----------------------------------------------------------------------------
//@ Additive shader - The fusion effect glows at the center facing the camera.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//# _echoRGBA         	- Vector4 ( r, g, b, a ) 
//# _echoHitMix0       	- Size of the fusion effect 0=none 2.0=full 
//# _echoHitColor       - Color at center  
//# _echoMidColor       - Color between Hit and Edge 
//# _echoEdgeColor      - Color of Edge  
//# _echoScale          - Scales Shader/Mesh 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Additive/FX/Fusion"
{
	Properties 
   	{
 		_echoRGBA ( "RGB Multiply", Vector )					= ( 1, 1, 1, 1 )    
    	_echoHitMix0("Hit Mix Center", Range ( 0.0, 2.0 ) )		= 0
	   	_echoHitColor ( "Center Color", Color )					= ( 1.0, 1.0, 1.0, 1.0 )
    	_echoMidColor ( "Mid Color", Color )					= ( 0.0, 0.2, 1.0, 1.0 )
    	_echoEdgeColor ( "Edge Color", Color )					= ( 1.0, 1.0, 0.0, 1.0 )
       	_echoScale ("Scale XYZ", Vector )						= ( 1.0, 1.0, 1.0, 1.0 )
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

			float       _echoHitMix0;
			float4		_echoHitColor;
			float4		_echoMidColor;
			float4		_echoEdgeColor;
			float4      _echoRGBA;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal    : NORMAL;
           	};

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
                fixed4 dcolor   : TEXCOORD1;
            };
            
 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys 	v;
				float 	dotprod;
				float 	mixper;
				float4  dcolor;
				
				dotprod = dot ( EchoObjViewDir ( ad.vertex ), ad.normal );

				mixper = ( _echoHitMix0 * dotprod );

				if ( mixper <= 0.2 )
				{
					dcolor = lerp ( float4 ( 0.0, 0.0, 0.0, 0.0 ), _echoEdgeColor, mixper * 5.0 );
				}
				else if ( mixper <= 0.4 )
				{
					dcolor = lerp ( _echoEdgeColor, _echoMidColor, ( mixper - 0.2 ) * 5.0 );
				}
				else 
				{
					dcolor = lerp ( _echoMidColor, _echoHitColor, ( mixper - 0.4 ) * ( 1.0 / 0.6 ) );
				}

				
				v.dcolor        = clamp ( dcolor * _echoRGBA, 0.0, 2.0 );
    			v.pos 			= mul ( UNITY_MATRIX_MVP, ad.vertex * _echoScale );
       			
				return v;
			}
		
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				return v.dcolor;
			}

			ENDCG
		}
 	}
}
