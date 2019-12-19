Shader "Game/Effect/BloomSpecific/Bloom_Dissolve"
{
	Properties
	{
		_MainTex("Main Tex",2D) = "white"{}
		_Color("Color",Color) = (1,1,1,1)
		_DissolveColor("_Dissolve Color",Color) = (1,1,1,1)


		_NoiseTex("Dissolve Map",2D) = "white"{}
		_DissolveAmount("_Dissolve Amount",Range(0,1)) = 1
		_DissolveWidth("_Dissolve Width",float) = .1
		_DissolveScale("_Dissolve Scale",Range(0,1))=1

	}
	SubShader
	{
		Tags{"RenderType" = "BloomDissolveEdge" "Queue" = "Transparent"}
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "AutoLight.cginc"
		#include "Lighting.cginc"
		sampler2D _NoiseTex;
		float _DissolveAmount;
		float _DissolveWidth;
		float4 _Color;
		float4 _DissolveColor;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float _DissolveScale;
		float2 GetDissolveUV(float3 vertex)
		{
			float2 uv = float2(vertex.x, vertex.z) + vertex.y*.7;
			return uv* _DissolveScale;
		}
		ENDCG

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
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
				float3 worldPos:TEXCOORD1;
				float diffuse:TEXCOORD2;
				SHADOW_COORDS(3)
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.uv.xy =  TRANSFORM_TEX( v.uv, _MainTex);
				o.uv.zw = GetDissolveUV(v.vertex);
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
				fixed dissolve = tex2D(_NoiseTex,i.uv.zw).r - _DissolveAmount-_DissolveWidth;
				clip(dissolve);

				float4 albedo = tex2D(_MainTex,i.uv.xy)* _Color;
				UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
				fixed3 ambient = albedo*UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 diffuse = albedo* _LightColor0.rgb*i.diffuse*atten;
				return fixed4(ambient+diffuse,albedo.a);
			}
			ENDCG
		}

		Pass
		{
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
				o.uv = GetDissolveUV(v.vertex);
				return o;
			}

			fixed4 fragshadow(v2fs i) :SV_TARGET
			{
				fixed dissolve = tex2D(_NoiseTex,i.uv).r - _DissolveAmount-_DissolveWidth;
				clip(dissolve);
				SHADOW_CASTER_FRAGMENT(i);
			}
			ENDCG
		}


		Pass
		{
		NAME "EDGE"
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
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.uv = GetDissolveUV(v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed dissolve = tex2D(_NoiseTex,i.uv).r - _DissolveAmount;
				clip(dissolve);
				clip( _DissolveWidth -dissolve);
				return _DissolveColor;
			}
			ENDCG
		}

	}
}
