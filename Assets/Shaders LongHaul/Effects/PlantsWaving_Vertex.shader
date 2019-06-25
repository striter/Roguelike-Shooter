// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Game/Effect/PlantsWaving_Vertex"
{
	Properties
	{
		_MainTex("Color UV TEX",2D) = "white"{}
		_Color("Color Tint",Color) = (1,1,1,1)
		_VerticalWaveParam("Vertical Wave Edge",Range(-1,1))=.05
		_HorizontalWaveParam("Vertical Wave Edge",Range(-1,1))=.05
			_WaveSpeed("Wave Speed",Range(0,5))=1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
				Cull Back

				CGINCLUDE
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
				ENDCG

			Pass		//Base Pass
			{
				Tags{ "LightMode" = "ForwardBase"}
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal:NORMAL;
					float2 uv:TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv:TEXCOORD0;
					float3 worldPos:TEXCOORD1;
					float diffuse : TEXCOORD2;
					SHADOW_COORDS(3)
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Color;
				float _VerticalWaveParam;
				float _HorizontalWaveParam;
				float _WaveSpeed;
				v2f vert(appdata v)
				{
					v2f o;
					o.uv = v.uv;
					o.worldPos = mul(unity_ObjectToWorld,v.vertex);
					fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject)); //法线方向n
					fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(o.worldPos));
					o.diffuse = saturate(dot(worldLightDir,worldNormal));
					float wave = v.vertex.y*sin(_Time.y*_WaveSpeed);
					o.worldPos +=float3(wave*_HorizontalWaveParam, 0, wave*_VerticalWaveParam) / 100;
					o.pos = UnityWorldToClipPos(o.worldPos);
					
					TRANSFER_SHADOW(o);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float3 albedo = tex2D(_MainTex,i.uv)*_Color;
					UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
					fixed3 ambient = albedo * UNITY_LIGHTMODEL_AMBIENT.xyz;
					atten = atten * .8 + .2;		//inspired by half lambert
					float3 diffuse = albedo * _LightColor0.rgb*i.diffuse*atten;
					return fixed4(ambient + diffuse	,1);
				}
				ENDCG
			}
			
			Pass
			{
				Tags{"LightMode" = "ShadowCaster"}
				CGPROGRAM
				#pragma vertex vertshadow
				#pragma fragment fragshadow
				#pragma multi_compile_shadowcaster

				struct v2fs
				{
					float4 pos:SV_POSITION;
				};

				float _VerticalWaveParam;
				float _HorizontalWaveParam;
				float _WaveSpeed;
			v2fs vertshadow(appdata_base v)
			{
				v2fs o;
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float wave = v.vertex.y*sin(_Time.y*_WaveSpeed);
				 worldPos += float3(wave*_HorizontalWaveParam, 0, wave*_VerticalWaveParam) / 100;
				 o.pos = UnityWorldToClipPos(worldPos);
				return o;
			}

			fixed4 fragshadow(v2fs i) :SV_TARGET
			{
				SHADOW_CASTER_FRAGMENT(i);
			}
				ENDCG
			}
		}
}
