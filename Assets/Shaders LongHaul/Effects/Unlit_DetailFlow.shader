Shader "Game/Effect/Unlit_DetailFlow"
{
	Properties
	{
	    _MainTex("Sprite Texture", 2D) = "white" {}
	    _Color("_Color",Color)=(1,1,1,1)
		_SubTex1("Detail Tex",2D) = "white"{}
		_Amount1("Horizontal Flow",Range(0,5))=1
		_Amount2("Vertical Flow",Range(0,5))=1
	}

	SubShader
	{ 
		Tags {"RenderType" = "Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Cull Back Lighting Off ZWrite Off Fog { Color(0,0,0,0) }

		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			name "BLOOM_DETAILFLOW"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float4 color    : COLOR;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color    : TEXCOORD0;
				float4 uv:TEXCOORD1;
			};
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			sampler2D _SubTex1;
			float4 _SubTex1_ST;
			float _Amount1;
			float _Amount2;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _SubTex1);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex,i.uv.xy)*_Color*i.color;
				fixed4 colDetail = tex2D(_SubTex1, i.uv.zw+float2(_Amount1,_Amount2)*_Time.y);
				return col*colDetail;
			}
			ENDCG
		}
	}
}
