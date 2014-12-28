//$-----------------------------------------------------------------------------
//@ Additive Projector shader - For Unity Projectors. 
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//&-----------------------------------------------------------------------------
Shader "echoLogin/Projector/Additive"
{
	Properties 
   	{
    	_MainTex ("Texture", 2D )				= "black" {} 
  	}
   
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Overlay" "IgnoreProjector"="True"  }

    	Pass 
		{    
      	 	ZWrite Off
      	 	Blend One One
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			float4x4 	_Projector;
			
			#include "UnityCG.cginc"

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half4 tc1		: TEXCOORD0;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;

    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= mul ( _Projector, ad.vertex );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				return tex2Dproj ( _MainTex, v.tc1 );
    		}

			ENDCG
		}
 	}
}
