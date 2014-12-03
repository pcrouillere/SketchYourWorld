// CORE Framework include file --- DO NOT MODIFY - GO AWAY

#ifdef LIGHTMAP_ON
			sampler2D   unity_Lightmap;
			float4   	unity_LightmapST;
#endif

         	struct VertInput
            {
                float4 vertex	 : POSITION;
                float2 texcoord	 : TEXCOORD0;
			  	float3 normal    : NORMAL;
			  	float4 tangent   : TANGENT;
#ifdef LIGHTMAP_ON 
			  	float4 texcoord1: TEXCOORD1;
#endif
            };
        
           	struct Varys
            {
            	half4 pos				: SV_POSITION;
#if defined (ECHODEF_REFLECTION)
                half4 tc1				: TEXCOORD0;
#else
                half2 tc1				: TEXCOORD0;
#endif

#if defined (ECHO_VERTEX_LIGHT)
		  		fixed3 dcolor        	: TEXCOORD1;
#endif

#if defined (ECHO_DIRLIGHT_SHADOW)
	#if defined (ECHO_DIRSPOT_ON)
			  	fixed4 dirLight        	: TEXCOORD2;
	#else
			  	fixed3 dirLight        	: TEXCOORD2;
	#endif
#endif

#if defined (ECHO_DIRBUMP_ON)
	#if defined (ECHO_DIRSPOT_ON)
			  	fixed4 lightDir        	: TEXCOORD3;
	#else
			  	fixed3 lightDir        	: TEXCOORD3;
	#endif
#endif

#if defined (ECHO_POINTBUMP_ON)
	#if defined (ECHO_POINTFALLOFF_ON) && defined(ECHO_POINTBUMP_ON)
			  	fixed4 lightDir2       	: TEXCOORD4;
	#else
			  	fixed3 lightDir2       	: TEXCOORD4;
	#endif
#endif

#if defined (ECHODEF_SPECULAR)
            	fixed3 scolor 			: TEXCOORD5; 
#endif

#ifdef LIGHTMAP_ON
				half4 tc2               : TEXCOORD6;
#endif 

#ifndef SHADOWS_OFF			  	
		       	SHADOW_COORDS(7)
#endif

            };


			// ============================================= 	
			Varys vert ( VertInput v )
			{
				Varys o;
				float3 		dcolor 			= float3(0,0,0);
				float3 		worldVertex		= mul ( _Object2World, v.vertex );
				float3 		objectNormal	= normalize ( mul ( v.normal, (float3x3)_World2Object ) );
          		float3 		tangent 		= normalize ( mul ( _Object2World, float4 ( v.tangent.xyz, 0.0 ) ).xyz );
            	float3 		binormal 		= normalize ( cross ( objectNormal, tangent ) * v.tangent.w ); 
       			float3x3	mymat 			= float3x3 ( tangent, binormal, objectNormal );
#if defined (ECHODEF_SPECULAR)
            	float3 scolor 				= float3 (0,0,0); 
#endif

#if defined (ECHO_POINT_ON)
	#if defined (ECHO_POINTBUMP_ON)
   				o.lightDir2.xyz		= mul ( mymat, normalize ( Echo_MainPoint_LightDir ( worldVertex ) ) );
	#endif
	
	#if defined (ECHO_POINTFALLOFF_ON)
		#if defined (ECHO_POINTBUMP_ON)
				o.lightDir2.w  = Echo_MainPoint_Falloff ( Echo_MainPoint_LightDir ( worldVertex ) );
		#else
				dcolor += Echo_MainPoint_CalcLight_Falloff ( dot ( objectNormal, normalize ( Echo_MainPoint_LightDir ( worldVertex ) ) ), _echoPointLightPos.xyz - worldVertex );
		#endif
	#else
		#if !defined (ECHO_POINTBUMP_ON)
				dcolor += Echo_MainPoint_CalcLight ( dot ( objectNormal, normalize ( Echo_MainPoint_LightDir ( worldVertex ) ) ) );
		#endif
	#endif
	
	#if defined (ECHODEF_SPECULAR)
		#if defined (ECHO_POINTSPEC_ON)
				scolor += EchoSpecular ( _echoPointLightColor.xyz, _echoShine, Echo_MainPoint_LightDir(worldVertex), objectNormal, EchoWorldViewDir ( worldVertex ) );
		#endif
	#endif
	
#endif

#if defined (ECHO_DIR_ON) 
	#if defined (ECHO_DIRBUMP_ON)
       			o.lightDir.xyz 	= mul ( mymat, Echo_MainDir_LightDir() ) * float3(1.42,1.42,1.42);  // why 1.42 seems 2 make light same as unity shaders ??
			#if defined (ECHO_DIRSPOT_ON)
				o.lightDir.w = acos ( max ( 0, dot ( normalize ( _echoDirLightDir.xyz ), normalize ( ( _echoDirLightPos.xyz - worldVertex.xyz ).xyz ) ) ) );
			#endif
	#else
		#if defined (ECHO_DIRLIGHT_SHADOW)
				o.dirLight.xyz = _echoDirLightColor.xyz * max ( 0.0, dot ( objectNormal, normalize ( _echoDirLightDir.xyz ) ) );
			#if defined (ECHO_DIRSPOT_ON)
				o.dirLight.w = acos ( max ( 0, dot ( normalize ( _echoDirLightDir.xyz ), normalize ( ( _echoDirLightPos.xyz - worldVertex.xyz ).xyz ) ) ) );
			#endif
		#else
				dcolor += _echoDirLightColor.xyz * max ( 0.0, dot ( objectNormal, normalize ( _echoDirLightDir.xyz ) ) );
		#endif
	#endif
	
	#if defined (ECHODEF_SPECULAR)
		#if defined (ECHO_DIRSPEC_ON)
				scolor += EchoSpecular ( _Color.xyz, _echoShine, Echo_MainDir_LightDir(), objectNormal, EchoWorldViewDir ( worldVertex ) );
		#endif
	#endif

#endif

#if defined (ECHO_4POINT_ON)
       			// 4 lights
       			float3 lightVector;
       			for ( int index = 0; index < 4; index++ )
       			{
  					lightVector = Echo_4Point_LightVector ( index, worldVertex ) ;
					dcolor += Echo_4Point_CalcLight ( index, dot ( objectNormal, normalize ( lightVector ) ), lightVector );

	#if defined (ECHODEF_SPECULAR)
		#if defined (ECHO_4POINTSPEC_ON)
			#if !defined(ECHO_ADDBEAST_CODE)
					scolor += EchoSpecular ( float3(_echo4LightColorR[index],_echo4LightColorG[index],_echo4LightColorB[index]), _echoShine, Echo_4Point_LightVector ( index, worldVertex ), objectNormal, EchoWorldViewDir ( worldVertex ) );
			#endif
		#endif
	#endif
       			}
#endif

#ifdef LIGHTMAP_OFF
	#if defined (ECHO_SHLIGHT_ON)
				dcolor += ShadeSH9 ( float4 ( objectNormal, 1 ) );
	#endif
#endif

#if defined (ECHO_VERTEX_LIGHT)
				o.dcolor        = clamp ( dcolor, 0, 2 );
#endif
	   			o.pos			= mul ( UNITY_MATRIX_MVP, v.vertex );
   				o.tc1.xy  		= ( _MainTex_ST.xy * v.texcoord.xy ) + _echoUV.xy + _MainTex_ST.zw;

#if defined (ECHODEF_REFLECTION)
				float3 reflection = reflect ( normalize ( mul ( UNITY_MATRIX_MV , v.vertex ) ), float3 ( normalize ( mul ( (float3x3)UNITY_MATRIX_MV , v.normal ) ) ) );
	
				reflection.z += 1.0;
	
				float num = ( sqrt ( reflection.x * reflection.x + reflection.y * reflection.y + reflection.z * reflection.z ) * 2.0 );

				o.tc1.z		= _MainTex_ST.xy * ( reflection.x / num ) + _echoUV.z + _MainTex_ST.zw;
				o.tc1.w		= _MainTex_ST.xy * ( reflection.y / num ) + _echoUV.w + _MainTex_ST.zw ;
#endif

#ifdef LIGHTMAP_ON 
  				o.tc2.xy	= ( unity_LightmapST.xy * v.texcoord1.xy ) + unity_LightmapST.zw;
#endif

#if defined (ECHODEF_SPECULAR)
    			o.scolor  = clamp ( scolor , 0, 0.8 );
#endif

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					o.tc1.y = 1.0-o.tc1.y;
#endif

#ifndef SHADOWS_OFF			  	
      			TRANSFER_SHADOW(o);
#endif

    			return o;
			}
			

			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{

#if defined (ECHODEF_LIGHTMAP) || defined (ECHODEF_REFLECTION)
				fixed4 fcolor 	= tex2D ( _MainTex, v.tc1.xy );
#else
				fixed3 fcolor 	= tex2D ( _MainTex, v.tc1.xy ).xyz;
#endif

#ifdef LIGHTMAP_ON 
				fixed3 lcolor = fixed3(0,0,0);
#else
				fixed3 lcolor = _echoAmbientLightColorF.xyz;
#endif
				fixed3 normal 	= EchoUnpackNormal ( tex2D ( _NormalMap, v.tc1.xy ) );

#if defined (ECHODEF_REFLECTION)
				fcolor.xyz  = lerp ( fcolor.xyz, tex2D ( _EnvMap, v.tc1.zw ).xyz, fcolor.www );
#endif

#if defined (ECHO_DIRBUMP_ON)
	#if !defined (SHADOWS_OFF)
					fixed atten = SHADOW_ATTENUATION(v);

		#ifdef ECHO_DIRSHADOWCOLOR_ON
					lcolor *= lerp ( _echoShadowColor.xyz, fixed3(1,1,1), atten );
		#endif

		#if defined (ECHO_DIRSPOT_ON)
					atten = min ( atten , ( step ( v.lightDir.w, _echoDirSpotSize ) ) );
		#endif

					lcolor += _echoDirLightColor.xyz * ( ( max ( 0.0, dot ( normal, v.lightDir.xyz ) ) * atten ) );
	#else
		#if defined (ECHO_DIRLIGHT_SHADOW)			  	
					fixed atten = 1.0;
		#endif
		#if defined (ECHO_DIRSPOT_ON)
	            	lcolor += ( _echoDirLightColor.xyz * max ( 0.0, dot ( normal, v.lightDir.xyz ) ) ) * step ( v.lightDir.w, _echoDirSpotSize );
		#else
	            	lcolor += ( _echoDirLightColor.xyz * max ( 0.0, dot ( normal, v.lightDir ) ) );
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
					atten = min ( atten , ( step ( v.dirLight.w, _echoDirSpotSize ) ) );
		#endif

		#ifdef ECHO_DIRSHADOWCOLOR_ON
					lcolor *= lerp ( _echoShadowColor.xyz, fixed3(1,1,1), atten );
		#endif
    				lcolor += v.dirLight.xyz * atten;
	#endif
#endif

#if defined (ECHO_POINTBUMP_ON)
	#if defined (ECHO_POINTFALLOFF_ON)
            	lcolor += ( _echoPointLightColor.xyz * ( max ( 0.0, dot ( normal, v.lightDir2.xyz ) ) * v.lightDir2.w ) ) ;
	#else
            	lcolor += ( _echoPointLightColor.xyz * max ( 0.0, dot ( normal, v.lightDir2 ) ) ) ;
	#endif
#endif

#if defined (ECHO_VERTEX_LIGHT)
				lcolor += v.dcolor;
#endif

#if defined (ECHO_DOUBLELIGHT_ON)
				lcolor *= fixed3(2,2,2);
#endif

#ifdef LIGHTMAP_ON 
	#if defined (ECHO_DIRLIGHT_SHADOW)			  	
			  	lcolor.xyz += lerp ( fixed3(0,0,0), DecodeLightmap ( tex2D ( unity_Lightmap, v.tc2.zw ) ), atten );
	#else
			  	lcolor.xyz += DecodeLightmap ( tex2D ( unity_Lightmap, v.tc2.zw ) );
	#endif
#endif

#if defined (ECHODEF_LIGHTMAP)
				lcolor = lerp ( lcolor, fixed3(1,1,1), fcolor.www );
#endif

#if defined (ECHODEF_SPECULAR)
				fcolor.xyz += v.scolor;
#endif

				return fixed4 ( fcolor.xyz * lcolor, 1 );
			}


