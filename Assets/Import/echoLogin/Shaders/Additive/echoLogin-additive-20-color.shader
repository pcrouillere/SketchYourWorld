//$-----------------------------------------------------------------------------
//@ Additive shader - Uses vertex color and _TintColor coloring.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//# _echoUV         - The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _TintColor      - Color to tint
//&-----------------------------------------------------------------------------
Shader "echoLogin/Additive/21-Color"
{
	Properties 
   	{
    	_MainTex ( "Texture", 2D )				= "black" {} 
       	_echoUV ( "UV Offset u1 v1", Vector )	= ( 0, 0, 0, 0 )
  		_TintColor ( "Tint Color", Color )		= ( 0.5, 0.5, 0.5, 0.5 )    
  	}
   
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

    	Pass 
		{    
      	 	ZWrite Off
      	 	Cull Off
      	 	Blend SrcAlpha One
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			float4      _echoUV;
			float4		_TintColor;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float4 color	: COLOR;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
			  	fixed4 vcolor   : TEXCOORD1;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;

				v.vcolor		= clamp ( ad.color * _TintColor * 2.0, 0, 2 );
    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
    			return tex2D ( _MainTex, v.tc1 ) * v.vcolor;
			}

			ENDCG
		}
 	}
}
