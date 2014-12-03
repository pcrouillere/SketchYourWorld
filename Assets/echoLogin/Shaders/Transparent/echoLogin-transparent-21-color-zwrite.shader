//$-----------------------------------------------------------------------------
//@ Transparent Shader	- Use alpha channel for cutout.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - YES
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Transparent/21-Color-ZWrite"
{
	Properties 
   	{
		_MainTex ("Texture", 2D) 		= "gray" {}
 		_TransparencyLM ("Transparency LM", 2d )	= "black"{}
     	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
 		_echoRGBA ( "RGBA Multiply", Vector )			= ( 1, 1, 1, 1 )    
  	}
 
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="False" "RenderType"="Transparent" }

		UsePass "echoLogin/Transparent/20-Color/BASE"
  		UsePass "Hidden/echoLogin/COREPASS-ZWRITE"
 	}
}
