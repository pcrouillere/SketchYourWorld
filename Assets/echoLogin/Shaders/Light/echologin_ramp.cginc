// CORE Framework include file --- DO NOT MODIFY - GO AWAY

         	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal   : NORMAL;
			  	float4 color	: COLOR;
#if defined (ECHODEF_RAMP_BUMP) 
			  	float4 tangent  : TANGENT;
#endif
            };

           	struct Varys
            {
            	half4 pos		: SV_POSITION;
#if defined (ECHODEF_RAMP_TEXTURE) || defined (ECHODEF_RAMP_BUMP)
                half2 tc1		: TEXCOORD0;
#endif

#if defined (ECHO_DIRSPOT_ON)
      			fixed4 vcolor   : TEXCOORD1;
#else
      			fixed3 vcolor   : TEXCOORD1;
#endif
      			
#if defined (ECHO_POINT_RAMPFADE)
                fixed2 fragdata  : TEXCOORD2;
#else
                fixed3 fragdata  : TEXCOORD2;
#endif
#if defined (ECHO_VERTEX_LIGHT)
      			fixed3 fcolor   : TEXCOORD3;
#endif

#if defined (ECHO_DIRLIGHT_SHADOW)
			  	fixed3 dirLight	: TEXCOORD4;
#endif

#if defined (ECHODEF_RAMP_BUMP) && defined (ECHO_DIRBUMP_ON)
				fixed3 lightDir : TEXCOORD5;
#endif

#if defined (ECHODEF_RAMP_BUMP) && defined (ECHO_POINTBUMP_ON)
				fixed3 lightDir2 : TEXCOORD6;
#endif

#ifndef SHADOWS_OFF			  	
		       	SHADOW_COORDS(7)
#endif
            };

			// Vertex Shader     ===================== 	
 	
			Varys vert ( VertInput v )
			{
				Varys o;

				float3 worldVertex		= mul ( _Object2World, v.vertex );
				float3 objectNormal		= normalize ( mul ( v.normal, (float3x3)_World2Object ) );
//				float3 worldViewDir     = EchoWorldViewDir ( worldVertex );
				float3 lightVector;
#if defined (ECHO_VERTEX_LIGHT)
				float3 lcolor = float3(0,0,0);
#endif

// bump
#if defined (ECHODEF_RAMP_BUMP) 
          		float3 		tangent 		= normalize ( mul ( _Object2World, float4 ( v.tangent.xyz, 0.0 ) ).xyz );
            	float3 		binormal 		= normalize ( cross ( objectNormal, tangent ) * v.tangent.w ); 
       			float3x3	mymat 			= float3x3 ( tangent, binormal, objectNormal );
#endif

#if defined (ECHO_DIR_ON) 
 				lightVector 	= _echoDirLightDir.xyz;

	#if defined (ECHO_DIRSPOT_ON)
				o.vcolor.w = acos ( max ( 0, dot ( normalize ( _echoDirLightDir.xyz ), normalize ( ( _echoDirLightPos.xyz - worldVertex.xyz ).xyz ) ) ) );
	#endif

	#if defined (ECHODEF_RAMP_BUMP) && defined (ECHO_DIRBUMP_ON)
				o.lightDir 	= mul ( mymat, normalize ( lightVector ) ) * float3(1.42,1.42,1.42);
	#endif
 				
 	#if defined(ECHO_DIRSPEC_ON)
				o.fragdata.x 	= max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) );
	#else
		#if defined (ECHO_DIRLIGHT_SHADOW)
				o.dirLight = _echoDirLightColor.xyz * max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) );
		#else
			#if defined (ECHODEF_RAMP_BUMP)
				#if !defined (ECHO_DIRBUMP_ON)
					lcolor += _echoDirLightColor.xyz * max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) );
				#endif
			#else
					lcolor += _echoDirLightColor.xyz * max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) );
			#endif
		#endif
	#endif
#endif

#if defined (ECHO_POINT_ON) 
				lightVector 	= Echo_MainPoint_LightDir ( worldVertex );

		#if defined (ECHODEF_RAMP_BUMP) && defined (ECHO_POINTBUMP_ON)
				o.lightDir2 	= mul ( mymat, normalize ( lightVector ) );
		#endif

 	#if defined(ECHO_POINTSPEC_ON)

		#if defined	(ECHO_POINTFALLOFF_ON)
			#if defined (ECHO_POINT_RAMPFADE)
				o.fragdata.y 	= max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) ) * Echo_MainPoint_Falloff ( lightVector );
			#else
				o.fragdata.y 	= max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) );
				o.fragdata.z 	= Echo_MainPoint_Falloff ( lightVector );
			#endif
		#else
				o.fragdata.y 	= max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) );
		#endif
				
	#else
	
		#if defined (ECHODEF_RAMP_BUMP)
			#if !defined (ECHO_POINTBUMP_ON)
				#if defined (ECHO_POINTFALLOFF_ON)
				lcolor += Echo_MainPoint_CalcLight_Falloff ( dot ( objectNormal, normalize ( lightVector ) ), lightVector );
				#else
				lcolor += Echo_MainPoint_CalcLight ( dot ( objectNormal, normalize ( lightVector ) ) );
				#endif
			#endif
		#else
			#if defined (ECHO_POINTFALLOFF_ON)
				lcolor += Echo_MainPoint_CalcLight_Falloff ( dot ( objectNormal, normalize ( lightVector ) ), lightVector );
			#else
				lcolor += Echo_MainPoint_CalcLight ( dot ( objectNormal, normalize ( lightVector ) ) );
			#endif
		#endif
		

	#endif
#endif
				
#if defined (ECHO_4POINT_ON)
				float dotprod;
       			for ( int index = 0; index < 4; index++ )
       			{
					lightVector = Echo_4Point_LightVector ( index, worldVertex );
					dotprod 	= max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) ) * Echo_4Point_Falloff ( index, lightVector );
					lcolor += dotprod * float3 ( _echo4LightColorR[index],_echo4LightColorG[index],_echo4LightColorB[index]);
				}

#endif

#if defined (ECHO_SHLIGHT_ON)
				lcolor += ShadeSH9 ( float4 ( objectNormal, 1 ) );
#endif


#if defined (ECHO_VERTEX_LIGHT)
				o.fcolor        = clamp ( lcolor, 0, 2 );
#endif
				
				o.vcolor.xyz 	= v.color.xyz * _echoRGBA.xyz;
				o.pos			= mul ( UNITY_MATRIX_MVP, v.vertex * _echoScale );
      			
#ifndef SHADOWS_OFF			  	
       			TRANSFER_SHADOW(o);
#endif

#if defined (ECHODEF_RAMP_TEXTURE)
   				o.tc1 	  	= ( _MainTex_ST.xy * v.texcoord.xy ) + _echoUV.xy + _MainTex_ST.zw;
#else
		#if defined (ECHODEF_RAMP_BUMP)
   				o.tc1 	  	= ( _NormalMap_ST.xy * v.texcoord.xy ) + _echoUV.xy + _NormalMap_ST.zw;
		#endif
#endif

    			return o;
			}
			
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
	#if defined (ECHODEF_RAMP_BUMP) 
				fixed3 normal 	= EchoUnpackNormal ( tex2D ( _NormalMap, v.tc1.xy ) );
	#endif
	
#if defined (ECHO_VERTEX_LIGHT)
				fixed3 lcolor 			= _echoAmbientLightColor.xyz + v.fcolor;
#else
				fixed3 lcolor 			= _echoAmbientLightColor.xyz;
#endif

#if defined (ECHO_DIR_ON) 
	#if defined (ECHO_DIRSPEC_ON)
				#define ECHO_LCOLOR
				fixed3 dcolor;
		#if defined (ECHODEF_RAMP_BUMP) && defined (ECHO_DIRBUMP_ON) 
				dcolor = _echoDirLightColor.xyz * tex2D ( _echoRampDir, v.fragdata.xx ).xyz * max ( 0.0, dot ( normal, v.lightDir ) );
		#else
				dcolor = ( _echoDirLightColor.xyz * tex2D ( _echoRampDir, v.fragdata.xx ).xyz );
		#endif
	#else
		#if defined (ECHODEF_RAMP_BUMP) && defined (ECHO_DIRBUMP_ON) 
				#define ECHO_LCOLOR
				fixed3 dcolor;
				dcolor = _echoDirLightColor.xyz * max ( 0.0, dot ( normal, v.lightDir ) );
		#endif
	#endif
	
	#if defined (ECHO_LCOLOR)
		#ifndef SHADOWS_OFF
			    fixed atten = SHADOW_ATTENUATION(v);

			#if defined (ECHO_DIRSPOT_ON)
				atten = min ( atten , ( step ( v.vcolor.w, _echoDirSpotSize ) ) );
			#endif

   			#ifdef ECHO_DIRSHADOWCOLOR_ON
				lcolor *= lerp ( _echoShadowColor.xyz, fixed3(1,1,1), atten );
			#endif

				lcolor += dcolor *atten;
		#else
			#if defined (ECHO_DIRSPOT_ON)
				lcolor += dcolor * step ( v.vcolor.w, _echoDirSpotSize );
			#else
				lcolor += dcolor;
			#endif
		#endif
	#else
		#if defined (ECHO_DIRLIGHT_SHADOW)
			#if !defined (SHADOWS_OFF)
				fixed atten = SHADOW_ATTENUATION(v);
			#else
				fixed atten = 1.0;
			#endif

			#if defined (ECHO_DIRSPOT_ON)
				atten = min ( atten , ( step ( v.vcolor.w, _echoDirSpotSize ) ) );
			#endif

   			#ifdef ECHO_DIRSHADOWCOLOR_ON
				lcolor *= lerp ( _echoShadowColor.xyz, fixed3(1,1,1), atten );
			#endif		

				lcolor += v.dirLight * atten;
		#endif
	#endif
	
#endif

								
#if defined (ECHO_POINT_ON) 
	#if defined (ECHODEF_RAMP_BUMP) && defined (ECHO_POINTBUMP_ON) 
		#if defined (ECHO_POINTSPEC_ON)
			#if defined (ECHO_POINT_RAMPFADE) || !defined (ECHO_POINTFALLOFF_ON)
				lcolor += ( ( _echoPointLightColor * tex2D ( _echoRampPoint, v.fragdata.yy ).xyz ) ) * max ( 0.0, dot ( normal, v.lightDir2 ) );
			#else
				lcolor += ( ( _echoPointLightColor * tex2D ( _echoRampPoint, v.fragdata.yy ).xyz ) ) * max ( 0.0, dot ( normal, v.lightDir2 ) ) * v.fragdata.zzz;
			#endif
		#else
				lcolor += _echoPointLightColor * max ( 0.0, dot ( normal, v.lightDir2 ) );
		#endif	
	
	#else
		#if defined (ECHO_POINTSPEC_ON)
			#if defined (ECHO_POINT_RAMPFADE) || !defined (ECHO_POINTFALLOFF_ON)
				lcolor += ( _echoPointLightColor * tex2D ( _echoRampPoint, v.fragdata.yy ).xyz );
			#else
				lcolor += ( _echoPointLightColor * tex2D ( _echoRampPoint, v.fragdata.yy ).xyz ) * v.fragdata.zzz;
			#endif
		#endif
	#endif
#endif

#if defined (ECHODEF_RAMP_TEXTURE)
				return fixed4 ( tex2D ( _MainTex, v.tc1 ).xyz * lcolor * v.vcolor.xyz ECHO_DOUBLELIGHT3,  1 );
#else
				return fixed4 ( lcolor * v.vcolor.xyz ECHO_DOUBLELIGHT3,  1 );
#endif
			}