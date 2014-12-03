// CORE Framework include file --- DO NOT MODIFY - GO AWAY
	
#if defined (ECHODEF_VERTEXCOLOR)
	#define ECHO_VERTEXMULTIPLY *v.vcolor.xyz
#else
	#define ECHO_VERTEXMULTIPLY
#endif
							
#if defined (ECHODEF_DEFSTRUCTS)

#ifndef LIGHTMAP_OFF
			sampler2D   unity_Lightmap;
			float4   	unity_LightmapST;
#endif
         	struct VertInput
            {
                float4 vertex	 : POSITION;
                float2 texcoord	 : TEXCOORD0;
			  	float3 normal    : NORMAL;
	#if defined (ECHODEF_VERTEXCOLOR) || defined (ECHODEF_SPLATVERTEX)
			  	float4 color	: COLOR;
	#endif
	#ifdef LIGHTMAP_ON
			  	float4 texcoord1: TEXCOORD1;
	#endif
		};
        
           	struct Varys
            {
            	half4 pos		: SV_POSITION;
	#if !defined (ECHODEF_NO_TC1)
		#ifdef LIGHTMAP_ON
                half4 tc1		: TEXCOORD0;
	 	#else
	            half2 tc1		: TEXCOORD0;
	 	#endif
	#else
    	#ifdef LIGHTMAP_ON 
	            half4 tc1		: TEXCOORD0;
	 	#endif
	#endif

	#if defined (ECHODEF_2NDTEXUV) || defined ( ECHODEF_REFLECTION )
		#if defined ( ECHODEF_SPLATDETAIL )
                half4 tc2		: TEXCOORD1;
        #else
                half2 tc2		: TEXCOORD1;
        #endif
    #else
		#if defined ( ECHODEF_SPLATDETAIL )
                half4 tc2		: TEXCOORD1;
        #endif
	#endif

	#if defined (ECHO_VERTEX_LIGHT)
			  	fixed3 dcolor        	: TEXCOORD2;
	#endif

	#if defined (ECHO_DIRLIGHT_SHADOW)
		#if defined (ECHO_DIRSPOT_ON)
			  	fixed4 dirLight        	: TEXCOORD3;
		#else
			  	fixed3 dirLight        	: TEXCOORD3;
		#endif
	#endif

	#if defined (ECHODEF_SPECULAR)
				fixed3 scolor          : TEXCOORD4;
	#endif

	#if defined (ECHODEF_RIMLIT)
				fixed rimmix           : TEXCOORD5;
	#else
		#if defined (ECHODEF_SPLATVERTEX)
				fixed4 vrgba           : TEXCOORD5;
		#endif
	#endif

	#if defined (ECHODEF_VERTEXCOLOR)
		#ifdef LIGHTMAP_ON 
				half3 vcolor : TEXCOORD6;
		#endif
	#endif
		
	#ifndef SHADOWS_OFF			  	
		       	SHADOW_COORDS(7)
	#endif


            };
#endif


			Varys vert ( VertInput v )
			{
				Varys o;

#if defined (ECHODEF_FLIPNORMAL)
				v.normal *= -1.0;
#endif

				float3 dcolor 			= fixed3(0,0,0);
				float3 worldVertex		= mul ( _Object2World, v.vertex );
				float3 objectNormal		= normalize ( mul ( v.normal, (float3x3)_World2Object ) );
				float3 lightVector;
				float dotprod;
#if defined (ECHODEF_RIPPLE)
				float lightStrength		= 0.0;
				float ripple;
				float coloradj;

				coloradj 		= EchoWave ( v.texcoord.x, _echoAmount, _echoSpeed );
				ripple      	= coloradj * _echoHeight;
				v.vertex.xyz 	= v.vertex.xyz + ( float3 ( ripple, ripple, ripple ) * v.normal );
	#if defined (ECHODEF_FLIPNORMAL)
				coloradj *= -1.0;
	#endif

#endif

#if defined (ECHODEF_RIMLIT)
				float rimmix = 0.0;
				float rimnum;
#endif
		
#if defined (ECHODEF_SPECULAR)
				float3 worldViewDir     = EchoWorldViewDir ( worldVertex );
				float3 scolor           = float3(0,0,0);
#endif

#if defined (ECHO_DIR_ON) 
				lightVector = _echoDirLightDir.xyz;
				dotprod 	= max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) );

#if defined (ECHODEF_RIPPLE)
				lightStrength += dotprod;
#endif

#if defined (ECHO_DIRLIGHT_SHADOW)
	#if defined (ECHODEF_VERTEXCOLOR)
			o.dirLight.xyz = _echoDirLightColor.xyz * dotprod * v.color.xyz * _echoRGBA.xyz;
	#else
			o.dirLight.xyz = _echoDirLightColor.xyz * dotprod;
	#endif
	#if defined (ECHO_DIRSPOT_ON)
				o.dirLight.w = acos ( max ( 0, dot ( normalize ( _echoDirLightDir.xyz ), normalize ( ( _echoDirLightPos.xyz - worldVertex.xyz ).xyz ) ) ) );
	#endif
#else
				dcolor += _echoDirLightColor.xyz * dotprod;
#endif

#if defined (ECHODEF_SPECULAR)
	#if defined (ECHO_DIRSPEC_ON)
				scolor += EchoSpecular ( _Color.xyz, _echoShine, lightVector, objectNormal, worldViewDir );
	#endif
#endif

#if defined (ECHODEF_RIMLIT)
	#if defined (ECHO_DIRSPEC_ON) 
				rimnum = lerp ( _echoDarkSize, _echoLightSize, dotprod );
				dotprod = 1.0 - dot ( EchoWorldViewDir ( worldVertex ), objectNormal  );
				rimmix += smoothstep ( rimnum, 1.0, dotprod );
	#endif
#endif

#endif


#if defined (ECHO_POINT_ON)
				lightVector = Echo_MainPoint_LightDir ( worldVertex );
				dotprod = max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) );
				
	#if defined (ECHO_POINTFALLOFF_ON)
				float falloff = Echo_MainPoint_Falloff ( lightVector );	
				dcolor += _echoPointLightColor.xyz * dotprod * falloff;
		#if defined (ECHODEF_RIPPLE)
				lightStrength += dotprod * falloff;
		#endif
	#else
				dcolor += _echoPointLightColor.xyz * dotprod;
		#if defined (ECHODEF_RIPPLE)
				lightStrength += dotprod;
		#endif
	#endif

#if defined (ECHODEF_SPECULAR)
	#if defined (ECHO_POINTSPEC_ON) 
				scolor += EchoSpecular ( _echoPointLightColor.xyz, _echoShine, lightVector, objectNormal, worldViewDir );
	#endif
#endif

#if defined (ECHODEF_RIMLIT)
	#if defined (ECHO_POINTSPEC_ON) 
				rimnum = lerp ( _echoDarkSize, _echoLightSize, dotprod );
				dotprod = 1.0 - dot ( EchoWorldViewDir ( worldVertex ), objectNormal  );
				rimmix += smoothstep ( rimnum, 1.0, dotprod );
	#endif
#endif
					 
#endif

#if defined (ECHO_4POINT_ON)
       			for ( int index = 0; index < 4; index++ )
       			{
					lightVector = Echo_4Point_LightVector ( index, worldVertex );
					dotprod 	= max ( 0.0, dot ( objectNormal, normalize ( lightVector ) ) );

					dcolor += Echo_4Point_CalcLight ( index, dotprod, lightVector );

#if defined (ECHODEF_SPECULAR)
	#if defined (ECHO_4POINTSPEC_ON)
		#if !defined(ECHO_ADDBEAST_CODE)
					scolor += EchoSpecular ( float3( _echo4LightColorR[index],_echo4LightColorG[index],_echo4LightColorB[index] ), _echoShine, lightVector, objectNormal, worldViewDir );
		#endif 
	#endif 
#endif

#if defined (ECHODEF_RIMLIT)
	#if defined (ECHO_POINTSPEC_ON) 
				rimnum = lerp ( _echoDarkSize, _echoLightSize, dotprod );
				dotprod = 1.0 - dot ( EchoWorldViewDir ( worldVertex ), objectNormal  );
				rimmix += smoothstep ( rimnum, 1.0, dotprod );
	#endif
#endif
       			}
#endif

#if defined (ECHO_SHLIGHT_ON)
				dcolor += ShadeSH9 ( float4 ( mul( (float3x3) _Object2World, SCALED_NORMAL ), 1 ) );
#endif

#if defined (ECHODEF_RIPPLE)
				coloradj = clamp ( ( coloradj * 0.5 ) + 0.5 + ( lightStrength * 0.33 ), 0.01, 1.0 );
				dcolor        = dcolor * float3 ( coloradj, coloradj, coloradj );
#endif

#if defined (ECHODEF_SPLATVERTEX)
				o.vrgba = v.color;
#endif


#if defined (ECHO_VERTEX_LIGHT)
	#if defined (ECHODEF_VERTEXCOLOR)
				o.dcolor.xyz        = clamp ( dcolor * v.color.xyz * _echoRGBA.xyz, 0, 8);
	#else
				o.dcolor.xyz        = clamp ( dcolor, 0, 8);
	#endif
#endif

#if defined (ECHODEF_VERTEXCOLOR)
	#ifdef LIGHTMAP_ON
				o.vcolor.xyz        = v.color.xyz * _echoRGBA.xyz;
	#endif
#endif

#if defined (ECHODEF_SPECULAR)
    			o.scolor  = clamp ( scolor , 0, 2 );
#endif

#if defined (ECHODEF_RIMLIT)
				o.rimmix    = clamp ( rimmix, 0.0, 1.0 );
#endif
      			o.pos		= mul ( UNITY_MATRIX_MVP, v.vertex * _echoScale );
      			
#if !defined (ECHODEF_NO_TC1) 
   				o.tc1.xy 	  	= ( _MainTex_ST.xy * v.texcoord.xy ) + _echoUV.xy + _MainTex_ST.zw;
#endif

#ifdef LIGHTMAP_ON
  				o.tc1.zw 	  		= ( unity_LightmapST.xy * v.texcoord1.xy ) + unity_LightmapST.zw;
#endif

#if defined ( ECHODEF_2NDTEXUV )
				o.tc2.xy 	  	= ( _MainTex_ST.xy * v.texcoord.xy ) + _echoUV.zw;
#endif

#if defined ( ECHODEF_SPLATDETAIL )
				o.tc2.zw 	  	= ( _DetailTex_ST.xy * v.texcoord.xy );
#endif

#if defined ( ECHODEF_REFLECTION ) 
				float3 reflection = reflect ( normalize ( mul ( UNITY_MATRIX_MV , v.vertex ) ), float3 ( normalize ( mul ( (float3x3)UNITY_MATRIX_MV , v.normal ) ) ) );
	
				reflection.z += 1.0;
	
				float num = ( sqrt ( reflection.x * reflection.x + reflection.y * reflection.y + reflection.z * reflection.z ) * 2.0 );

				o.tc2.x		= ECHODEF_REFLECTION_STXY * ( reflection.x / num ) + _echoUV.z + ECHODEF_REFLECTION_STZW;
				o.tc2.y		= ECHODEF_REFLECTION_STXY * ( reflection.y / num ) + _echoUV.w + ECHODEF_REFLECTION_STZW;
#endif

#if !defined (ECHODEF_NOTEXTURE)
#if !defined (ECHODEF_NO_TC1)
#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
				{
					o.tc1.y = 1.0-o.tc1.y;
#if defined ( ECHODEF_2NDTEXUV )
					o.tc2.y = 1.0-o.tc2.y;
#endif
				}
#endif
#endif
#endif

#ifndef SHADOWS_OFF			  	
      			TRANSFER_SHADOW(o);
#endif

    			return o;
			}


#if defined (ECHODEF_USEFRAG)
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
#if !defined (ECHODEF_NOTEXTURE) && !defined (ECHODEF_NO_TC1)			
		#if defined (ECHODEF_LIGHTMAP) || defined (ECHODEF_TOGREY) || defined(ECHODEF_FIXED4TEX)
				fixed4 fcolor = tex2D ( _MainTex, v.tc1.xy );
		#else
				fixed3 fcolor = tex2D ( _MainTex, v.tc1.xy ).xyz;
		#endif
#else
	#if defined (ECHODEF_SOLIDCOLOR)
				fixed3 fcolor = ECHODEF_SOLIDCOLOR;
	#endif
#endif

#if defined (ECHODEF_SPLATVERTEX)
		fixed3 ter;
	#if defined (ECHODEF_SPLATDETAIL)
		fixed3 detail = tex2D ( _DetailTex, v.tc2.zw ).xyz;
		ter  = v.vrgba.rrr * detail;
		ter += v.vrgba.ggg * fcolor.xxx * _echoSplatColor1;
		ter += v.vrgba.bbb * fcolor.yyy * _echoSplatColor2;
		ter += v.vrgba.aaa * fcolor.zzz * _echoSplatColor3;
	#else
		ter  = v.vrgba.rrr * fcolor.xxx * _echoSplatColor1;
		ter += v.vrgba.ggg * fcolor.yyy * _echoSplatColor2;
		ter += v.vrgba.bbb * fcolor.zzz * _echoSplatColor3;
		ter += v.vrgba.aaa * fcolor.www * _echoSplatColor4;
	#endif
		fcolor = fixed4 ( ter, 1 );
#endif

#if defined (ECHODEF_SPLATCONTROL)
		fixed3 ter;
		fixed4 control = tex2D ( _ControlTex, v.tc2.zw );
	#if defined (ECHODEF_SPLATDETAIL)
		fixed3 detail = tex2D ( _DetailTex, v.tc2.zw ).xyz;
		ter  = control.rrr * detail;
		ter += control.ggg * fcolor.xxx * _echoSplatColor1;
		ter += control.bbb * fcolor.yyy * _echoSplatColor2;
		ter += control.aaa * fcolor.zzz * _echoSplatColor3;
	#else
		ter  = control.rrr * fcolor.xxx * _echoSplatColor1;
		ter += control.ggg * fcolor.yyy * _echoSplatColor2;
		ter += control.bbb * fcolor.zzz * _echoSplatColor3;
		ter += control.aaa * fcolor.www * _echoSplatColor4;
	#endif
		fcolor = fixed4 ( ter, 1 );
#endif

#if defined (ECHODEF_CUSTOMUV2TEX)
				ECHODEF_CUSTOMUV2TEX
#endif

#ifdef LIGHTMAP_ON
				fixed3 lcolor = fixed3(0,0,0);
#else
				fixed3 lcolor = _echoAmbientLightColorF.xyz;
#endif

#if defined (ECHODEF_CUSTOMCODE)
	ECHODEF_CUSTOMCODE
#endif				

#if defined (ECHO_DIRLIGHT_SHADOW)			  	
  	#if !defined (SHADOWS_OFF)
  			fixed atten = SHADOW_ATTENUATION(v);
	#else
			fixed atten = 1.0;
	#endif

	#ifdef ECHO_DIRSHADOWCOLOR_ON
				lcolor *= lerp ( _echoShadowColor.xyz, fixed3(1,1,1), atten );
	#endif

	#if defined (ECHO_DIRSPOT_ON)
				atten = min ( atten , ( step ( v.dirLight.w, _echoDirSpotSize ) ) );
	#endif

	#if !defined (ECHO_DIR_ONLYSHADOW)
    			lcolor +=  ( v.dirLight.xyz * atten );
	#endif
	
	#if defined (ECHO_VERTEX_LIGHT)
				lcolor += v.dcolor.xyz;
	#endif
#else
				lcolor += v.dcolor.xyz;
#endif

#if defined (ECHO_DOUBLELIGHT_ON)
				lcolor *= fixed3(2,2,2);
#endif

#ifdef LIGHTMAP_ON 
	#if defined (ECHO_DIRLIGHT_SHADOW)			  	
			  	lcolor.xyz += lerp ( fixed3(0,0,0), DecodeLightmap ( tex2D ( unity_Lightmap, v.tc1.zw ) )ECHO_VERTEXMULTIPLY, atten );
	#else
			  	lcolor.xyz += DecodeLightmap ( tex2D ( unity_Lightmap, v.tc1.zw ) )ECHO_VERTEXMULTIPLY;
	#endif
#endif

#if defined (ECHODEF_LIGHTMAP)
				lcolor = lerp ( lcolor, fixed3(1,1,1), fcolor.www );
#endif

#if defined (ECHODEF_SPECULAR)
				fcolor.xyz += v.scolor;
#endif

#if defined (ECHODEF_CUSTOMRETURN)
	ECHODEF_CUSTOMRETURN
#else
				return fixed4 ( fcolor.xyz * lcolor, 1 );
#endif				
			}
#endif
