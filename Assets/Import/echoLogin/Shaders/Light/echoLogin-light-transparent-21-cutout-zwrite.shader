//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Uses alpha channel for lightmapping.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/Transparent/21-Cutout-ZWrite"
{
   	Properties 
	{
    	_MainTex ("Texture Alpha is cutout", 2D)		= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
    	_echoMix("Mix", Range ( -0.1, 1.0 ) )			= 0.5
  	}
   	
	//=========================================================================
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" }

		UsePass "Hidden/echoLogin/COREPASS-ZWRITE"
    	UsePass "echoLogin/Light/Transparent/20-cutout/BACK"
    	UsePass "echoLogin/Light/Transparent/20-cutout/FRONT"
	
	}
 	
	Fallback "echoLogin/Light/Solid/Color"

}
 
