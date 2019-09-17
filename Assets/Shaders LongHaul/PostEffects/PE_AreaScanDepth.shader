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

			float4 _FrustumCornersRayBL;
			float4 _FrustumCornersRayBR;
			float4 _FrustumCornersRayTL;
			float4 _FrustumCornersRayTR;
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
				float3 interpolatedRay:TEXCOORD2;
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
				bool right = o.uv.x > .5;
				bool top = o.uv.y > .5;
				o.interpolatedRay = right ? (top ? _FrustumCornersRayTR : _FrustumCornersRayBR) : (top ? _FrustumCornersRayTL : _FrustumCornersRayBL);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float linearDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv_depth));
				float3 worldPos = _WorldSpaceCameraPos+ i.interpolatedRay*linearDepth;
				float offsetLength = length(worldPos - _ScanOrigin.xyz);
				float scanValue = .0f;
				float4 texColor = tex2D(_ScanTex, i.uv*_ScanTexScale);
				if (offsetLength<_ScanElapse&&offsetLength>_ScanElapse - _ScanWidth)
					scanValue = _ScanLerp*texColor.r;
				return fixed4(lerp(tex2D(_MainTex,i.uv).rgb, _ScanColor.rgb, scanValue),1);
			}
			ENDCG
		}
	}
}
