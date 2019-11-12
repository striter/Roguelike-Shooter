Shader "Hidden/PostEffect/PE_DepthSSAO"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			sampler2D _CameraDepthTexture;
			float4 _SampleSphere[32];
			int _SampleCount;
			float _Strength;
			float _SizeArea;
			float _FallOff;
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				half2 uv_depth:TEXCOORD1;
			};

			v2f vert (appdata_img v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.uv_depth = v.texcoord;
#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv_depth.y = 1 - o.uv_depth.y;
#endif
				return o;
			}

			float3 normal_from_depth(float depth, float2 texcoords) {

				const float2 offset1 = float2(0.0, 0.001);
				const float2 offset2 = float2(0.001, 0.0);

				float depth1 = tex2D(_CameraDepthTexture, texcoords + offset1).r;
				float depth2 = tex2D(_CameraDepthTexture, texcoords + offset2).r;

				float3 p1 = float3(offset1, depth1 - depth);
				float3 p2 = float3(offset2, depth2 - depth);

				float3 normal = cross(p1, p2);
				normal.z = -normal.z;

				return normalize(normal);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				float depth = tex2D(_CameraDepthTexture, i.uv).r;
				float3 position = float3(i.uv, depth);
				float3 normal = normal_from_depth(depth, i.uv);
				float occlusion = 0;
				for (int i = 0; i < _SampleCount; i++) {
					float3 ray = _SampleSphere[i];
					float3 hemi_ray = position + sign(dot(ray, normal)) * ray;
					float occ_depth = tex2D(_CameraDepthTexture, saturate(hemi_ray.xy)).r;
					float difference = depth - occ_depth;
					occlusion += occ_depth==0?1: step(_FallOff, difference) *(1- smoothstep(_FallOff,_SizeArea,difference));
				}
				float ao = 1-occlusion / _SampleCount;
				ao = pow(ao,5)*_Strength;
				return lerp(col,float4(0,0,0,1),ao);
			}
			ENDCG
		}
	}
}
