Shader "Hidden/PostEffect/PE_DepthGodRay"
{
	Properties
	{
		[PreRenderData]_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma shader_feature QUALITY_NORMAL QUALITY_MEDIUM QUALITY_HIGH
			#include "UnityCG.cginc"


#if defined(QUALITY_HIGH)
#define SAMPLE 32
#elif defined(QUALITY_MEDIUM)
#define SAMPLE 16
#else 
#define SAMPLE 8
#endif

			float _BaseAttenuation;
			float _Attenuation;
			float _LightColor;
			float4 _LightScreenPos;
			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			float2 random2(float2 p) {
				return frac(sin(float2(
					dot(p, float3(114.5, 141.9, 198.10)),
					dot(p, float3(364.3, 648.8, 946.4))
					)) * 643.1);
			}

			fixed4 frag (v2f_img i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float2 delta = _LightScreenPos.xy - i.uv;
				float invDelta = 1.0 / SAMPLE;
				float lightWeight = 0;
				float at = _BaseAttenuation;
				float attenuation = _Attenuation;
				// 加一个抖动值
				float jitter = random2(i.uv);
				// 循环采样，深度不为0则可认为是光源前的遮挡物
				for (int idx = 0; idx < SAMPLE; idx++)
				{
					float2 uv = i.uv + clamp(SAMPLE - idx - jitter, 0, SAMPLE) * invDelta * delta;
					float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
					depth = Linear01Depth(depth);
					if (depth == 1) {
						lightWeight += at;
					}
					at *= attenuation;
				}
				col.rgb += lightWeight * _LightColor;
				col.r = at;
				return col;
			}
			ENDCG
		}
	}
}
