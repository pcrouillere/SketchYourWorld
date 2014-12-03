// CORE Framework include file --- DO NOT MODIFY - GO AWAY
fixed     _echoDirSpotSize;
float4    _echoAmbientLightColor;
fixed4    _echoAmbientLightColorF;
fixed4    _echoShadowColor;

#if defined (ECHOFIXEDLIGHT)
	fixed4    _echoPointLightColor;
	fixed4    _echoDirLightColor;
	float4    _echoDirLightPos;
	float4    _echoDirLightDir;
	float4    _echoPointLightPos;
	float4    _echo4LightColorR;
	float4    _echo4LightColorG;
	float4    _echo4LightColorB;
	float4    _echo4LightIntensity;
	float4    _echo4LightPosX;
	float4    _echo4LightPosY;
	float4    _echo4LightPosZ;
	float4    _echo4LightRange;
	float     _echoPointLightRange;
#else
	float4    _echoPointLightColor;
	float4    _echoDirLightColor;
	float4    _echoDirLightPos;
	float4    _echoDirLightDir;
	float4    _echoPointLightPos;
	float4    _echo4LightColorR;
	float4    _echo4LightColorG;
	float4    _echo4LightColorB;
	float4    _echo4LightIntensity;
	float4    _echo4LightPosX;
	float4    _echo4LightPosY;
	float4    _echo4LightPosZ;
	float4    _echo4LightRange;
	float     _echoPointLightRange;
#endif

// dont look at this #ifdef hell below, it may cause insanity or seizures
#if defined (ECHO_BUMPMAP_SHADER)
	#if defined (ECHO_POINTDLIGHT_ON) ||defined (ECHO_4POINT_ON) || defined (ECHO_SHLIGHT_ON)
		#define ECHO_VERTEX_LIGHT
	#endif
	
	#if !defined (ECHO_DIRDLIGHT_ON) && defined (SHADOWS_OFF) && !defined (ECHO_VERTEX_LIGHT)
		#define ECHO_VERTEX_LIGHT
	#endif

	#if ( defined (ECHO_DIRDLIGHT_ON) && !defined(SHADOWS_OFF) ) || defined (ECHO_DIRSPOT_ON)
		#define ECHO_DIRLIGHT_SHADOW
	#endif
	
#else
	#if defined (ECHO_RAMP_SHADER)
		#if !defined (SHADOWS_OFF)
			#if ( defined (ECHO_DIRDLIGHT_ON) && defined (ECHO_DIRSPEC_OFF) ) || defined (ECHO_DIRSPOT_ON)
				#define ECHO_DIRLIGHT_SHADOW
				
				#if defined (ECHO_SHLIGHT_ON) || defined (ECHO_4POINT_ON) || defined (ECHO_POINTSPEC_OFF)
					#define ECHO_VERTEX_LIGHT
				#endif
			#else
				#if defined (ECHO_SHLIGHT_ON) || defined (ECHO_4POINT_ON) || defined (ECHO_DIRSPEC_OFF) || defined (ECHO_POINTSPEC_OFF)
					#define ECHO_VERTEX_LIGHT
				#endif
			#endif
		#else
			#if defined (ECHO_DIRSPOT_ON) && !defined (ECHO_DIRSPEC_ON)
				#define ECHO_DIRLIGHT_SHADOW

				#if defined (ECHO_SHLIGHT_ON) || defined (ECHO_4POINT_ON) || defined (ECHO_POINTSPEC_OFF)
					#define ECHO_VERTEX_LIGHT
				#endif
			#else
				#if defined (ECHO_SHLIGHT_ON) || defined (ECHO_4POINT_ON) || defined (ECHO_DIRSPEC_OFF) || defined (ECHO_POINTSPEC_OFF)
					#define ECHO_VERTEX_LIGHT
				#endif
			#endif
		#endif
	#else
		#if !defined (SHADOWS_OFF)
			#define ECHO_DIRLIGHT_SHADOW
			#if defined (ECHO_POINT_ON) ||defined (ECHO_4POINT_ON) || defined (ECHO_SHLIGHT_ON)
				#define ECHO_VERTEX_LIGHT
			#endif
		#else
			#if defined (ECHO_DIRSPOT_ON)
				#define ECHO_DIRLIGHT_SHADOW
				#if defined (ECHO_POINT_ON) ||defined (ECHO_4POINT_ON) || defined (ECHO_SHLIGHT_ON)
					#define ECHO_VERTEX_LIGHT
				#endif
			#else
				#if defined (ECHO_DIR_ON) || defined (ECHO_POINT_ON) ||defined (ECHO_4POINT_ON) || defined (ECHO_SHLIGHT_ON)
					#define ECHO_VERTEX_LIGHT
				#endif
			#endif
		#endif
	#endif

#endif

// ============================================= 	
inline float3 Echo_MainPoint_LightDir ( float3 iworldvertex )
{
 	return ( _echoPointLightPos.xyz - iworldvertex );
}

// ============================================= 	
inline float3 Echo_MainPoint_LightDir_Local ( float3 ivertex )
{
 	return ( mul ( _echoPointLightPos.xyz, (float3x3)_World2Object ) - ivertex );
}

// =============================================
 inline float Echo_MainPoint_Falloff ( float3 ilightvector )
 {
 	return 1.0 - clamp ( ( length ( ilightvector ) / _echoPointLightRange ), 0, 1 );
 }

// =============================================
inline float3 Echo_MainPoint_CalcLight_Falloff ( float idotprod, float3 ilightvector )
{
	return _echoPointLightColor.xyz * max ( 0.0, idotprod ) * Echo_MainPoint_Falloff ( ilightvector );
} 	

// =============================================
inline float3 Echo_MainPoint_CalcLight ( float idotprod )
{
	return _echoPointLightColor.xyz * max ( 0.0, idotprod );
} 	

// =============================================
inline float3 Echo_MainDir_CalcLight ( float idotprod )
{
	return _echoDirLightColor.xyz * max ( 0.0, idotprod );
} 	

// ============================================= 	
inline float3 Echo_MainDir_LightDir()
{
	return  _echoDirLightDir.xyz;
}

// =============================================
 inline float Echo_4Point_Falloff ( int index, float3 ilightvector )
 {
 	return clamp ( 1.0 - ( length ( ilightvector ) / _echo4LightRange[index] ), 0, 1 );
 }

// =============================================
 inline float3 Echo_4Point_CalcLight (  int index, float dotprod, float3 ilightvector )
{
  return float3 ( _echo4LightColorR[index],_echo4LightColorG[index],_echo4LightColorB[index]) * ( dotprod  * Echo_4Point_Falloff ( index, ilightvector ) );
}

// =============================================
 inline float3 Echo_4Point_LightVector ( int index, float3 iworldvertex	)
 {
	return float3 ( float3 ( _echo4LightPosX[index], _echo4LightPosY[index], _echo4LightPosZ[index] ) - iworldvertex );
 }
 
// =============================================
 inline fixed3 EchoUnpackNormal(fixed4 packednormal)
 {
#if defined(SHADER_API_GLES) && defined(SHADER_API_MOBILE)
	return packednormal.xyz * fixed3(2,2,2) - fixed3(1,1,1);
#else
	fixed3 normal;
	normal.xy = packednormal.wy * 2.0 - 1.0;
	normal.z = sqrt(1 - normal.x*normal.x - normal.y * normal.y);
	return normal;
#endif
 }
