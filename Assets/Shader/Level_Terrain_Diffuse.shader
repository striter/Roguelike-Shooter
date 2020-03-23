Shader "Game/Extra/Level_Terrain_Diffuse"
{
	Properties
	{
		_GroundTex("Ground TEX",2D) = "white"{}
		_PlantsTex("Plants Tex",2D)="white"{}
		_UVScale("UV Scale",float) = 1
		_Color("Color Tint",Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }

			CGINCLUDE

			#include "../LongHauls/Shaders/CommonLightingInclude.cginc"
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _GroundTex;
			sampler2D _PlantsTex;
			sampler2D _TerrainMapTex;
			float4 _TerrainMapScale;
			float _UVScale;
			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)

			struct a2fDV
			{
				float4 vertex : POSITION;
				float3 normal:NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2fDV
			{
				float4 pos : SV_POSITION;
				float4 uv:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				float diffuse : TEXCOORD2;
				SHADOW_COORDS(3)
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			float4 GetUV(float3 worldPos)
			{
				return  float4(worldPos.x / _UVScale, worldPos.z / _UVScale , worldPos.x / _TerrainMapScale.z,worldPos.z / _TerrainMapScale.w);
			}

			float3 GetAlbedo(float4 uv)
			{
				float3 ground = tex2D(_GroundTex, uv.xy);
				float3 plant = tex2D(_PlantsTex, uv.xy);
				
				return  lerp(ground, plant, tex2D(_TerrainMapTex, uv.zw).r);

			}

			v2fDV DiffuseVertex(a2fDV v)
			{
				v2fDV o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.diffuse = GetDiffuse(mul(v.normal, (float3x3)unity_WorldToObject), UnityWorldSpaceLightDir(o.worldPos));
				o.uv = GetUV(o.worldPos);
				TRANSFER_SHADOW(o);
				return o;
			}

			float4 DiffuseFragmentBase(v2fDV i) :SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

				return float4(GetDiffuseBaseColor(GetAlbedo(i.uv)*UNITY_ACCESS_INSTANCED_PROP(Props, _Color), UNITY_LIGHTMODEL_AMBIENT.xyz, _LightColor0.rgb, atten, i.diffuse), 1);
			}

			float4 DiffuseFragmentAdd(v2fDV i) :SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
				return float4(GetDiffuseAddColor(_LightColor0.rgb, atten, i.diffuse), 1);
			}

			struct v2fs
			{
				V2F_SHADOW_CASTER;
			};

			v2fs ShadowVertex(appdata_base v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				v2fs o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
					return o;
			}

			fixed4 ShadowFragment(v2fs i) :SV_TARGET
			{
				SHADOW_CASTER_FRAGMENT(i);
			}

			ENDCG

			Pass
			{
				NAME "FORWARDBASE"
				Tags{"LightMode" = "ForwardBase"}
				Cull Back
				CGPROGRAM
				#pragma vertex DiffuseVertex
				#pragma fragment DiffuseFragmentBase
				#pragma multi_compile_fwdbase
				#pragma multi_compile_instancing
				ENDCG
			}

			Pass
			{
				Name "ForwardAdd"
				Tags{"LightMode" = "ForwardAdd"}
				Blend One One
				CGPROGRAM
				#pragma vertex DiffuseVertex
				#pragma fragment DiffuseFragmentAdd
				#pragma multi_compile_fwdadd_fullshadows
				#pragma multi_compile_instancing
				ENDCG
			}

			Pass
			{
				NAME "SHADOWCASTER"
				Tags{"LightMode" = "ShadowCaster"}
				CGPROGRAM
				#pragma vertex ShadowVertex
				#pragma fragment ShadowFragment
				#pragma multi_compile_instancing
				ENDCG
			}
		}
}