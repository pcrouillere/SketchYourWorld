//$-----------------------------------------------------------------------------
//@ Lighted Shader	- Dissolve to nothing effect.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ NOTE: The alpha channel contains dissolve data. 
//@
//@ Properties/Uniforms
//@
//# _echoUV         - The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scale Mesh in XYZ, W should always be 1
//# _echoRGBA       - Value to add or subtract from material rgb ( -2 to +2.0 ) 
//# _echoMix        - Dissolve amount 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/Light/Transparent/31-Dissolve-ZWrite"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)				= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
 		_echoRGBA ( "RGB Multiply", Vector )	= ( 1, 1, 1, 1 )    
    	_echoMix("Mix", Range ( -0.3, 1.3 ) )	= 0
   	}

	//=========================================================================
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="False" "RenderType"="TransparentCutout"}

		UsePass "Hidden/echoLogin/COREPASS-ZWRITE"
		UsePass "echoLogin/Light/Transparent/30-Dissolve/BASE"
		
	}

	Fallback "echoLogin/Light/Solid/Color"
}
