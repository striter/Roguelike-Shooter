Shader "PostEffect/PE_ScreenSpaceAmbientOcculusion"
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

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 viewRay:TEXCOORD1;
			};
			#define MAX_SAMPLE_KERNAL_COUNT 32
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _CameraDepthNormalsTexture;
			float4x4 _InverseProjectionMatrix;
			float _DepthBiasValue;
			float4 _SampleKernalArray[MAX_SAMPLE_KERNAL_COUNT];
			float _SampleKernalCount;
			float _AOStrength;
			float _SampleKernalRaidus;

			float3 GetNormal(float2 uv)
			{
				return DecodeViewNormalStereo(tex2D(_CameraDepthNormalsTexture,uv));
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				float4 viewRay = mul(_InverseProjectionMatrix, o.vertex);
				o.viewRay = viewRay.xyz / viewRay.w;
				
				return o;
			}


			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
			float linear01Depth;
			float3 viewNormal;

			float4 cdn = tex2D(_CameraDepthNormalsTexture, i.uv);
			DecodeDepthNormal(cdn, linear01Depth, viewNormal);

			float3 viewPos = linear01Depth * i.viewRay;
			viewNormal = normalize(viewNormal)*float3(1, 1, -1);

			float oc = 0;
			for (int i = 0; i < _SampleKernalCount; i++)
			{

			}
			oc /= _SampleKernalCount;
			oc = max(0, 1 - oc * _AOStrength);
			col *= float4(oc, oc, oc, 1);
			return col;
			}
			ENDCG
		}
	}
}
