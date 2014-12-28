// CORE Framework include file --- DO NOT MODIFY - GO AWAY


#ifndef UNITY_CG_INCLUDED
uniform float4x4 	_Object2World, _World2Object;
#endif

uniform float4 		_echoScale;
uniform float4		_echoUV;

// =============================================
inline float3 EchoNormalDir ( float3 ivertex )
{
	return normalize ( mul ( ivertex , (float3x3)_World2Object ) );
}

// =============================================
inline float4 EchoVertexPos ( float4 ivertexpos )
{
	return mul ( _Object2World, ivertexpos );
}

// =============================================
inline float3 EchoObjViewDir ( float4 ivertexpos )
{
	return normalize ( ObjSpaceViewDir ( ivertexpos ) );
}

// =============================================
inline float3 EchoWorldViewDir ( float3 ivertexpos )
{
	return normalize ( _WorldSpaceCameraPos - ivertexpos );
}

// =============================================
inline float3 EchoSpecular ( float3 icolor, float ishine, float3 ilightdir, float3 inormaldir, float3 iviewdir )
{
	return icolor * pow ( max ( 0.0, dot ( reflect ( -normalize ( ilightdir ), inormaldir ), iviewdir ) ), float3 ( ishine, ishine, ishine ) ) ;
}

// =============================================
inline float EchoShieldHit ( in float4 ihitvec, in float ihitmix, in float3 inormaldir )
{
	float dotprod = clamp ( -dot ( inormaldir, EchoNormalDir ( normalize ( ihitvec.xyz ) ) ), 0, 1 );

	return ( smoothstep ( 0.0, 1.0, clamp ( ( dotprod * ihitvec.w - ( 1.0 - ihitmix ) ) * ( 5.0 ), 0, 2 ) ) );
}

// =============================================
inline float EchoWave ( float itexu, float iamount, float ispeed )
{
	return  ( itexu * sin ( itexu * iamount - ( _Time * ispeed ) ) );
}

// =============================================
inline float EchoRipple ( float2 itexuv, float iamount, float ispeed, float iheight, float icenterx, float icentery )
{
	itexuv.x += icenterx;
	itexuv.y += icentery;

	return sin ( length ( itexuv ) * iamount - ispeed * _Time ) * iheight;	
}

// =============================================
inline float echoRand ( float2 inv )
{
	return (sin(dot(inv.xy ,float2(12.9898,78.233))) * 43758.5453);
}
