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
Shader "echoLogin/Transparent/Cutout/11-ZWrite"
{
	Properties 
   	{
		_MainTex ("Texture", 2D) 					= "gray" {}
		_TransparencyLM ("Transparency LM", 2d )	= "black"{}
      	_echoUV("UV Offset u1 v1", Vector )			= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )			= ( 1.0, 1.0, 1.0, 1.0 )
   	}
 
	//=========================================================================
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

  		UsePass "Hidden/echoLogin/COREPASS-ZWRITE"
		UsePass "echoLogin/Transparent/Cutout/10-Fastest/BASE"
 	}
}
