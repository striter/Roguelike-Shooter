Shader "Hidden/PostEffect/PE_FocalDepth"
{
	Properties
	{
		[PreRenderData]_MainTex ("Texture", 2D) = "white" {}
		_BlurTex("Blur Texure",2D)="white"{}
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

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _BlurTex;
			sampler2D _CameraDepthTexture;
			float _FocalDepthStart;
			float _FocalDepthEnd;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainCol = tex2D(_MainTex, i.uv);
				fixed4 blurCol = tex2D(_BlurTex, i.uv);
				float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv));
				
				float focalParam=(depth>_FocalDepthStart&&depth<_FocalDepthEnd)?0:1;

				return lerp(mainCol,blurCol,focalParam);
			}
			ENDCG
		}
	}
}
