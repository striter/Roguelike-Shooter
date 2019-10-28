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
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				sampler2D _CameraOpaqueTexture;
				sampler2D _MainTex;
				sampler2D _DistortTex;
				float4 _MainTex_ST;
				float4 _Color;
				float _Opacity;

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
				fixed3 backCol = tex2D(_CameraOpaqueTexture, i.screenPos.xy / i.screenPos.w+tex2D(_DistortTex,i.uv)/20).rgb;
				return float4(lerp(backCol, albedo, _Opacity).rgb, 1);
			}
			ENDCG
		}
	}
}
