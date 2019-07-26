﻿Shader "Game/Realistic/Diffuse_Texture_DetailMask"
{
	Properties
	{
		_MainTex(" Mask Tex",2D) = "white"{}
		_TexScale("Mask Scale",float) = 8
		_SubMultiply("Detail Multiply",Range(0,1))=.5
		_Lambert("Lambert Param",Range(0,1))=.5
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

			sampler2D _MainTex;
			float _TexScale;
			float _Lambert;
			v2f vert (appdata v)
			{
				v2f o;
				o.uv  =v.uv;
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
				float2 worldUV = float2(i.worldPos.x + i.worldPos.y,i.worldPos.z+i.worldPos.y)/ _TexScale;
				float3 albedo = tex2D(_MainTex, worldUV);
				UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
				fixed3 ambient = albedo*UNITY_LIGHTMODEL_AMBIENT.xyz;
				atten = atten * _Lambert + (1- _Lambert);
				float3 diffuse = albedo* _LightColor0.rgb*i.diffuse*atten;
				return fixed4(ambient+diffuse	,1);
			}
			ENDCG
		}

		USEPASS "Game/Common/Diffuse_Texture_Normalmap/SHADOWCASTER"
	}

}
