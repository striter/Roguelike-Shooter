Shader "Unlit/Ice"
{
	Properties
	{
		_MainTex("MainTex",2D) = "white"{}
		_Color("Color Tint",Color) = (1,1,1,1)
		_IceColor("Ice Color",Color) = (1,1,1,1)
		_OpacityMultiple("Opacity Multiple",float) = 1
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
			float4 _MainTex_ST;
			float4 _Color;
			float _OpacityMultiple;
			float4 _IceColor;
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
				float rim : TEXCOORD1;
				float4 screenPos:TEXCOORD2;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float3 normal = normalize(v.normal);
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				o.rim =1- abs( pow(dot(normal, viewDir), 1));
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 background = tex2D(_CameraOpaqueTexture,i.screenPos.xy / i.screenPos.w);

				fixed4 albedo =tex2D(_MainTex,i.uv)* _Color;
				float4 iceCol = i.rim*_IceColor;
				float opacity = i.rim * _OpacityMultiple;
				return float4(background.rgb+ iceCol, opacity);
			}
			ENDCG
		}
	}
}
