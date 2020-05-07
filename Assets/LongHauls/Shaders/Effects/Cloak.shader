Shader "Game/Effect/Cloak"
{
	Properties
	{
		_MainTex("MainTex",2D) = "white"{}
		_DistortTex("DistortTex",2D)="white"{}
		_Color("Color Tint",Color) = (1,1,1,1)
		_Opacity("Opacity Multiple",float) = .7
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			Cull Back
			CGINCLUDE
				#include "Lighting.cginc"
				float _Opacity;
			ENDCG
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				sampler2D _CameraOpaqueTexture;
				sampler2D _MainTex;
				sampler2D _DistortTex;
				float4 _MainTex_ST;
				float4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal:NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 screenPos:TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 albedo = tex2D(_MainTex, i.uv)*_Color;
				fixed3 backCol = tex2D(_CameraOpaqueTexture, i.screenPos.xy / i.screenPos.w+tex2D(_DistortTex,i.uv)/30).rgb;
				return float4(lerp(backCol, albedo, _Opacity).rgb, 1);
			}
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
				sampler3D _DitherMaskLOD;
			struct v2fs
			{
				V2F_SHADOW_CASTER;
				float4 screenPos:TEXCOORD0;
			};

			v2fs ShadowVertex(appdata_base v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				v2fs o;
				o.screenPos = ComputeScreenPos(v.vertex);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
					return o;
			}

			fixed4 ShadowFragment(v2fs i) :SV_TARGET
			{
				float2 vpos = i.screenPos.xy / i.screenPos.w;
				float dither = tex3D(_DitherMaskLOD, float3(vpos*10,_Opacity * 0.9375)).a;
				clip(dither - 0.01);
				SHADOW_CASTER_FRAGMENT(i);
			}
				ENDCG
			}
	}
}
