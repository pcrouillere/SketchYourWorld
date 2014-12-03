//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Planet shader with rim lighting.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//# _echoRGBA          	- RGB Multiply
//# _Color          	- Color of rim lighting
//# _echoLightSize      - Size of rim lighting on lighted side
//# _echoDarkSize       - Size of rim lighting on dark side 
//# _echoGlowColor      - Color of Glow/Atmosphere
//# _echoGlowScale      - Size of Glow/Atmosphere
//# _echoGlowStrength   - Strength of rimlit effect on Glow/Atmosphere
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/FX/Planet-Rimlit-Atmosphere"
{
   	Properties 
	{
    	_MainTex ("Texture BackGround", 2D)								= "black" {} 
       	_echoUV("UV Offset u1 v1 u2 v2", Vector )						= ( 0, 0, 0, 0 )
     	_echoScale ("Scale XYZ", Vector )								= ( 1.0, 1.0, 1.0, 1.0 )
 		_echoRGBA ( "RGB Multiply", Vector )							= ( 1, 1, 1, 1 )    
     	_Color ("Rim Color", Color )								= ( 1,1,1,1)
     	_echoLightSize ("Light Side Rim Size", Range ( 1.0, 0.0 ) )		= 0.1
     	_echoDarkSize ("Dark Side Rim Size", Range ( 1.0, 0.0 ) )		= 0.4
     	_echoGlowColor ("Atmosphere Color", Color )						= ( 1,1,1,1)
      	_echoGlowScale ("Atmosphere Scale", Range ( 1.01, 1.2 ) )		= 1.1
      	_echoGlowStrength ("Atmosphere Strength", Range ( 0.0, 3.0 ) )	= 1.5
 	}
 	
	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry+800" "IgnoreProjector"="False" "RenderType"="echoLight" }

		UsePass "echoLogin/Light/FX/Planet-Rimlit/BASE"
		UsePass "Hidden/echoLogin/COREPASS-SPHEREGLOW"
 	}
 	
	Fallback "echoLogin/Light/Solid/Color"
}
 
