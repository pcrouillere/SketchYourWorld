//$-----------------------------------------------------------------------------
//@ Background Shader 	- Fastest solid color shader of this group  
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//# _Color             - Color of background 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Background/00-solid"
{
	Properties 
	{
    	_Color ("Color", Color )	= ( 0, 0, 0, 0 )
   	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry+440" "IgnoreProjector"="True" "RenderType"="Background" }

    	Pass 
		{    
			ZWrite Off
      		Cull Back
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
			
			fixed4 _Color;

           	struct VertInput
            {
                float4 vertex	: POSITION;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;

    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );

				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
    			return _Color;
			}

			ENDCG
		}
 	}
}
