// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Doat/MetallicSmoothness"
{
	Properties
	{
		_Tint("Tint", Color) = (1,1,1,1)
		_Albedo("Albedo", 2D) = "white" {}
		_TileandOffset("Tile and Offset", Vector) = (1,1,0,0)
		_MetallicIntensity("Metallic Intensity", Range( 0 , 1)) = 1
		_SmoothnessIntensity("Smoothness Intensity", Range( 0 , 1)) = 1
		_MetallicSmoothness("MetallicSmoothness", 2D) = "white" {}
		_NormalIntensity("Normal Intensity", Float) = 1
		[Normal]_Normal("Normal", 2D) = "white" {}
		_OcclusionStrength("Occlusion Strength", Range( 0 , 1)) = 0
		_Occlusion("Occlusion", 2D) = "white" {}
		_HeightScale("Height Scale", Range( 0 , 0.1)) = 0
		_Height("Height", 2D) = "white" {}
		[HDR]_EmissionTint("Emission Tint", Color) = (1,1,1,1)
		_Emission("Emission", 2D) = "white" {}
		[Toggle]_UseEmission("Use Emission", Float) = 0
		[Toggle]_InvertSmoothness("Invert Smoothness", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 viewDir;
			INTERNAL_DATA
			float3 worldNormal;
			float3 worldPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _TileandOffset;
		uniform sampler2D _Height;
		uniform float _HeightScale;
		uniform float4 _Height_ST;
		uniform float _NormalIntensity;
		uniform float4 _Tint;
		uniform sampler2D _Albedo;
		uniform sampler2D _Occlusion;
		uniform float _OcclusionStrength;
		uniform float _UseEmission;
		uniform sampler2D _Emission;
		uniform float4 _EmissionTint;
		uniform float _MetallicIntensity;
		uniform sampler2D _MetallicSmoothness;
		uniform float _InvertSmoothness;
		uniform float _SmoothnessIntensity;


		inline float2 POM( sampler2D heightMap, float2 uvs, float2 dx, float2 dy, float3 normalWorld, float3 viewWorld, float3 viewDirTan, int minSamples, int maxSamples, float parallax, float refPlane, float2 tilling, float2 curv, int index )
		{
			float3 result = 0;
			int stepIndex = 0;
			int numSteps = ( int )lerp( (float)maxSamples, (float)minSamples, saturate( dot( normalWorld, viewWorld ) ) );
			float layerHeight = 1.0 / numSteps;
			float2 plane = parallax * ( viewDirTan.xy / viewDirTan.z );
			uvs.xy += refPlane * plane;
			float2 deltaTex = -plane * layerHeight;
			float2 prevTexOffset = 0;
			float prevRayZ = 1.0f;
			float prevHeight = 0.0f;
			float2 currTexOffset = deltaTex;
			float currRayZ = 1.0f - layerHeight;
			float currHeight = 0.0f;
			float intersection = 0;
			float2 finalTexOffset = 0;
			while ( stepIndex < numSteps + 1 )
			{
			 	currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).r;
			 	if ( currHeight > currRayZ )
			 	{
			 	 	stepIndex = numSteps + 1;
			 	}
			 	else
			 	{
			 	 	stepIndex++;
			 	 	prevTexOffset = currTexOffset;
			 	 	prevRayZ = currRayZ;
			 	 	prevHeight = currHeight;
			 	 	currTexOffset += deltaTex;
			 	 	currRayZ -= layerHeight;
			 	}
			}
			int sectionSteps = 2;
			int sectionIndex = 0;
			float newZ = 0;
			float newHeight = 0;
			while ( sectionIndex < sectionSteps )
			{
			 	intersection = ( prevHeight - prevRayZ ) / ( prevHeight - currHeight + currRayZ - prevRayZ );
			 	finalTexOffset = prevTexOffset + intersection * deltaTex;
			 	newZ = prevRayZ - intersection * layerHeight;
			 	newHeight = tex2Dgrad( heightMap, uvs + finalTexOffset, dx, dy ).r;
			 	if ( newHeight > newZ )
			 	{
			 	 	currTexOffset = finalTexOffset;
			 	 	currHeight = newHeight;
			 	 	currRayZ = newZ;
			 	 	deltaTex = intersection * deltaTex;
			 	 	layerHeight = intersection * layerHeight;
			 	}
			 	else
			 	{
			 	 	prevTexOffset = finalTexOffset;
			 	 	prevHeight = newHeight;
			 	 	prevRayZ = newZ;
			 	 	deltaTex = ( 1 - intersection ) * deltaTex;
			 	 	layerHeight = ( 1 - intersection ) * layerHeight;
			 	}
			 	sectionIndex++;
			}
			return uvs.xy + finalTexOffset;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult27 = (float2(_TileandOffset.x , _TileandOffset.y));
			float2 appendResult28 = (float2(_TileandOffset.z , _TileandOffset.w));
			float2 uv_TexCoord7_g3 = i.uv_texcoord * appendResult27 + appendResult28;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float2 OffsetPOM8_g3 = POM( _Height, uv_TexCoord7_g3, ddx(uv_TexCoord7_g3), ddy(uv_TexCoord7_g3), ase_worldNormal, ase_worldViewDir, i.viewDir, 8, 8, _HeightScale, 0, _Height_ST.xy, float2(0,0), 0 );
			float3 finalNormal41_g3 = UnpackScaleNormal( tex2D( _Normal, OffsetPOM8_g3 ), _NormalIntensity );
			o.Normal = finalNormal41_g3;
			float4 tex2DNode18_g3 = tex2D( _Albedo, OffsetPOM8_g3 );
			float4 temp_cast_2 = (_OcclusionStrength).xxxx;
			float4 finalOcclusion21_g3 = pow( tex2D( _Occlusion, OffsetPOM8_g3 ) , temp_cast_2 );
			float4 finalBaseColor44_g3 = ( float4( ( _Tint.rgb * (tex2DNode18_g3).rgb ) , 0.0 ) * finalOcclusion21_g3 );
			o.Albedo = finalBaseColor44_g3.rgb;
			float4 color22_g3 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 finalEmission45_g3 = ( (( _UseEmission )?( tex2D( _Emission, OffsetPOM8_g3 ) ):( color22_g3 )) * _EmissionTint );
			o.Emission = finalEmission45_g3.rgb;
			float4 tex2DNode26_g3 = tex2D( _MetallicSmoothness, OffsetPOM8_g3 );
			float finalMetallic42_g3 = ( _MetallicIntensity * tex2DNode26_g3.r );
			o.Metallic = finalMetallic42_g3;
			float temp_output_55_0_g3 = _SmoothnessIntensity;
			float finalSmoothness43_g3 = ( tex2DNode26_g3.a * (( _InvertSmoothness )?( ( 1.0 - temp_output_55_0_g3 ) ):( temp_output_55_0_g3 )) );
			o.Smoothness = finalSmoothness43_g3;
			o.Occlusion = finalOcclusion21_g3.r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
40;76;1352;809;3429.759;500.659;2.035418;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;6;-2442.855,768.8375;Inherit;True;Property;_Emission;Emission;13;0;Create;True;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;7;-2439.741,580.9611;Inherit;True;Property;_Occlusion;Occlusion;9;0;Create;True;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;2;-2433.383,390.2906;Inherit;True;Property;_Height;Height;11;0;Create;True;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;5;-2437.517,208.5479;Inherit;True;Property;_Normal;Normal;7;1;[Normal];Create;True;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.Vector4Node;12;-1906.439,-264.64;Inherit;False;Property;_TileandOffset;Tile and Offset;2;0;Create;True;0;0;False;0;False;1,1,0,0;2,2,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;8;-2441.232,23.90811;Inherit;True;Property;_MetallicSmoothness;MetallicSmoothness;5;0;Create;True;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ColorNode;19;-1892.512,813.168;Inherit;False;Property;_EmissionTint;Emission Tint;12;1;[HDR];Create;True;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;27;-1690.796,-257.1101;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;3;-2435.453,-163.6045;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ColorNode;16;-1920.57,283.2414;Inherit;False;Property;_Tint;Tint;0;0;Create;True;0;0;False;0;False;1,1,1,1;0.7264151,0.7096825,0.6955767,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-1956.089,444.2604;Inherit;False;Property;_MetallicIntensity;Metallic Intensity;3;0;Create;True;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1954.5,513.0901;Inherit;False;Property;_SmoothnessIntensity;Smoothness Intensity;4;0;Create;True;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1955.288,719.1046;Inherit;False;Property;_OcclusionStrength;Occlusion Strength;8;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;35;-2049.35,302.8342;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WireNode;34;-2070.185,233.3866;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DynamicAppendNode;28;-1689.523,-166.8568;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;32;-2086.389,87.54726;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WireNode;31;-2079.444,18.09988;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WireNode;33;-2084.074,159.3096;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1954.449,650.6337;Inherit;False;Property;_HeightScale;Height Scale;10;0;Create;True;0;0;False;0;False;0;0.05;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1886.348,581.592;Inherit;False;Property;_NormalIntensity;Normal Intensity;6;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;36;-1192.097,-79.06609;Inherit;False;MetallicSmoothnessOpaque;14;;3;ba9aaafe8272fd047b6a07a0e0e419ac;0;16;2;FLOAT2;1,1;False;3;FLOAT2;1,1;False;59;SAMPLER2D;0,0;False;64;SAMPLER2D;0,0;False;60;SAMPLER2D;0,0;False;4;SAMPLER2D;0,0;False;63;SAMPLER2D;0,0;False;62;SAMPLER2D;0,0;False;51;FLOAT3;0,0,0;False;52;FLOAT;0;False;54;FLOAT;0;False;55;FLOAT;1;False;53;FLOAT;0.7;False;9;FLOAT;1;False;56;FLOAT;0;False;57;COLOR;0,0,0,0;False;6;COLOR;1;FLOAT3;68;COLOR;70;FLOAT;72;FLOAT;74;COLOR;76
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-703.8975,-81.02192;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Doat/MetallicSmoothness;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;27;0;12;1
WireConnection;27;1;12;2
WireConnection;35;0;6;0
WireConnection;34;0;7;0
WireConnection;28;0;12;3
WireConnection;28;1;12;4
WireConnection;32;0;5;0
WireConnection;31;0;8;0
WireConnection;33;0;2;0
WireConnection;36;2;27;0
WireConnection;36;3;28;0
WireConnection;36;59;3;0
WireConnection;36;64;31;0
WireConnection;36;60;32;0
WireConnection;36;4;33;0
WireConnection;36;63;34;0
WireConnection;36;62;35;0
WireConnection;36;51;16;0
WireConnection;36;54;14;0
WireConnection;36;55;17;0
WireConnection;36;53;13;0
WireConnection;36;9;11;0
WireConnection;36;56;18;0
WireConnection;36;57;19;0
WireConnection;0;0;36;1
WireConnection;0;1;36;68
WireConnection;0;2;36;70
WireConnection;0;3;36;72
WireConnection;0;4;36;74
WireConnection;0;5;36;76
ASEEND*/
//CHKSM=993F9C699B48D01BC7C8555206540D24EA16E60E