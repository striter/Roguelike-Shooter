﻿Shader "Game/LowPoly_TexColor_Diffuse"
{
	Properties
	{
		_MainTex("Main Tex",2D) = "white"{}
		_Color("Color",Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"  "LightMode"="ForwardBase"}
				Cull Back

			CGINCLUDE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			ENDCG
		Pass		//Base Pass
		{
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
				float diffuse:TEXCOORD2;
				SHADOW_COORDS(3)
			};

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			v2f vert (appdata v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX( v.uv, _MainTex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos =mul(unity_ObjectToWorld,v.vertex);
				fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject)); //法线方向n
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(o.worldPos));
				o.diffuse = saturate(dot(worldLightDir,worldNormal));
				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 albedo = tex2D(_MainTex,i.uv)* _Color;
				UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
				fixed3 ambient = albedo*UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 diffuse = albedo* _LightColor0.rgb*i.diffuse*atten;
				return fixed4(ambient+diffuse,1);
			}
			ENDCG
		}

		USEPASS "Common/DefaultStandard/SHADOWCASTER"
	}
}
