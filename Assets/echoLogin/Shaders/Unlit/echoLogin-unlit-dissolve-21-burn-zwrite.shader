//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Dissolve with edge burn color to nothing.
//@
//# LIGHT PROBES        - NO
//# SHADOWS             - NO
//# BEAST LIGHTMAPPING  - YES
//# IGNORE PROJECTOR    - NO
//@
//@ NOTE: The alpha channel contains dissolve data. 
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoRGBA       	- Value to add or subtract from material rgb ( -2 to +2.0 ) 
//# _echoBurnSize       - Size of edge burn
//# _echoBurnColor      - Color of burn
//# _echoMix            - Dissolve amount 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/Dissolve/21-Burn-ZWrite"
{
   	Properties 
	{
    	_MainTex ("Texture Main", 2D )					= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
 		_echoRGBA ( "RGB Add", Vector )			= ( 1, 1, 1, 1 )    
    	_echoBurnSize("BurnSize", Range ( 0.0, 1.0 ) )	= 0.1
    	_echoBurnColor ( "BurnColor", Color )			= ( 0.8, 0.4, 0.0, 1 )
    	_echoMix("Mix", Range ( -0.3, 1.3 ) )			= 0
   	}

 	//=========================================================================
	SubShader 
	{
 		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" }

  		UsePass "Hidden/echoLogin/COREPASS-ZWRITE"
    	UsePass "echoLogin/Unlit/Dissolve/20-Burn/BASE"
 	}
}
