//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Dissolve effect.
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
//# _echoMix          	- Dissolve amount 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Unlit/Dissolve/11-ZWrite"
{
   	Properties 
	{
    	_MainTex ("Texture Main", 2D )					= "black" {} 
      	_echoUV("UV Offset u2 v2 u3 v3", Vector )		= ( 0, 0, 0, 0 )
 		_echoRGBA ( "RGB Add", Vector )					= ( 1, 1, 1, 1 )    
    	_echoMix("Mix", Range ( -0.3, 1.3 ) )			= 0
   	}

	//=========================================================================
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" }

  		UsePass "Hidden/echoLogin/COREPASS-ZWRITE"
		UsePass "echoLogin/Unlit/Dissolve/10-Fastest/BASE"

 	}
}
