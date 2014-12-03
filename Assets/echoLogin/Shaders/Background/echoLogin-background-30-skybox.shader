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
Shader "echoLogin/Background/30-Skybox"
{
	Properties 
	{
    	_Cube ("Cube Texture", Cube)				= "grey" {} 
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
			float4		_echoUV;   

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
    			return texCUBE ( _Cube, v.tc1 );
			}

			ENDCG
		}
 	}
}
