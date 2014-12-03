//-----------------------------------------------------------------------------
// Outline		- Outline shader used by framework.
//-----------------------------------------------------------------------------
Shader "Hidden/echoLogin-Outline-Overlay-Static"
{
   	Properties 
	{
 		_MainTex ("Texture", 2D)								= "black" {} 
    	_echoGlowColor ("Outline Color", Color )				= ( 1,1,1,1)
      	_echoGlowScale ("Outline Scale", Range ( 1.01, 1.5 ) )	= 1.1
   		_echoSpeed ("Flare Speed", Range ( 64.0, 128.0 ) )		= 128
   	}
   	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Overlay" "IgnoreProjector"="True" }
		
		//=========================================================================
		// OUTLINE OVERLAY
		//=========================================================================
    	Pass 
		{
			Name "COREPASS-STATIC"    
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
			#include "../Include/EchoLogin-Light.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float      	_echoGlowScale;
			fixed4      _echoGlowColor;
			float       _echoGlowStrength;
			float       _echoSpeed;

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
               fixed alpha     : TEXCOORD2;
            };
                        
  			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys 	v;
				float 	dotprod;

				v.alpha = ( sin ( _Time.z * _echoGlowStrength ) * 0.5 ) + 0.5;

				v.pos = mul ( UNITY_MATRIX_MVP, ad.vertex * float4 ( _echoGlowScale, _echoGlowScale, _echoGlowScale, 1.0 ) ); 

  				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy );
 
   				v.tc1.x 	  	+= ( sin ( _Time * _echoSpeed * 2.0 ) * 0.5 ) + 0.5;
   				v.tc1.y 	  	+= ( cos ( _Time * _echoSpeed * 2.0 ) * 0.5 ) + 0.5;
     			
  				return v;
			}
		
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = _echoGlowColor;
				fixed4 scolor = tex2D ( _MainTex, v.tc1 );
				
				fcolor *= scolor;
				
				return fcolor;
			}

			ENDCG
		}
	}
}
 
