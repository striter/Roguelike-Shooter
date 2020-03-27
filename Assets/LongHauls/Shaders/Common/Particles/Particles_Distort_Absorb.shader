Shader "Game/Particle/Distort_CenterAbsorb"
{
	Properties
	{
		_AbsorbStrength("Absorb Strength",Range(0,0.1))=.005
	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent-1" "PreviewType"="Plane"}
		Cull Back Lighting Off ZWrite Off Fog { Color(0,0,0,0) }

		Pass
		{		
			name "Main"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPos:TEXCOORD0;
				float4 centerScreenPos:TEXCOORD1;
			};
			sampler2D _CameraOpaqueTexture;
			float _AbsorbStrength;
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.centerScreenPos = ComputeScreenPos(UnityObjectToClipPos(float4(0,0,0,0)));
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float2 centerScreenUV = i.centerScreenPos.xy / i.centerScreenPos.w;
				float2 dir = screenUV - centerScreenUV;
				float2 distort = normalize(dir)*(1 - length(dir))*_AbsorbStrength;

				fixed4 col = tex2D(_CameraOpaqueTexture,screenUV+ distort);
				return col;
			}
			ENDCG
		}
	}
}
