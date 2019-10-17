Shader "Unlit/Ice"
{
	Properties
	{
		_Color("Color Tint",Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass

		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Color;
			sampler2D _OpaqueGrabBlurTex;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 screenPos:TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 screenCol = tex2D(_OpaqueGrabBlurTex,i.screenPos.xy / i.screenPos.w);
				fixed4 col = screenCol*_Color;
				return col;
			}
			ENDCG
		}
	}
}
