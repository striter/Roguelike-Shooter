Shader "Hidden/PostEffect/PE_AreaScanDepth"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ScanTex(" Scan Texture",2D) = "white"{}
		_ScanTexScale("Scan Tex Scale",float)=15
		_ScanColor("Scan Color",Color)=(1,1,1,1)
		_ScanElapse("Scan Elapse",float)=.5
		_ScanLerp("Scan Lerp",float)=1
		_ScanOrigin("Scan Origin",Vector)=(1,1,1,1)
		_ScanWidth("Scan Width",float)=.5
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

			float4x4 _FrustumCornersRay;
			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			sampler2D _CameraDepthTexture;
			sampler2D _ScanTex;
			float4 _ScanColor;
			float _ScanLerp;
			float _ScanElapse;
			float4 _ScanOrigin;
			float _ScanWidth;
			float _ScanTexScale;


			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				half2 uv_depth:TEXCOORD1;
				float4 interpolatedRay:TEXCOORD2;
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
				int index = 0;
				if (o.uv.x > .5)
					index += 1;
				if (o.uv.y > .5)
					index += 2;
				o.interpolatedRay = _FrustumCornersRay[index];
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float linearDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv_depth));
				float3 worldPos = _WorldSpaceCameraPos+ i.interpolatedRay.xyz*linearDepth;
				float offsetLength = length(worldPos - _ScanOrigin.xyz);
				float scanValue = .0f;
				if (offsetLength<_ScanElapse&&offsetLength>_ScanElapse - _ScanWidth)
					scanValue = _ScanLerp;
				return fixed4(lerp(tex2D(_MainTex,i.uv).rgb, (tex2D(_ScanTex, i.uv*_ScanTexScale)*_ScanColor).rgb, scanValue),1);
			}
			ENDCG
		}
	}
}
