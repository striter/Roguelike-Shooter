Shader "Hidden/PostEffect/PE_DepthSSAO"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("Noise",2D) = "white"{}
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
			sampler2D _NoiseTex;
			sampler2D _CameraDepthTexture;
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
				const float total_strength = 1.0;
				const float base = 0.2;

				const float area = 0.01;
				const float falloff = 0.000001;

				const float radius = 0.0003;

				const int samples = 16;
				float3 sample_sphere[samples] = {
					float3(0.5381, 0.1856,-0.4319), float3(0.1379, 0.2486, 0.4430),
					float3(0.3371, 0.5679,-0.0057), float3(-0.6999,-0.0451,-0.0019),
					float3(0.0689,-0.1598,-0.8547), float3(0.0560, 0.0069,-0.1843),
					float3(-0.0146, 0.1402, 0.0762), float3(0.0100,-0.1924,-0.0344),
					float3(-0.3577,-0.5301,-0.4358), float3(-0.3169, 0.1063, 0.0158),
					float3(0.0103,-0.5869, 0.0046), float3(-0.0897,-0.4940, 0.3287),
					float3(0.7119,-0.0154,-0.0918), float3(-0.0533, 0.0596,-0.5411),
					float3(0.0352,-0.0631, 0.5460), float3(-0.4776, 0.2847,-0.0271)
				};

				float3 random = normalize(tex2D(_NoiseTex, i.uv* 4.0).rgb);

				float depth = tex2D(_CameraDepthTexture, i.uv).r;

				float3 position = float3(i.uv, depth);
				float3 normal = normal_from_depth(depth, i.uv);

				float radius_depth = radius / depth;
				float occlusion = 0;
				fixed3 col = tex2D(_MainTex, i.uv).rgb;
				for (int i = 0; i < samples; i++) {

					float3 ray = radius_depth * reflect(sample_sphere[i], random);
					float3 hemi_ray = position + sign(dot(ray, normal)) * ray;

					float occ_depth = tex2D(_CameraDepthTexture, saturate(hemi_ray.xy)).r;
					float difference = depth - occ_depth;

					occlusion += step(falloff, difference) * (1.0 - smoothstep(falloff, area, difference));
				}
				float ao = 1-total_strength * occlusion * (1.0 / samples);
				
				
				return lerp( fixed4(col,1), fixed4(0, 0, 0, 1),pow(ao,5));
			}
			ENDCG
		}
	}
}
