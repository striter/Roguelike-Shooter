Shader "Hidden/PostEffect/PE_DepthSSAO"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
		{
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _NoiseTex;
			sampler2D _CameraDepthTexture;
			float4 _SampleSphere[32];
			int _SampleCount;
			float _Strength;
			float _FallOff;
			float4 _AOColor;
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata_img v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			float3 normal_from_depth(float2 texcoords) {

				const float2 offset1 = float2(0.0, 0.001);
				const float2 offset2 = float2(0.001, 0.0);

				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, texcoords);
				float depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, texcoords + offset1);
				float depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, texcoords + offset2);
				
				float3 p1 = float3(offset1, depth1 - depth);
				float3 p2 = float3(offset2, depth2 - depth);
				return -normalize(cross(p1, p2));
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float3 normal = normal_from_depth( i.uv);
				float3 random = tex2D(_NoiseTex, i.uv*10).rgb;
				float2 uv = i.uv;
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				float occlusion = 0;
				for (int i = 0; i < _SampleCount; i++) {
					float3 ray =  _SampleSphere[i]*random;
					float2 occ_depth_uv = saturate(uv + sign(dot(ray, normal)) * ray.xy * _MainTex_TexelSize);
					float depthOffset = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, occ_depth_uv) - depth;
					occlusion +=step(depthOffset, _FallOff) *lerp(-1,1,  smoothstep(-_FallOff, _FallOff, depthOffset));
				}
				occlusion = saturate(occlusion / _SampleCount * _Strength);
				return lerp(tex2D(_MainTex, uv), _AOColor, occlusion);
			}
			ENDCG
		}
	}
}
