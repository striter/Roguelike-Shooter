Shader "Game/Effect/BloomSpecific/Dissolve_Diffuse_Iceland_Mask"
{
	Properties
	{
		_MainTex("Main Tex",2D) = "white"{}
		_Color("Color",Color) = (1,1,1,1)

		_SubTex2("Color Special Mask",2D)="black"{}
		_Amount3("Color Blink Speed",Range(0,10))=2

		_SubTex1("Dissolve Map",2D) = "white"{}
		_Amount1("_Dissolve Progress",Range(0,1)) = 1
		_Amount2("_Dissolve Width",float) = .1
	}
	SubShader
	{
		name "MAIN"
		Tags{"RenderType" = "BloomMask" }
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "AutoLight.cginc"
		#include "Lighting.cginc"

		sampler2D _SubTex1;
		float4 _SubTex1_ST;
		float _Amount1;
		float _Amount2;
		float _Amount3;
		ENDCG

		Pass		//Base Pass
		{
			Tags{"RenderType"="Opaque" "LightMode" = "ForwardBase" "Queue" = "Transparent"}
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
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
				float4 uv:TEXCOORD0;
				float2 uvMask:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
				float diffuse:TEXCOORD3;
				SHADOW_COORDS(4)
			};

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SubTex2;
			float4 _SubTex2_ST;
			v2f vert (appdata v)
			{
				v2f o;
				o.uv.xy = TRANSFORM_TEX( v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _SubTex1);
				o.uvMask = TRANSFORM_TEX(v.uv, _SubTex2);
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
				fixed dissolve = tex2D(_SubTex1,i.uv.zw).r - _Amount1-_Amount2;
				clip(dissolve);

				float3 albedo = tex2D(_MainTex,i.uv.xy)* _Color;
				float4 colorMask = tex2D(_SubTex2, i.uvMask);
				if (colorMask.r == 1)
					return fixed4(albedo* abs(sin(_Time.y*_Amount3)), 1);
				else if (colorMask.r > 0)
					return fixed4(albedo, 1);

				UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
				fixed3 ambient = albedo*UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 diffuse = albedo* _LightColor0.rgb*i.diffuse*atten;
				return fixed4(ambient+diffuse,1);
			}
			ENDCG
		}

		Pass
		{
			Cull Off
			Tags{"LightMode" = "ShadowCaster"}
			CGPROGRAM
			#pragma vertex vertshadow
			#pragma fragment fragshadow

			struct v2fs
			{
				V2F_SHADOW_CASTER;
				float2 uv:TEXCOORD0;
			};
			v2fs vertshadow(appdata_base v)
			{
				v2fs o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				o.uv = TRANSFORM_TEX(v.texcoord, _SubTex1);
				return o;
			}

			fixed4 fragshadow(v2fs i) :SV_TARGET
			{
				fixed dissolve = tex2D(_SubTex1,i.uv).r - _Amount1-_Amount2;
				clip(dissolve);
				SHADOW_CASTER_FRAGMENT(i);
			}
			ENDCG
		}


	}
}
