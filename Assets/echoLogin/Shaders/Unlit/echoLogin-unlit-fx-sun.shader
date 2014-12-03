//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Fades texture back and forth from different UV positions.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - Yes
//@
//@ Properties/Uniforms
//@
//# _echoUV         		- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _Color         	    - Color of Rim lighting
//# _echoRimSize            - Size of rimlit effect
//# _echoRGBA               - Color of Flares
//# _echoSpeed          	- Speed of flare aniamtion
//# _echoFlareScale         - Scale of all solar flares
//# _echoHeight             - Height of flares
//# _echoAmount             - Amount of flares
//# _echoGlowColor          - Color of Glow
//# _echoGlowScale          - Scale the Glow Size
//# _echoGlowStrength       - Strength of rimlit effect on Glow
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/FX/Sun"
{
	Properties 
	{
		_MainTex ("Texture", 2D)						= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
     	_Color ("Rim Color", Color ) 				= ( 1.0,0.1,0.2, 1.0 )
 		_echoRimSize ( "Rim Light Amount", Range ( 0.0, 8.0 ) )		=	4.0   
 	  	_echoRGBA ( "Flare Color", Color )						= ( 1, 1, 1, 1 )    
   		_echoSpeed ("Flare Speed", Range ( 0.0, 64.0 ) )	= 16
      	_echoFlareScale ("Flare Scale", Range ( 0.0, 64.0 ) )	= 16.0
   		_echoHeight ("Flare Height", Range ( 0.0, 64.0 ) )		= 1
   		_echoAmount ("Flare Amount", Range ( 1.0, 128.0 ) )		= 4
 	  	_echoGlowColor ( "Glow Color", Color )				= ( 1, 1, 1, 1 )    
     	_echoGlowScale ("Glow Scale", Range ( 1.0, 1.5 ) )		= 12.0
      	_echoGlowStrength ("Glow Strength", Range ( 0.0, 3.0 ) )	= 1.5
 	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry+800" "IgnoreProjector"="True" "RenderType"="echoUnlit" }


	   	Pass 
		{    
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

			float       _echoSpeed;
			fixed4      _Color;
			float 		_echoRimSize;

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
                fixed rimmix    : TEXCOORD3;
            };

			// ============================================= 	
			Varys vert ( VertInput  ad )
			{
				Varys v;
				float 	dotprod;
				float 	mixper;
			
				dotprod = abs ( dot ( EchoObjViewDir ( ad.vertex ), ad.normal ) );

				v.rimmix = 1.0 - clamp ( dotprod * _echoRimSize, 0.0, 1.0 );

    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 			= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy );
				
				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = tex2D ( _MainTex, v.tc1 );
				fcolor.xyz = lerp ( fcolor.xyz, _Color.xyz, v.rimmix );
    			return fcolor;
			}

			ENDCG
		}

		
    	Pass 
		{    
      	 	ZWrite Off
      	 	Blend SrcAlpha One
      		Cull Off
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			fixed4      _echoRGBA;
			float       _echoSpeed;
			float      	_echoFlareScale;
			float       _echoHeight;
			float       _echoAmount;
			float       _echoCenterX;
			float       _echoCenterY;

			#include "UnityCG.cginc"
			#include "../Include/EchoLogin.cginc"

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal   : NORMAL;
           	};

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                fixed rimlit    : TEXCOORD2;
            };
            
 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys 	v;
				float 	dotprod;
				float3 	normal;
				float ripple;

				dotprod = max ( 0.0, abs ( dot ( EchoObjViewDir ( ad.vertex ), ad.normal ) ) );
				v.rimlit = clamp ( dotprod * 1.2, 0.0, 1.0 );

				ripple 			= EchoRipple ( ad.texcoord, _echoAmount, _echoSpeed, _echoHeight , 0.0, 0.0  );
		
				dotprod = step ( 0.65, 1.0 - dotprod );
				
				ad.vertex.xyz 	=  ad.vertex.xyz + ( float3 ( ripple, ripple, ripple ) * ad.normal * dotprod );
				
				v.pos = mul( UNITY_MATRIX_MV, ad.vertex); 
     			normal = mul( (float3x3)UNITY_MATRIX_IT_MV, ad.normal);  
     			normal.z = 0.01;
     			v.pos = v.pos + float4(normalize(normal),0) * _echoFlareScale;
     			v.pos = mul(UNITY_MATRIX_P, v.pos);

				return v;
			}
		
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = _echoRGBA;
				
				fcolor.w *= v.rimlit; 
				
				return fcolor;
			}

			ENDCG
		}
		
		UsePass "Hidden/echoLogin/COREPASS-SPHEREGLOW"
 	}
 }
