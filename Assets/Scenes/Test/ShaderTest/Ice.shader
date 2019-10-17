Shader "Unlit/Ice"
{
	Properties
	{
		_MainTex("MainTex",2D) = "white"{}
		_Color("Color Tint",Color) = (1,1,1,1)
		_IceColor("Ice Color",Color) = (1,1,1,1)
		_Lambert("Lambert",Range(0,1)) = .5
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Lambert;
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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float3 normal = normalize(v.normal);
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				o.rim =1- abs( pow(dot(normal, viewDir), 1));
				o.uv =TRANSFORM_TEX(v.uv,_MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 albedo =tex2D(_MainTex,i.uv)* _Color;
				float4 iceCol = i.rim*_IceColor;
				float opacity = _Lambert + (1 - _Lambert)*i.rim;
				return float4(albedo.rgb+ iceCol, opacity);
			}
			ENDCG
		}
	}
}
