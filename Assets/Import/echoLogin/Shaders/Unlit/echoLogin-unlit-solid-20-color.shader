//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Solid color shader which also uses vertex color.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _Color             - Object color 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/Solid/20-Color"
{
   	Properties 
	{
    	_Color ("Color", Color )	= ( 1, 1, 1, 1 )
  	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry" "IgnoreProjector"="False" "RenderType"="echoUnlit" }

    	Pass 
		{    
      		Cull Back
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF

			#include "UnityCG.cginc"
			#include "../Include/EchoLogin.cginc"

			sampler2D	_MainTex;

#ifndef LIGHTMAP_OFF
			sampler2D   unity_Lightmap;
			float4   	unity_LightmapST;
#endif

			float4      _Color;

          	struct VertInput
            {
                float4 vertex	: POSITION;
			  	float4 color	: COLOR;
#ifndef LIGHTMAP_OFF
			  	float4 texcoord1: TEXCOORD1;
#endif
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
 			  	fixed3 vcolor   : TEXCOORD0;
#ifndef LIGHTMAP_OFF
                half2 tc3		: TEXCOORD3;
#endif
           	};

 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;
				
    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
				v.vcolor		= ad.color.xyz * _Color.xyz;

#ifndef LIGHTMAP_OFF
   				v.tc3 	  		= ( unity_LightmapST.xy * ad.texcoord1.xy ) + unity_LightmapST.zw;
#endif

				return v;
			}
 	
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
#ifndef LIGHTMAP_OFF
			  	return fixed4 ( v.vcolor * DecodeLightmap ( tex2D ( unity_Lightmap, v.tc3 ) ), 1.0 );
#else
				return fixed4 ( v.vcolor, 1.0 );
#endif
			}

			ENDCG

		}
 	}
 }
