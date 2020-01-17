Shader "Game/Common/Diffuse_Base"
{
	Properties
	{
		_MainTex("Color UV TEX",2D) = "white"{}
		_Color("Color Tint",Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }

			CGINCLUDE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv:TEXCOORD0;
				float3 normal:NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				float3 worldNormal:TEXCOORD2;
				float3 worldLightDir:TEXCOORD3;
				SHADOW_COORDS(4)
					UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o); 
				o.uv = v.uv;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.worldLightDir = UnityWorldSpaceLightDir(o.worldPos);
				TRANSFER_SHADOW(o);
				return o;
			}
			ENDCG

			Pass
			{
				NAME "FORWARDBASE"
				Tags{"LightMode" = "ForwardBase"}
				Cull Back
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma multi_compile_instancing
				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Lambert;
				fixed4 frag(v2f i) : SV_Target
				{ 
					UNITY_SETUP_INSTANCE_ID(i);
					float3 albedo = tex2D(_MainTex,i.uv)*UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
					UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
					fixed3 ambient = albedo * UNITY_LIGHTMODEL_AMBIENT.xyz;
					atten *= saturate(dot(normalize( i.worldNormal), normalize(i.worldLightDir)));
					atten = atten * _Lambert + (1 - _Lambert);
					float3 diffuse = albedo * _LightColor0.rgb*atten;
					return fixed4(ambient + diffuse	,1);
				}
				ENDCG
			}

			Pass
			{
				Name "ForwardAdd"
				Tags{"LightMode" = "ForwardAdd"}
				Blend One One
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragAdd
				#pragma multi_compile_fwdadd_fullshadows
				#pragma multi_compile_instancing
				

				fixed4 fragAdd(v2f i) :SV_TARGET
				{
					fixed3 diffuse = saturate(dot(normalize( i.worldNormal),normalize( i.worldLightDir))) *_LightColor0.rgb;
					UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);
					return fixed4(diffuse * atten,1);
				}
				ENDCG
			}

			Pass
			{
				NAME "SHADOWCASTER"
				Tags{"LightMode" = "ShadowCaster"}
				CGPROGRAM
				#pragma vertex vertshadow
				#pragma fragment fragshadow
				#pragma multi_compile_instancing
				struct v2fs
				{
					V2F_SHADOW_CASTER;
				};

				v2fs vertshadow(appdata_base v)
				{
					UNITY_SETUP_INSTANCE_ID(v);
					v2fs o;
					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
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