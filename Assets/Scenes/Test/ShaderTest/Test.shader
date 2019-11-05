Shader "SimplestInstancedShader"
{
	Properties
	{
		_MainTex("Main Tex",2D) = "white"{}
		_Color("Color", Color) = (1, 1, 1, 1)
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
			CGINCLUDE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			ENDCG

		Pass
		{
			Tags{"LightMode"="ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_instancing
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv:TEXCOORD;
				float3 normal:NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				float diffuse : TEXCOORD2;
				SHADOW_COORDS(3)
			};


			v2f vert(appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				o.uv = v.uv;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject)); //法线方向n
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(o.worldPos));
				o.diffuse = saturate(dot(worldLightDir, worldNormal));
				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 albedo = tex2D(_MainTex,i.uv)*_Color;
				UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
				fixed3 ambient = albedo * UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 diffuse = albedo * _LightColor0.rgb*i.diffuse*atten;
				return fixed4(ambient + diffuse, 1);
			}
			ENDCG
		}			
	}
}