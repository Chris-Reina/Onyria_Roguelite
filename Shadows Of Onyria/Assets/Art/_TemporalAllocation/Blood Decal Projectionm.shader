// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Blood Decal Projectionm"
{
    Properties
    {
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[ASEBegin]_Decal1("Decal", 2D) = "white" {}
		_Color1("Color", Color) = (0,0,0,0)
		_Smoothness1("Smoothness", Range( 0 , 1)) = 0
		_NormalStrength1("Normal Strength", Range( 0 , 1)) = 1
		_ObjectScale1("ObjectScale", Float) = 0
		_Radius1("Radius", Float) = 0
		_FallOff1("Fall Off", Float) = 1
		_CurrentValue1("CurrentValue", Range( 0 , 1)) = 0
		_PositionX1("PositionX", Float) = 0
		_PositionY1("PositionY", Float) = 0
		[ASEEnd]_PositionZ1("PositionZ", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

        [HideInInspector]_DrawOrder("Draw Order", Int) = 0
        [HideInInspector]_DecalMeshDepthBias("DecalMesh DepthBias", Float) = 0
        [HideInInspector]_DecalStencilWriteMask("Float", Int) = 0
        [HideInInspector]_DecalStencilRef("Float", Int) = 0
        [HideInInspector][ToggleUI]_AffectAlbedo("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_AffectNormal("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_AffectAO("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_AffectMetal("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_AffectSmoothness("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_AffectEmission("Boolean", Float) = 1
        [HideInInspector]_DecalColorMask0("Float", Int) = 0
        [HideInInspector]_DecalColorMask1("Float", Int) = 0
        [HideInInspector]_DecalColorMask2("Float", Int) = 0
        [HideInInspector]_DecalColorMask3("Float", Int) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
		LOD 0

		
        Tags { "RenderPipeline"="HDRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry" }

		HLSLINCLUDE
		#pragma target 4.5
		#pragma only_renderers d3d11 playstation xboxone vulkan metal switch

		struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float3 NormalTS;
            float NormalAlpha;
            float Metallic;
            float Occlusion;
            float Smoothness;
            float MAOSAlpha;
			float3 Emission;
        };
		ENDHLSL
		
		
        Pass
        { 
			
            Name "DBufferProjector"
            Tags { "LightMode"="DBufferProjector" }
    
            Cull Front
            Blend 0 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 1 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 2 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 3 Zero OneMinusSrcColor
            ZTest Greater
            ZWrite Off
            ColorMask [_DecalColorMask0] ColorMask [_DecalColorMask1] 1 ColorMask [_DecalColorMask2] 2 ColorMask [_DecalColorMask3] 3
            Stencil
            {
            	Ref [_DecalStencilRef]
            	WriteMask [_DecalStencilWriteMask]
            	Comp Always
            	Pass Replace
            	Fail Keep
            	ZFail Keep
            }

    
            HLSLPROGRAM
    
            #pragma shader_feature _ _MATERIAL_AFFECTS_ALBEDO
            #pragma shader_feature _ _MATERIAL_AFFECTS_NORMAL
            #pragma shader_feature _ _MATERIAL_AFFECTS_MASKMAP
            #define _MATERIAL_AFFECTS_EMISSION
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            #define ASE_SRP_VERSION 999999

    
            #pragma vertex Vert
            #pragma fragment Frag
            
            #pragma multi_compile_instancing
    
            #pragma multi_compile DECALS_3RT DECALS_4RT
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    
            #define SHADERPASS SHADERPASS_DBUFFER_PROJECTOR
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/Decal.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"

			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_TANGENT


            struct AttributesMesh
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

      		struct PackedVaryingsToPS
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				float3 positionRWS : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

            
            CBUFFER_START(UnityPerMaterial)
            float4 _Color1;
            float4 _Decal1_ST;
            float _NormalStrength1;
            float _Smoothness1;
            float _PositionX1;
            float _PositionY1;
            float _PositionZ1;
            float _Radius1;
            float _ObjectScale1;
            float _CurrentValue1;
            float _FallOff1;
            float _DrawOrder;
            float _DecalMeshDepthBias;
            float _DecalStencilWriteMask;
            float _DecalStencilRef;
			#ifdef _MATERIAL_AFFECTS_ALBEDO
            float _AffectAlbedo;
			#endif
			#ifdef _MATERIAL_AFFECTS_NORMAL
            float _AffectNormal;
			#endif
            #ifdef _MATERIAL_AFFECTS_MASKMAP
            float _AffectAO;
			float _AffectMetal;
            float _AffectSmoothness;
			#endif
			#ifdef _MATERIAL_AFFECTS_EMISSION
            float _AffectEmission;
			#endif
            float _DecalColorMask0;
            float _DecalColorMask1;
            float _DecalColorMask2;
            float _DecalColorMask3;
            CBUFFER_END
                
			sampler2D _Decal1;
			float4x4 unity_CameraProjection;
			float4x4 unity_CameraInvProjection;
			float4x4 unity_WorldToCamera;
			float4x4 unity_CameraToWorld;


			float3 PerturbNormal107_g1( float3 surf_pos, float3 surf_norm, float height, float scale )
			{
				// "Bump Mapping Unparametrized Surfaces on the GPU" by Morten S. Mikkelsen
				float3 vSigmaS = ddx( surf_pos );
				float3 vSigmaT = ddy( surf_pos );
				float3 vN = surf_norm;
				float3 vR1 = cross( vSigmaT , vN );
				float3 vR2 = cross( vN , vSigmaS );
				float fDet = dot( vSigmaS , vR1 );
				float dBs = ddx( height );
				float dBt = ddy( height );
				float3 vSurfGrad = scale * 0.05 * sign( fDet ) * ( dBs * vR1 + dBt * vR2 );
				return normalize ( abs( fDet ) * vN - vSurfGrad );
			}
			
                
            void GetSurfaceData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, PositionInputs posInput, float angleFadeFactor, out DecalSurfaceData surfaceData)
            {
                #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                    float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
                    float fadeFactor = clamp(normalToWorld[0][3], 0.0f, 1.0f) * angleFadeFactor;
                    float2 scale = float2(normalToWorld[3][0], normalToWorld[3][1]);
                    float2 offset = float2(normalToWorld[3][2], normalToWorld[3][3]);
                    fragInputs.texCoord0.xy = fragInputs.texCoord0.xy * scale + offset;
                    fragInputs.texCoord1.xy = fragInputs.texCoord1.xy * scale + offset;
                    fragInputs.texCoord2.xy = fragInputs.texCoord2.xy * scale + offset;
                    fragInputs.texCoord3.xy = fragInputs.texCoord3.xy * scale + offset;
                    fragInputs.positionRWS = posInput.positionWS;
                    fragInputs.tangentToWorld[2].xyz = TransformObjectToWorldDir(float3(0, 1, 0));
                    fragInputs.tangentToWorld[1].xyz = TransformObjectToWorldDir(float3(0, 0, 1));
                #else
                    #ifdef LOD_FADE_CROSSFADE 
                    LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                    #endif
    
                    float fadeFactor = 1.0;
                #endif
    
                ZERO_INITIALIZE(DecalSurfaceData, surfaceData);
    
                #ifdef _MATERIAL_AFFECTS_EMISSION
                #endif
    
                #ifdef _MATERIAL_AFFECTS_ALBEDO
                    surfaceData.baseColor.xyz = surfaceDescription.BaseColor;
                    surfaceData.baseColor.w = surfaceDescription.Alpha * fadeFactor;
                #endif
    
                #ifdef _MATERIAL_AFFECTS_NORMAL
                    #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) 
                        surfaceData.normalWS.xyz = mul((float3x3)normalToWorld, surfaceDescription.NormalTS);
                    #elif (SHADERPASS == SHADERPASS_DBUFFER_MESH) || (SHADERPASS == SHADERPASS_FORWARD_PREVIEW)
                        surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(surfaceDescription.NormalTS, fragInputs.tangentToWorld));
                    #endif
    
                    surfaceData.normalWS.w = surfaceDescription.NormalAlpha * fadeFactor;
                #else
                    #if (SHADERPASS == SHADERPASS_FORWARD_PREVIEW) 
                        surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(float3(0.0, 0.0, 0.1), fragInputs.tangentToWorld));
                    #endif
                #endif
    
                #ifdef _MATERIAL_AFFECTS_MASKMAP
                    surfaceData.mask.z = surfaceDescription.Smoothness;
                    surfaceData.mask.w = surfaceDescription.MAOSAlpha * fadeFactor;
    
                    #ifdef DECALS_4RT
                        surfaceData.mask.x = surfaceDescription.Metallic;
                        surfaceData.mask.y = surfaceDescription.Occlusion;
                        surfaceData.MAOSBlend.x = surfaceDescription.MAOSAlpha * fadeFactor;
                        surfaceData.MAOSBlend.y = surfaceDescription.MAOSAlpha * fadeFactor;
                    #endif
                                                                  
                #endif
            }
                
			PackedVaryingsToPS Vert(AttributesMesh inputMesh  )
			{
				PackedVaryingsToPS output;
					
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );
				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				output.ase_texcoord1.xyz = ase_worldNormal;
				float3 ase_worldTangent = TransformObjectToWorldDir(inputMesh.tangentOS.xyz);
				output.ase_texcoord2.xyz = ase_worldTangent;
				float ase_vertexTangentSign = inputMesh.tangentOS.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord3.xyz = ase_worldBitangent;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.w = 0;
				output.ase_texcoord2.w = 0;
				output.ase_texcoord3.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = inputMesh.normalOS;
				inputMesh.tangentOS = inputMesh.tangentOS;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);
				
				output.positionCS = TransformWorldToHClip(positionRWS);
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				o.positionRWS = positionRWS;
				#endif

			#if (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				#if UNITY_REVERSED_Z
				output.positionCS.z -= _DecalMeshDepthBias;
				#else
				output.positionCS.z += _DecalMeshDepthBias;
				#endif
			#endif
		
				return output;
			}

			void Frag( PackedVaryingsToPS packedInput,
			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				OUTPUT_DBUFFER(outDBuffer)
			#else
				out float4 outEmissive : SV_Target0
			#endif
			
			)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				UNITY_SETUP_INSTANCE_ID(packedInput);
				
				FragInputs input;
                ZERO_INITIALIZE(FragInputs, input);
                input.tangentToWorld = k_identity3x3;
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				float3 positionRWS = packedInput.positionRWS;
				input.positionRWS = positionRWS;
				#endif

                input.positionSS = packedInput.positionCS;

				DecalSurfaceData surfaceData;
				float clipValue = 1.0;
				float angleFadeFactor = 1.0;

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)    

				float depth = LoadCameraDepth(input.positionSS.xy);
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

				DecalPrepassData material;
				ZERO_INITIALIZE(DecalPrepassData, material);
				if (_EnableDecalLayers)
				{
					uint decalLayerMask = uint(UNITY_ACCESS_INSTANCED_PROP(Decal, _DecalLayerMaskFromDecal).x);

					DecodeFromDecalPrepass(posInput.positionSS, material);

					if ((decalLayerMask & material.decalLayerMask) == 0)
						clipValue -= 2.0;
				}

				
				float3 positionDS = TransformWorldToObject(posInput.positionWS);
				positionDS = positionDS * float3(1.0, -1.0, 1.0) + float3(0.5, 0.5, 0.5);
				if (!(all(positionDS.xyz > 0.0f) && all(1.0f - positionDS.xyz > 0.0f)))
				{
					clipValue -= 2.0; 
				}

			#ifndef SHADER_API_METAL
				clip(clipValue);
			#else
				if (clipValue > 0.0)
				{
			#endif
				input.texCoord0.xy = positionDS.xz;
				input.texCoord1.xy = positionDS.xz;
				input.texCoord2.xy = positionDS.xz;
				input.texCoord3.xy = positionDS.xz;

				float3 V = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
				if (_EnableDecalLayers)
				{
					float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
					float2 angleFade = float2(normalToWorld[1][3], normalToWorld[2][3]);

					if (angleFade.x > 0.0f)
					{
						float dotAngle = 1.0 - dot(material.geomNormalWS, normalToWorld[2].xyz);
						angleFadeFactor = 1.0 - saturate(dotAngle * angleFade.x + angleFade.y);
					}
				}

			#else
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, uint2(0, 0));
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
					float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
				#else
					float3 V = float3(1.0, 1.0, 1.0);
				#endif
			#endif

				float4 texCoord0 = input.texCoord0;
				float4 texCoord1 = input.texCoord1;
				float4 texCoord2 = input.texCoord2;
				float4 texCoord3 = input.texCoord3;
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				float2 uv_Decal1 = texCoord0.xy * _Decal1_ST.xy + _Decal1_ST.zw;
				float4 tex2DNode19 = tex2D( _Decal1, uv_Decal1 );
				
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 surf_pos107_g1 = ase_worldPos;
				float3 ase_worldNormal = packedInput.ase_texcoord1.xyz;
				float3 surf_norm107_g1 = ase_worldNormal;
				float height107_g1 = tex2DNode19.r;
				float scale107_g1 = _NormalStrength1;
				float3 localPerturbNormal107_g1 = PerturbNormal107_g1( surf_pos107_g1 , surf_norm107_g1 , height107_g1 , scale107_g1 );
				float3 ase_worldTangent = packedInput.ase_texcoord2.xyz;
				float3 ase_worldBitangent = packedInput.ase_texcoord3.xyz;
				float3x3 ase_worldToTangent = float3x3(ase_worldTangent,ase_worldBitangent,ase_worldNormal);
				float3 worldToTangentDir42_g1 = mul( ase_worldToTangent, localPerturbNormal107_g1);
				
				float3 appendResult10 = (float3(_PositionX1 , _PositionY1 , _PositionZ1));
				float lerpResult23 = lerp( tex2DNode19.r , 0.0 , saturate( pow( abs( ( distance( appendResult10 , ase_worldPos ) / ( ( _Radius1 * _ObjectScale1 ) * _CurrentValue1 ) ) ) , _FallOff1 ) ));
				
				surfaceDescription.BaseColor = ( _Color1 * tex2DNode19 ).rgb;
				surfaceDescription.Alpha = 1;
				surfaceDescription.NormalTS = worldToTangentDir42_g1;
				surfaceDescription.NormalAlpha = 1;
				surfaceDescription.Metallic = 0;
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Smoothness = _Smoothness1;
				surfaceDescription.MAOSAlpha = lerpResult23;
				surfaceDescription.Emission = float3( 0, 0, 0 );

				GetSurfaceData(surfaceDescription, input, V, posInput, angleFadeFactor, surfaceData);

			#if ((SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)) && defined(SHADER_API_METAL)
				} // if (clipValue > 0.0)

				clip(clipValue);
			#endif

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				ENCODE_INTO_DBUFFER(surfaceData, outDBuffer);
			#else
				// Emissive need to be pre-exposed
				outEmissive.rgb = surfaceData.emissive * GetCurrentExposureMultiplier();
				outEmissive.a = 1.0;
			#endif
			}

            ENDHLSL
        }

		
        Pass
        { 
			
            Name "DecalProjectorForwardEmissive"
            Tags { "LightMode"="DecalProjectorForwardEmissive" }
    
            Cull Front
            Blend 0 SrcAlpha One
            ZTest Greater
            ZWrite Off
    
            HLSLPROGRAM
    
           
            #pragma shader_feature _ _MATERIAL_AFFECTS_ALBEDO
            #pragma shader_feature _ _MATERIAL_AFFECTS_NORMAL
            #pragma shader_feature _ _MATERIAL_AFFECTS_MASKMAP
            #define _MATERIAL_AFFECTS_EMISSION
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            #define ASE_SRP_VERSION 999999

    
           
            #pragma vertex Vert
            #pragma fragment Frag
            
            #pragma multi_compile_instancing
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    
            #define SHADERPASS SHADERPASS_FORWARD_EMISSIVE_PROJECTOR
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/Decal.hlsl"

			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"

			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_TANGENT


            struct AttributesMesh
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

      		struct PackedVaryingsToPS
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				float3 positionRWS : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

            
            CBUFFER_START(UnityPerMaterial)
            float4 _Color1;
            float4 _Decal1_ST;
            float _NormalStrength1;
            float _Smoothness1;
            float _PositionX1;
            float _PositionY1;
            float _PositionZ1;
            float _Radius1;
            float _ObjectScale1;
            float _CurrentValue1;
            float _FallOff1;
            float _DrawOrder;
            float _DecalMeshDepthBias;
            float _DecalStencilWriteMask;
            float _DecalStencilRef;
            #ifdef _MATERIAL_AFFECTS_ALBEDO
            float _AffectAlbedo;
			#endif
            #ifdef _MATERIAL_AFFECTS_NORMAL
            float _AffectNormal;
			#endif
            #ifdef _MATERIAL_AFFECTS_MASKMAP
            float _AffectAO;
			float _AffectMetal;
            float _AffectSmoothness;
			#endif
            #ifdef _MATERIAL_AFFECTS_EMISSION
            float _AffectEmission;
			#endif
            float _DecalColorMask0;
            float _DecalColorMask1;
            float _DecalColorMask2;
            float _DecalColorMask3;
            CBUFFER_END
                
			sampler2D _Decal1;
			float4x4 unity_CameraProjection;
			float4x4 unity_CameraInvProjection;
			float4x4 unity_WorldToCamera;
			float4x4 unity_CameraToWorld;


			float3 PerturbNormal107_g1( float3 surf_pos, float3 surf_norm, float height, float scale )
			{
				// "Bump Mapping Unparametrized Surfaces on the GPU" by Morten S. Mikkelsen
				float3 vSigmaS = ddx( surf_pos );
				float3 vSigmaT = ddy( surf_pos );
				float3 vN = surf_norm;
				float3 vR1 = cross( vSigmaT , vN );
				float3 vR2 = cross( vN , vSigmaS );
				float fDet = dot( vSigmaS , vR1 );
				float dBs = ddx( height );
				float dBt = ddy( height );
				float3 vSurfGrad = scale * 0.05 * sign( fDet ) * ( dBs * vR1 + dBt * vR2 );
				return normalize ( abs( fDet ) * vN - vSurfGrad );
			}
			
                
            void GetSurfaceData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, PositionInputs posInput, float angleFadeFactor, out DecalSurfaceData surfaceData)
            {
                #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                    float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
                    float fadeFactor = clamp(normalToWorld[0][3], 0.0f, 1.0f) * angleFadeFactor;
                    float2 scale = float2(normalToWorld[3][0], normalToWorld[3][1]);
                    float2 offset = float2(normalToWorld[3][2], normalToWorld[3][3]);
                    fragInputs.texCoord0.xy = fragInputs.texCoord0.xy * scale + offset;
                    fragInputs.texCoord1.xy = fragInputs.texCoord1.xy * scale + offset;
                    fragInputs.texCoord2.xy = fragInputs.texCoord2.xy * scale + offset;
                    fragInputs.texCoord3.xy = fragInputs.texCoord3.xy * scale + offset;
                    fragInputs.positionRWS = posInput.positionWS;
                    fragInputs.tangentToWorld[2].xyz = TransformObjectToWorldDir(float3(0, 1, 0));
                    fragInputs.tangentToWorld[1].xyz = TransformObjectToWorldDir(float3(0, 0, 1));
                #else
                    #ifdef LOD_FADE_CROSSFADE 
                    LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                    #endif
    
                    float fadeFactor = 1.0;
                #endif
    
                ZERO_INITIALIZE(DecalSurfaceData, surfaceData);
    
                #ifdef _MATERIAL_AFFECTS_EMISSION
                    surfaceData.emissive.rgb = surfaceDescription.Emission.rgb * fadeFactor;
                #endif
    
                #ifdef _MATERIAL_AFFECTS_ALBEDO
                #endif
    
                #ifdef _MATERIAL_AFFECTS_NORMAL
                    #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) 
                    #elif (SHADERPASS == SHADERPASS_DBUFFER_MESH) || (SHADERPASS == SHADERPASS_FORWARD_PREVIEW)
                    #endif
    
                #else
                    #if (SHADERPASS == SHADERPASS_FORWARD_PREVIEW)
                    #endif
                #endif
    
                #ifdef _MATERIAL_AFFECTS_MASKMAP
    
                    #ifdef DECALS_4RT
                    #endif
                                                                  
                #endif
            }
                
			PackedVaryingsToPS Vert(AttributesMesh inputMesh  )
			{
				PackedVaryingsToPS output;
					
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );
				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				output.ase_texcoord1.xyz = ase_worldNormal;
				float3 ase_worldTangent = TransformObjectToWorldDir(inputMesh.tangentOS.xyz);
				output.ase_texcoord2.xyz = ase_worldTangent;
				float ase_vertexTangentSign = inputMesh.tangentOS.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord3.xyz = ase_worldBitangent;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.w = 0;
				output.ase_texcoord2.w = 0;
				output.ase_texcoord3.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = inputMesh.normalOS;
				inputMesh.tangentOS = inputMesh.tangentOS;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);
				
				output.positionCS = TransformWorldToHClip(positionRWS);
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				o.positionRWS = positionRWS;
				#endif

			#if (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				#if UNITY_REVERSED_Z
				output.positionCS.z -= _DecalMeshDepthBias;
				#else
				output.positionCS.z += _DecalMeshDepthBias;
				#endif
			#endif
		
				return output;
			}

			void Frag( PackedVaryingsToPS packedInput,
			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				OUTPUT_DBUFFER(outDBuffer)
			#else
				out float4 outEmissive : SV_Target0
			#endif
			
			)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				UNITY_SETUP_INSTANCE_ID(packedInput);
				
				FragInputs input;
                ZERO_INITIALIZE(FragInputs, input);
                input.tangentToWorld = k_identity3x3;
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				float3 positionRWS = packedInput.positionRWS;
				input.positionRWS = positionRWS;
				#endif

                input.positionSS = packedInput.positionCS;

				DecalSurfaceData surfaceData;
				float clipValue = 1.0;
				float angleFadeFactor = 1.0;

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)    

				float depth = LoadCameraDepth(input.positionSS.xy);
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

				DecalPrepassData material;
				ZERO_INITIALIZE(DecalPrepassData, material);
				if (_EnableDecalLayers)
				{
					uint decalLayerMask = uint(UNITY_ACCESS_INSTANCED_PROP(Decal, _DecalLayerMaskFromDecal).x);

					DecodeFromDecalPrepass(posInput.positionSS, material);

					if ((decalLayerMask & material.decalLayerMask) == 0)
						clipValue -= 2.0;
				}

				
				float3 positionDS = TransformWorldToObject(posInput.positionWS);
				positionDS = positionDS * float3(1.0, -1.0, 1.0) + float3(0.5, 0.5, 0.5);
				if (!(all(positionDS.xyz > 0.0f) && all(1.0f - positionDS.xyz > 0.0f)))
				{
					clipValue -= 2.0; 
				}

			#ifndef SHADER_API_METAL
				clip(clipValue);
			#else
				if (clipValue > 0.0)
				{
			#endif
				input.texCoord0.xy = positionDS.xz;
				input.texCoord1.xy = positionDS.xz;
				input.texCoord2.xy = positionDS.xz;
				input.texCoord3.xy = positionDS.xz;

				float3 V = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
				if (_EnableDecalLayers)
				{
					float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
					float2 angleFade = float2(normalToWorld[1][3], normalToWorld[2][3]);

					if (angleFade.x > 0.0f)
					{
						float dotAngle = 1.0 - dot(material.geomNormalWS, normalToWorld[2].xyz);
						angleFadeFactor = 1.0 - saturate(dotAngle * angleFade.x + angleFade.y);
					}
				}

			#else
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, uint2(0, 0));
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
					float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
				#else
					float3 V = float3(1.0, 1.0, 1.0);
				#endif
			#endif

				float4 texCoord0 = input.texCoord0;
				float4 texCoord1 = input.texCoord1;
				float4 texCoord2 = input.texCoord2;
				float4 texCoord3 = input.texCoord3;
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				float2 uv_Decal1 = texCoord0.xy * _Decal1_ST.xy + _Decal1_ST.zw;
				float4 tex2DNode19 = tex2D( _Decal1, uv_Decal1 );
				
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 surf_pos107_g1 = ase_worldPos;
				float3 ase_worldNormal = packedInput.ase_texcoord1.xyz;
				float3 surf_norm107_g1 = ase_worldNormal;
				float height107_g1 = tex2DNode19.r;
				float scale107_g1 = _NormalStrength1;
				float3 localPerturbNormal107_g1 = PerturbNormal107_g1( surf_pos107_g1 , surf_norm107_g1 , height107_g1 , scale107_g1 );
				float3 ase_worldTangent = packedInput.ase_texcoord2.xyz;
				float3 ase_worldBitangent = packedInput.ase_texcoord3.xyz;
				float3x3 ase_worldToTangent = float3x3(ase_worldTangent,ase_worldBitangent,ase_worldNormal);
				float3 worldToTangentDir42_g1 = mul( ase_worldToTangent, localPerturbNormal107_g1);
				
				float3 appendResult10 = (float3(_PositionX1 , _PositionY1 , _PositionZ1));
				float lerpResult23 = lerp( tex2DNode19.r , 0.0 , saturate( pow( abs( ( distance( appendResult10 , ase_worldPos ) / ( ( _Radius1 * _ObjectScale1 ) * _CurrentValue1 ) ) ) , _FallOff1 ) ));
				
				surfaceDescription.BaseColor = ( _Color1 * tex2DNode19 ).rgb;
				surfaceDescription.Alpha = 1;
				surfaceDescription.NormalTS = worldToTangentDir42_g1;
				surfaceDescription.NormalAlpha = 1;
				surfaceDescription.Metallic = 0;
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Smoothness = _Smoothness1;
				surfaceDescription.MAOSAlpha = lerpResult23;
				surfaceDescription.Emission = float3( 0, 0, 0 );

				GetSurfaceData(surfaceDescription, input, V, posInput, angleFadeFactor, surfaceData);

			#if ((SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)) && defined(SHADER_API_METAL)
				}

				clip(clipValue);
			#endif

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				ENCODE_INTO_DBUFFER(surfaceData, outDBuffer);
			#else
				// Emissive need to be pre-exposed
				outEmissive.rgb = surfaceData.emissive * GetCurrentExposureMultiplier();
				outEmissive.a = 1.0;
			#endif
			}

            ENDHLSL
        }

		
        Pass
        { 
			
            Name "DBufferMesh"
            Tags { "LightMode"="DBufferMesh" }
    
            Blend 0 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 1 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 2 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 3 Zero OneMinusSrcColor
            ZTest LEqual
            ZWrite Off
            ColorMask [_DecalColorMask0] ColorMask [_DecalColorMask1] 1 ColorMask [_DecalColorMask2] 2 ColorMask [_DecalColorMask3] 3
            
			Stencil
			{
				Ref [_DecalStencilRef]
				WriteMask [_DecalStencilWriteMask]
				Comp Always
				Pass Replace
				Fail Keep
				ZFail Keep
			}

    
            HLSLPROGRAM
    
            #pragma shader_feature _ _MATERIAL_AFFECTS_ALBEDO
            #pragma shader_feature _ _MATERIAL_AFFECTS_NORMAL
            #pragma shader_feature _ _MATERIAL_AFFECTS_MASKMAP
            #define _MATERIAL_AFFECTS_EMISSION
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            #define ASE_SRP_VERSION 999999

    
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma only_renderers d3d11 playstation xboxone vulkan metal switch
            #pragma multi_compile_instancing
    
            #pragma multi_compile DECALS_3RT DECALS_4RT
            
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    
            
            #define SHADERPASS SHADERPASS_DBUFFER_MESH
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/Decal.hlsl"
			
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"

			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_TANGENT


            struct AttributesMesh
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

			struct PackedVaryingsToPS
			{
				float4 positionCS : SV_POSITION;
                float3 interp0 : TEXCOORD0;
                float3 interp1 : TEXCOORD1;
                float4 interp2 : TEXCOORD2;
                float4 interp3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
            CBUFFER_START(UnityPerMaterial)
            float4 _Color1;
            float4 _Decal1_ST;
            float _NormalStrength1;
            float _Smoothness1;
            float _PositionX1;
            float _PositionY1;
            float _PositionZ1;
            float _Radius1;
            float _ObjectScale1;
            float _CurrentValue1;
            float _FallOff1;
            float _DrawOrder;
            float _DecalMeshDepthBias;
            float _DecalStencilWriteMask;
            float _DecalStencilRef;
            #ifdef _MATERIAL_AFFECTS_ALBEDO
            float _AffectAlbedo;
			#endif
            #ifdef _MATERIAL_AFFECTS_NORMAL
            float _AffectNormal;
			#endif
            #ifdef _MATERIAL_AFFECTS_MASKMAP
            float _AffectAO;
			float _AffectMetal;
            float _AffectSmoothness;
			#endif
            #ifdef _MATERIAL_AFFECTS_EMISSION
            float _AffectEmission;
			#endif
            float _DecalColorMask0;
            float _DecalColorMask1;
            float _DecalColorMask2;
            float _DecalColorMask3;
            CBUFFER_END
       
	   		sampler2D _Decal1;
	   		float4x4 unity_CameraProjection;
	   		float4x4 unity_CameraInvProjection;
	   		float4x4 unity_WorldToCamera;
	   		float4x4 unity_CameraToWorld;


			float3 PerturbNormal107_g1( float3 surf_pos, float3 surf_norm, float height, float scale )
			{
				// "Bump Mapping Unparametrized Surfaces on the GPU" by Morten S. Mikkelsen
				float3 vSigmaS = ddx( surf_pos );
				float3 vSigmaT = ddy( surf_pos );
				float3 vN = surf_norm;
				float3 vR1 = cross( vSigmaT , vN );
				float3 vR2 = cross( vN , vSigmaS );
				float fDet = dot( vSigmaS , vR1 );
				float dBs = ddx( height );
				float dBt = ddy( height );
				float3 vSurfGrad = scale * 0.05 * sign( fDet ) * ( dBs * vR1 + dBt * vR2 );
				return normalize ( abs( fDet ) * vN - vSurfGrad );
			}
			

            void GetSurfaceData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, PositionInputs posInput, float angleFadeFactor, out DecalSurfaceData surfaceData)
            {
                #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                    float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
                    float fadeFactor = clamp(normalToWorld[0][3], 0.0f, 1.0f) * angleFadeFactor;
                    float2 scale = float2(normalToWorld[3][0], normalToWorld[3][1]);
                    float2 offset = float2(normalToWorld[3][2], normalToWorld[3][3]);
                    fragInputs.texCoord0.xy = fragInputs.texCoord0.xy * scale + offset;
                    fragInputs.texCoord1.xy = fragInputs.texCoord1.xy * scale + offset;
                    fragInputs.texCoord2.xy = fragInputs.texCoord2.xy * scale + offset;
                    fragInputs.texCoord3.xy = fragInputs.texCoord3.xy * scale + offset;
                    fragInputs.positionRWS = posInput.positionWS;
                    fragInputs.tangentToWorld[2].xyz = TransformObjectToWorldDir(float3(0, 1, 0));
                    fragInputs.tangentToWorld[1].xyz = TransformObjectToWorldDir(float3(0, 0, 1));
                #else
                    #ifdef LOD_FADE_CROSSFADE
                    LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                    #endif
    
                    float fadeFactor = 1.0;
                #endif
    
                ZERO_INITIALIZE(DecalSurfaceData, surfaceData);
    
                #ifdef _MATERIAL_AFFECTS_EMISSION
                #endif
    
                #ifdef _MATERIAL_AFFECTS_ALBEDO
                    surfaceData.baseColor.xyz = surfaceDescription.BaseColor;
                    surfaceData.baseColor.w = surfaceDescription.Alpha * fadeFactor;
                #endif
    
                #ifdef _MATERIAL_AFFECTS_NORMAL
                    #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) 
                        surfaceData.normalWS.xyz = mul((float3x3)normalToWorld, surfaceDescription.NormalTS);
                    #elif (SHADERPASS == SHADERPASS_DBUFFER_MESH) || (SHADERPASS == SHADERPASS_FORWARD_PREVIEW)
                        surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(surfaceDescription.NormalTS, fragInputs.tangentToWorld));
                    #endif
    
                    surfaceData.normalWS.w = surfaceDescription.NormalAlpha * fadeFactor;
                #else
                    #if (SHADERPASS == SHADERPASS_FORWARD_PREVIEW)
                        surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(float3(0.0, 0.0, 0.1), fragInputs.tangentToWorld));
                    #endif
                #endif
    
                #ifdef _MATERIAL_AFFECTS_MASKMAP
                    surfaceData.mask.z = surfaceDescription.Smoothness;
                    surfaceData.mask.w = surfaceDescription.MAOSAlpha * fadeFactor;
    
                    #ifdef DECALS_4RT
                        surfaceData.mask.x = surfaceDescription.Metallic;
                        surfaceData.mask.y = surfaceDescription.Occlusion;
                        surfaceData.MAOSBlend.x = surfaceDescription.MAOSAlpha * fadeFactor;
                        surfaceData.MAOSBlend.y = surfaceDescription.MAOSAlpha * fadeFactor;
                    #endif
                                                                  
                #endif
            }

			PackedVaryingsToPS Vert(AttributesMesh inputMesh  )
			{
				PackedVaryingsToPS output;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				float3 ase_worldTangent = TransformObjectToWorldDir(inputMesh.tangentOS.xyz);
				float ase_vertexTangentSign = inputMesh.tangentOS.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord4.xyz = ase_worldBitangent;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord4.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = inputMesh.normalOS;
				inputMesh.tangentOS = inputMesh.tangentOS;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);

				output.interp0.xyz = positionRWS;
				output.positionCS = TransformWorldToHClip(positionRWS);
				output.interp1.xyz = normalWS;
				output.interp2.xyzw = tangentWS;
				output.interp3.xyzw = inputMesh.uv0;

			#if (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				#if UNITY_REVERSED_Z
					output.positionCS.z -= _DecalMeshDepthBias;
				#else
					output.positionCS.z += _DecalMeshDepthBias;
				#endif
			#endif

				return output;
			}

			void Frag(  PackedVaryingsToPS packedInput,
			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				OUTPUT_DBUFFER(outDBuffer)
			#else
				out float4 outEmissive : SV_Target0
			#endif
			
			)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				UNITY_SETUP_INSTANCE_ID(packedInput);

                FragInputs input;
                ZERO_INITIALIZE(FragInputs, input);
                
                input.tangentToWorld = k_identity3x3;
                input.positionSS = packedInput.positionCS;
                
                input.positionRWS = packedInput.interp0.xyz;
				float3 positionRWS = input.positionRWS;

                input.tangentToWorld = BuildTangentToWorld(packedInput.interp2.xyzw, packedInput.interp1.xyz);
                input.texCoord0 = packedInput.interp3.xyzw;

				DecalSurfaceData surfaceData;
				float clipValue = 1.0;
				float angleFadeFactor = 1.0;

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)    

				float depth = LoadCameraDepth(input.positionSS.xy);
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

				DecalPrepassData material;
				ZERO_INITIALIZE(DecalPrepassData, material);
				if (_EnableDecalLayers)
				{
					uint decalLayerMask = uint(UNITY_ACCESS_INSTANCED_PROP(Decal, _DecalLayerMaskFromDecal).x);

					DecodeFromDecalPrepass(posInput.positionSS, material);

					if ((decalLayerMask & material.decalLayerMask) == 0)
						clipValue -= 2.0;
				}

				float3 positionDS = TransformWorldToObject(posInput.positionWS);
				positionDS = positionDS * float3(1.0, -1.0, 1.0) + float3(0.5, 0.5, 0.5);
				if (!(all(positionDS.xyz > 0.0f) && all(1.0f - positionDS.xyz > 0.0f)))
				{
					clipValue -= 2.0;
				}

			#ifndef SHADER_API_METAL
				clip(clipValue);
			#else
				if (clipValue > 0.0)
				{
			#endif
				input.texCoord0.xy = positionDS.xz;
				input.texCoord1.xy = positionDS.xz;
				input.texCoord2.xy = positionDS.xz;
				input.texCoord3.xy = positionDS.xz;

				float3 V = GetWorldSpaceNormalizeViewDir(posInput.positionWS);

				if (_EnableDecalLayers)
				{
					float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
					float2 angleFade = float2(normalToWorld[1][3], normalToWorld[2][3]);

					if (angleFade.x > 0.0f)
					{
						float dotAngle = 1.0 - dot(material.geomNormalWS, normalToWorld[2].xyz);
						angleFadeFactor = 1.0 - saturate(dotAngle * angleFade.x + angleFade.y);
					}
				}

			#else
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, uint2(0, 0));
				float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
			#endif

				float4 texCoord0 = input.texCoord0;
				float4 texCoord1 = input.texCoord1;
				float4 texCoord2 = input.texCoord2;
				float4 texCoord3 = input.texCoord3;

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				float2 uv_Decal1 = texCoord0.xy * _Decal1_ST.xy + _Decal1_ST.zw;
				float4 tex2DNode19 = tex2D( _Decal1, uv_Decal1 );
				
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 surf_pos107_g1 = ase_worldPos;
				float3 surf_norm107_g1 = packedInput.interp1;
				float height107_g1 = tex2DNode19.r;
				float scale107_g1 = _NormalStrength1;
				float3 localPerturbNormal107_g1 = PerturbNormal107_g1( surf_pos107_g1 , surf_norm107_g1 , height107_g1 , scale107_g1 );
				float3 ase_worldBitangent = packedInput.ase_texcoord4.xyz;
				float3x3 ase_worldToTangent = float3x3(packedInput.interp2.xyz,ase_worldBitangent,packedInput.interp1);
				float3 worldToTangentDir42_g1 = mul( ase_worldToTangent, localPerturbNormal107_g1);
				
				float3 appendResult10 = (float3(_PositionX1 , _PositionY1 , _PositionZ1));
				float lerpResult23 = lerp( tex2DNode19.r , 0.0 , saturate( pow( abs( ( distance( appendResult10 , ase_worldPos ) / ( ( _Radius1 * _ObjectScale1 ) * _CurrentValue1 ) ) ) , _FallOff1 ) ));
				
				surfaceDescription.BaseColor = ( _Color1 * tex2DNode19 ).rgb;
				surfaceDescription.Alpha = 1;
				surfaceDescription.NormalTS = worldToTangentDir42_g1;
				surfaceDescription.NormalAlpha = 1;
				surfaceDescription.Metallic = 0;
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Smoothness = _Smoothness1;
				surfaceDescription.MAOSAlpha = lerpResult23;
				surfaceDescription.Emission = float3( 0, 0, 0 );

				GetSurfaceData(surfaceDescription, input, V, posInput, angleFadeFactor, surfaceData);

			#if ((SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)) && defined(SHADER_API_METAL)
				} 

				clip(clipValue);
			#endif

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				ENCODE_INTO_DBUFFER(surfaceData, outDBuffer);
			#else
				outEmissive.rgb = surfaceData.emissive * GetCurrentExposureMultiplier();
				outEmissive.a = 1.0;
			#endif
			}
            ENDHLSL
        }

		
        Pass
        { 
			
            Name "DecalMeshForwardEmissive"
            Tags { "LightMode"="DecalMeshForwardEmissive" }
    
            // Render State
            Blend 0 SrcAlpha One
            ZTest LEqual
            ZWrite Off
    
            HLSLPROGRAM
    
            #pragma shader_feature _ _MATERIAL_AFFECTS_ALBEDO
            #pragma shader_feature _ _MATERIAL_AFFECTS_NORMAL
            #pragma shader_feature _ _MATERIAL_AFFECTS_MASKMAP
            #define _MATERIAL_AFFECTS_EMISSION
            #pragma multi_compile _ LOD_FADE_CROSSFADE
            #define ASE_SRP_VERSION 999999

    
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma only_renderers d3d11 playstation xboxone vulkan metal switch
            #pragma multi_compile_instancing
    
            
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    
            
            #define SHADERPASS SHADERPASS_FORWARD_EMISSIVE_MESH
			
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/Decal.hlsl"
			
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"

			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_TANGENT


            struct AttributesMesh
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

			struct PackedVaryingsToPS
			{
				float4 positionCS : SV_POSITION;
                float3 interp0 : TEXCOORD0;
                float3 interp1 : TEXCOORD1;
                float4 interp2 : TEXCOORD2;
                float4 interp3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
            CBUFFER_START(UnityPerMaterial)
            float4 _Color1;
            float4 _Decal1_ST;
            float _NormalStrength1;
            float _Smoothness1;
            float _PositionX1;
            float _PositionY1;
            float _PositionZ1;
            float _Radius1;
            float _ObjectScale1;
            float _CurrentValue1;
            float _FallOff1;
            float _DrawOrder;
            float _DecalMeshDepthBias;
            float _DecalStencilWriteMask;
            float _DecalStencilRef;
            #ifdef _MATERIAL_AFFECTS_ALBEDO
            float _AffectAlbedo;
			#endif
            #ifdef _MATERIAL_AFFECTS_NORMAL
            float _AffectNormal;
			#endif
            #ifdef _MATERIAL_AFFECTS_MASKMAP
            float _AffectAO;
			float _AffectMetal;
            float _AffectSmoothness;
			#endif
            #ifdef _MATERIAL_AFFECTS_EMISSION
            float _AffectEmission;
			#endif
            float _DecalColorMask0;
            float _DecalColorMask1;
            float _DecalColorMask2;
            float _DecalColorMask3;
            CBUFFER_END
       
	   		sampler2D _Decal1;
	   		float4x4 unity_CameraProjection;
	   		float4x4 unity_CameraInvProjection;
	   		float4x4 unity_WorldToCamera;
	   		float4x4 unity_CameraToWorld;


			float3 PerturbNormal107_g1( float3 surf_pos, float3 surf_norm, float height, float scale )
			{
				// "Bump Mapping Unparametrized Surfaces on the GPU" by Morten S. Mikkelsen
				float3 vSigmaS = ddx( surf_pos );
				float3 vSigmaT = ddy( surf_pos );
				float3 vN = surf_norm;
				float3 vR1 = cross( vSigmaT , vN );
				float3 vR2 = cross( vN , vSigmaS );
				float fDet = dot( vSigmaS , vR1 );
				float dBs = ddx( height );
				float dBt = ddy( height );
				float3 vSurfGrad = scale * 0.05 * sign( fDet ) * ( dBs * vR1 + dBt * vR2 );
				return normalize ( abs( fDet ) * vN - vSurfGrad );
			}
			

            void GetSurfaceData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, PositionInputs posInput, float angleFadeFactor, out DecalSurfaceData surfaceData)
            {
                #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                    float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
                    float fadeFactor = clamp(normalToWorld[0][3], 0.0f, 1.0f) * angleFadeFactor;
                    float2 scale = float2(normalToWorld[3][0], normalToWorld[3][1]);
                    float2 offset = float2(normalToWorld[3][2], normalToWorld[3][3]);
                    fragInputs.texCoord0.xy = fragInputs.texCoord0.xy * scale + offset;
                    fragInputs.texCoord1.xy = fragInputs.texCoord1.xy * scale + offset;
                    fragInputs.texCoord2.xy = fragInputs.texCoord2.xy * scale + offset;
                    fragInputs.texCoord3.xy = fragInputs.texCoord3.xy * scale + offset;
                    fragInputs.positionRWS = posInput.positionWS;
                    fragInputs.tangentToWorld[2].xyz = TransformObjectToWorldDir(float3(0, 1, 0));
                    fragInputs.tangentToWorld[1].xyz = TransformObjectToWorldDir(float3(0, 0, 1));
                #else
                    #ifdef LOD_FADE_CROSSFADE
                    LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                    #endif
    
                    float fadeFactor = 1.0;
                #endif
    
                ZERO_INITIALIZE(DecalSurfaceData, surfaceData);
    
                #ifdef _MATERIAL_AFFECTS_EMISSION
                    surfaceData.emissive.rgb = surfaceDescription.Emission.rgb * fadeFactor;
                #endif
    
                #ifdef _MATERIAL_AFFECTS_ALBEDO
                    surfaceData.baseColor.xyz = surfaceDescription.BaseColor;
                    surfaceData.baseColor.w = surfaceDescription.Alpha * fadeFactor;
                #endif
    
                #ifdef _MATERIAL_AFFECTS_NORMAL
                    #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) 
                        surfaceData.normalWS.xyz = mul((float3x3)normalToWorld, surfaceDescription.NormalTS);
                    #elif (SHADERPASS == SHADERPASS_DBUFFER_MESH) || (SHADERPASS == SHADERPASS_FORWARD_PREVIEW)
                        surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(surfaceDescription.NormalTS, fragInputs.tangentToWorld));
                    #endif
    
                    surfaceData.normalWS.w = surfaceDescription.NormalAlpha * fadeFactor;
                #else
                    #if (SHADERPASS == SHADERPASS_FORWARD_PREVIEW) 
                        surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(float3(0.0, 0.0, 0.1), fragInputs.tangentToWorld));
                    #endif
                #endif
    
                #ifdef _MATERIAL_AFFECTS_MASKMAP
                    surfaceData.mask.z = surfaceDescription.Smoothness;
                    surfaceData.mask.w = surfaceDescription.MAOSAlpha * fadeFactor;
    
                    #ifdef DECALS_4RT
                        surfaceData.mask.x = surfaceDescription.Metallic;
                        surfaceData.mask.y = surfaceDescription.Occlusion;
                        surfaceData.MAOSBlend.x = surfaceDescription.MAOSAlpha * fadeFactor;
                        surfaceData.MAOSBlend.y = surfaceDescription.MAOSAlpha * fadeFactor;
                    #endif
                                                                  
                #endif
            }

			PackedVaryingsToPS Vert(AttributesMesh inputMesh  )
			{
				PackedVaryingsToPS output;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				float3 ase_worldTangent = TransformObjectToWorldDir(inputMesh.tangentOS.xyz);
				float ase_vertexTangentSign = inputMesh.tangentOS.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord4.xyz = ase_worldBitangent;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord4.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = inputMesh.normalOS;
				inputMesh.tangentOS = inputMesh.tangentOS;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);

				output.interp0.xyz = positionRWS;
				output.positionCS = TransformWorldToHClip(positionRWS);
				output.interp1.xyz = normalWS;
				output.interp2.xyzw = tangentWS;
				output.interp3.xyzw = inputMesh.uv0;

			#if (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				#if UNITY_REVERSED_Z
					output.positionCS.z -= _DecalMeshDepthBias;
				#else
					output.positionCS.z += _DecalMeshDepthBias;
				#endif
			#endif

				return output;
			}

			void Frag(  PackedVaryingsToPS packedInput,
			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				OUTPUT_DBUFFER(outDBuffer)
			#else
				out float4 outEmissive : SV_Target0
			#endif
			
			)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				UNITY_SETUP_INSTANCE_ID(packedInput);

                FragInputs input;
                ZERO_INITIALIZE(FragInputs, input);
                
                input.tangentToWorld = k_identity3x3;
                input.positionSS = packedInput.positionCS;
                
                input.positionRWS = packedInput.interp0.xyz;
				float3 positionRWS = input.positionRWS;

                input.tangentToWorld = BuildTangentToWorld(packedInput.interp2.xyzw, packedInput.interp1.xyz);
                input.texCoord0 = packedInput.interp3.xyzw;

				DecalSurfaceData surfaceData;
				float clipValue = 1.0;
				float angleFadeFactor = 1.0;

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)    

				float depth = LoadCameraDepth(input.positionSS.xy);
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

				DecalPrepassData material;
				ZERO_INITIALIZE(DecalPrepassData, material);
				if (_EnableDecalLayers)
				{
					uint decalLayerMask = uint(UNITY_ACCESS_INSTANCED_PROP(Decal, _DecalLayerMaskFromDecal).x);

					DecodeFromDecalPrepass(posInput.positionSS, material);

					if ((decalLayerMask & material.decalLayerMask) == 0)
						clipValue -= 2.0;
				}

				float3 positionDS = TransformWorldToObject(posInput.positionWS);
				positionDS = positionDS * float3(1.0, -1.0, 1.0) + float3(0.5, 0.5, 0.5);
				if (!(all(positionDS.xyz > 0.0f) && all(1.0f - positionDS.xyz > 0.0f)))
				{
					clipValue -= 2.0;
				}

			#ifndef SHADER_API_METAL
				clip(clipValue);
			#else
				if (clipValue > 0.0)
				{
			#endif
				input.texCoord0.xy = positionDS.xz;
				input.texCoord1.xy = positionDS.xz;
				input.texCoord2.xy = positionDS.xz;
				input.texCoord3.xy = positionDS.xz;

				float3 V = GetWorldSpaceNormalizeViewDir(posInput.positionWS);

				if (_EnableDecalLayers)
				{
					float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
					float2 angleFade = float2(normalToWorld[1][3], normalToWorld[2][3]);

					if (angleFade.x > 0.0f)
					{
						float dotAngle = 1.0 - dot(material.geomNormalWS, normalToWorld[2].xyz);
						angleFadeFactor = 1.0 - saturate(dotAngle * angleFade.x + angleFade.y);
					}
				}

			#else
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, uint2(0, 0));
				float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
			#endif

				float4 texCoord0 = input.texCoord0;
				float4 texCoord1 = input.texCoord1;
				float4 texCoord2 = input.texCoord2;
				float4 texCoord3 = input.texCoord3;

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				float2 uv_Decal1 = texCoord0.xy * _Decal1_ST.xy + _Decal1_ST.zw;
				float4 tex2DNode19 = tex2D( _Decal1, uv_Decal1 );
				
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 surf_pos107_g1 = ase_worldPos;
				float3 surf_norm107_g1 = packedInput.interp1;
				float height107_g1 = tex2DNode19.r;
				float scale107_g1 = _NormalStrength1;
				float3 localPerturbNormal107_g1 = PerturbNormal107_g1( surf_pos107_g1 , surf_norm107_g1 , height107_g1 , scale107_g1 );
				float3 ase_worldBitangent = packedInput.ase_texcoord4.xyz;
				float3x3 ase_worldToTangent = float3x3(packedInput.interp2.xyz,ase_worldBitangent,packedInput.interp1);
				float3 worldToTangentDir42_g1 = mul( ase_worldToTangent, localPerturbNormal107_g1);
				
				float3 appendResult10 = (float3(_PositionX1 , _PositionY1 , _PositionZ1));
				float lerpResult23 = lerp( tex2DNode19.r , 0.0 , saturate( pow( abs( ( distance( appendResult10 , ase_worldPos ) / ( ( _Radius1 * _ObjectScale1 ) * _CurrentValue1 ) ) ) , _FallOff1 ) ));
				
				surfaceDescription.BaseColor = ( _Color1 * tex2DNode19 ).rgb;
				surfaceDescription.Alpha = 1;
				surfaceDescription.NormalTS = worldToTangentDir42_g1;
				surfaceDescription.NormalAlpha = 1;
				surfaceDescription.Metallic = 0;
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Smoothness = _Smoothness1;
				surfaceDescription.MAOSAlpha = lerpResult23;
				surfaceDescription.Emission = float3( 0, 0, 0 );

				GetSurfaceData(surfaceDescription, input, V, posInput, angleFadeFactor, surfaceData);

			#if ((SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)) && defined(SHADER_API_METAL)
				} 

				clip(clipValue);
			#endif

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				ENCODE_INTO_DBUFFER(surfaceData, outDBuffer);
			#else
				outEmissive.rgb = surfaceData.emissive * GetCurrentExposureMultiplier();
				outEmissive.a = 1.0;
			#endif
			}
            ENDHLSL
        }
		
    }
    CustomEditor "Rendering.HighDefinition.DecalGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
	
	
}
/*ASEBEGIN
Version=18800
93;37;1518;926;1352.102;677.3261;1.3;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-1338.879,-470.717;Inherit;False;Property;_PositionZ1;PositionZ;10;0;Create;True;0;0;0;False;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1341.879,-614.717;Inherit;False;Property;_PositionX1;PositionX;8;0;Create;True;0;0;0;False;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1037.401,-253.0252;Inherit;False;Property;_ObjectScale1;ObjectScale;4;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1342.879,-542.717;Inherit;False;Property;_PositionY1;PositionY;9;0;Create;True;0;0;0;False;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1028.978,-339.9153;Inherit;False;Property;_Radius1;Radius;5;0;Create;True;0;0;0;False;0;False;0;2.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;10;-1193.879,-565.717;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1036.564,-172.1247;Inherit;False;Property;_CurrentValue1;CurrentValue;7;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-878.497,-282.2468;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;9;-1276.344,-367.2327;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-747.564,-222.1247;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;13;-1017.791,-447.5446;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;15;-597.6276,-431.1793;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-482.9549,-356.0363;Inherit;False;Property;_FallOff1;Fall Off;6;0;Create;True;0;0;0;False;0;False;1;1.27;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;17;-480.2865,-432.521;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;18;-328.9937,-434.7523;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-655.4709,-6.130246;Inherit;True;Property;_Decal1;Decal;0;0;Create;True;0;0;0;False;0;False;-1;None;9a3a06c253bbf444ea3882b94df0036e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;22;-183.9081,-435.3664;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;-524.6713,-173.0302;Inherit;False;Property;_Color1;Color;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.3490563,0.01090802,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;-529.4252,211.2638;Inherit;False;Property;_NormalStrength1;Normal Strength;3;0;Create;True;0;0;0;False;0;False;1;0.27;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;23;168.558,-302.2724;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;24;-109.8153,42.01308;Inherit;False;Normal From Height;-1;;1;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-101.7252,228.1638;Inherit;False;Property;_Smoothness1;Smoothness;2;0;Create;True;0;0;0;False;0;False;0;0.581;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-115.0153,-126.9869;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;440,-237;Float;False;True;-1;2;Rendering.HighDefinition.DecalGUI;0;10;Blood Decal Projectionm;d345501910c196f4a81c9eff8a0a5ad7;True;DBufferProjector;0;0;DBufferProjector;12;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;True;2;5;False;-1;10;False;-1;1;0;False;-1;10;False;-1;False;True;2;5;False;-1;10;False;-1;1;0;False;-1;10;False;-1;False;True;2;5;False;-1;10;False;-1;1;0;False;-1;10;False;-1;False;True;1;0;False;-1;6;False;-1;0;1;False;-1;0;False;-1;False;False;True;1;False;-1;False;True;True;True;True;True;0;True;-13;True;True;True;True;True;0;True;-14;True;True;True;True;True;0;True;-15;True;True;0;True;-5;255;False;-1;255;True;-4;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;3;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;2;False;-1;False;True;1;LightMode=DBufferProjector;False;0;;0;0;Standard;8;Affect BaseColor;1;Affect Normal;1;Affect Metal;1;Affect AO;1;Affect Smoothness;1;Affect Emission;1;Support LOD CrossFade;1;Vertex Position,InvertActionOnDeselection;1;0;4;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;Rendering.HighDefinition.DecalGUI;0;1;New Amplify Shader;d345501910c196f4a81c9eff8a0a5ad7;True;DecalMeshForwardEmissive;0;3;DecalMeshForwardEmissive;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;True;8;5;False;-1;1;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;1;LightMode=DecalMeshForwardEmissive;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;Rendering.HighDefinition.DecalGUI;0;1;New Amplify Shader;d345501910c196f4a81c9eff8a0a5ad7;True;DBufferMesh;0;2;DBufferMesh;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;True;2;5;False;-1;10;False;-1;1;0;False;-1;10;False;-1;False;True;2;5;False;-1;10;False;-1;1;0;False;-1;10;False;-1;False;True;2;5;False;-1;10;False;-1;1;0;False;-1;10;False;-1;False;True;1;0;False;-1;6;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;True;True;True;True;True;0;True;-13;True;True;True;True;True;0;True;-14;True;True;True;True;True;0;True;-15;True;True;0;True;-5;255;False;-1;255;True;-4;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;3;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;False;True;1;LightMode=DBufferMesh;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;False;-1;2;Rendering.HighDefinition.DecalGUI;0;1;New Amplify Shader;d345501910c196f4a81c9eff8a0a5ad7;True;DecalProjectorForwardEmissive;0;1;DecalProjectorForwardEmissive;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;True;8;5;False;-1;1;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;False;False;False;True;2;False;-1;True;2;False;-1;False;True;1;LightMode=DecalProjectorForwardEmissive;False;0;;0;0;Standard;0;False;0
WireConnection;10;0;5;0
WireConnection;10;1;7;0
WireConnection;10;2;4;0
WireConnection;11;0;8;0
WireConnection;11;1;6;0
WireConnection;14;0;11;0
WireConnection;14;1;12;0
WireConnection;13;0;10;0
WireConnection;13;1;9;0
WireConnection;15;0;13;0
WireConnection;15;1;14;0
WireConnection;17;0;15;0
WireConnection;18;0;17;0
WireConnection;18;1;16;0
WireConnection;22;0;18;0
WireConnection;23;0;19;1
WireConnection;23;2;22;0
WireConnection;24;20;19;0
WireConnection;24;110;20;0
WireConnection;25;0;21;0
WireConnection;25;1;19;0
WireConnection;0;0;25;0
WireConnection;0;2;24;40
WireConnection;0;6;26;0
WireConnection;0;7;23;0
ASEEND*/
//CHKSM=9346CFAFD119C2942E03A1925BD7F621B8541302