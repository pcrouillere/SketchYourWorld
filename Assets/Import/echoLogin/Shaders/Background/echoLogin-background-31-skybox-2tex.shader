//$-----------------------------------------------------------------------------
//@ Background Shader 	- The fastest textured shader of this group.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - YES
//@
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Background/30-Skybox-2cube"
{
	Properties 
	{
    	_Cube ("Cube Texture", Cube)				= "grey" {} 
    	_Cube2 ("Cube Texture ( night )", Cube)				= "grey" {} 
      	_echoMix("Cloud Mix", Range ( 0, 1 ) ) 		= 0.0
   	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry+440" "IgnoreProjector"="True" "RenderType"="Background" }

    	Pass 
		{    
			ZWrite Off
      		Cull Off
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			samplerCUBE	_Cube;
			samplerCUBE	_Cube2;
			fixed		_echoMix;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float3 texcoord	: TEXCOORD0;
            };

           	struct Varys
            {
                float4 pos		: SV_POSITION;
                float3 tc1		: TEXCOORD0;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;

    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= ad.texcoord.xyz;

				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
			
			
    			return fixed4 ( lerp ( texCUBE ( _Cube, v.tc1 ).xyz, texCUBE ( _Cube2, v.tc1 ).xyz, _echoMix ), 1 );
			}

			ENDCG
		}
 	}
}
