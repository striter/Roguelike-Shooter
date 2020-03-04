Shader "Game/Common/Diffuse_Texture_Transparent"
{
	Properties
	{
		_MainTex("Color UV TEX",2D) = "white"{}
		_Color("Color Tint",Color) = (1,1,1,1)
		_Alpha("Alpha",Range(0,1))=.5
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
			CGINCLUDE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			ENDCG
		Pass		//Base Pass
		{
			NAME "MAIN"
			Tags{"LightMode" = "ForwardBase"}
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			sampler2D _MainTex;
			float4 _MainTex_ST;
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
			float _Alpha;

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
				float3 albedo = tex2D(_MainTex,i.uv)*_Color;
				UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
				fixed3 ambient = albedo*UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 diffuse = albedo* _LightColor0.rgb*i.diffuse*atten;
				return fixed4(ambient+diffuse	,_Alpha);
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
				struct v2fs
				{
					V2F_SHADOW_CASTER;
				};

				v2fs vertshadow(appdata_base v)
				{
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
