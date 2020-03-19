Shader "Game/Effect/River"
{
	Properties
	{
		_MainTex("Color UV TEX",2D) = "white"{}
		_Color("Color Tint",Color) = (1,1,1,1)
		_WaveParam("X|Strength Y|Frequency ZW|Direction",Vector)=(1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "PreviewType"="Plane" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		CGINCLUDE
		#include "UnityCG.cginc"
		ENDCG
		Pass		//Base Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal:NORMAL;
				float2 uv:TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			float4 _WaveParam;
			float Wave(float3 worldPos)
			{
				float2 direction = _WaveParam.zw;
				return  sin(direction.x* worldPos.x+ direction.y*worldPos.z+_Time.y*_WaveParam.y)*_WaveParam.x;
			}

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				o.uv  =TRANSFORM_TEX(v.uv, _MainTex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				worldPos += float3(0, Wave(worldPos),0);
				o.pos = UnityWorldToClipPos(worldPos);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 albedo = tex2D(_MainTex,i.uv)*_Color;
				return albedo;
			}
			ENDCG
		}
	}

}
