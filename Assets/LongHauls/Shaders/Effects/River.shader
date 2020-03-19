Shader "Game/Effect/River"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Color UV TEX",2D) = "white"{}
		 _TexUVScale("Main Tex UV Scale",float)=10
		_Color("Color Tint",Color) = (1,1,1,1)
		_WaveParam("X|Strength Y|Frequency ZW|Direction",Vector)=(1,1,1,1)
		[NoScaleOffset]_DistortTex("Distort Texure",2D) = "white"{}
		_DistortParam("X|Strength Y|Frequency ZW|Direction",Vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "PreviewType"="Plane" }
		Cull Back
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv:TEXCOORD0;
				float4 screenPos:TEXCOORD1;
			};

			
			sampler2D _CameraOpaqueTexture;
			sampler2D _MainTex;
			float _TexUVScale;
			sampler2D _DistortTex;
			float4 _Color;
			float4 _WaveParam;
			float Wave(float3 worldPos)
			{
				float2 direction = _WaveParam.zw;
				return  sin(direction.x* worldPos.x + direction.y*worldPos.z + _Time.y*_WaveParam.y)*_WaveParam.x;
			}
			float4 _DistortParam;
			float2 Distort(float2 uv)
			{
				return tex2D(_DistortTex, uv+_DistortParam.zw*_Time.y*_DistortParam.y).rg*_DistortParam.x;
			}

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				worldPos += float3(0, Wave(worldPos),0);
				o.pos = UnityWorldToClipPos(worldPos);
				o.screenPos= ComputeScreenPos(o.pos);
				o.uv = worldPos.xz/_TexUVScale;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 screenUV = i.screenPos.xy / i.screenPos.w+ Distort(i.uv);
				float4 albedo = lerp(tex2D(_MainTex,i.uv)*_Color, tex2D(_CameraOpaqueTexture, screenUV),.5);
				return albedo;
			}
			ENDCG
		}
	}

}
