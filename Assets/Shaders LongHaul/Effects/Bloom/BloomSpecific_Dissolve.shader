Shader "Effect/BloomSpecific/Dissolve"
{
	Properties
	{
	    _MainTex("Texture", 2D) = "white" {}
	    _Color("_Color",Color)=(1,1,1,1)

		_SubTex1("Dissolve Map",2D) = "white"{}
		_Amount1("_Dissolve Progress",Range(0,1))=1
		_Amount2("_Dissolve Width",float)=.1
		_Color1("_Dissolve Color",Color)=(1,1,1,1)
	}

	SubShader
	{
		Tags { "RenderType" = "BloomDissolve" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			name "BLOOM_DISSOLVE"
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
				float4 uv:TEXCOORD1;
			};
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SubTex1;
			float4 _SubTex1_ST;
			float4 _Color;
			float _Amount1;
			float _Amount2;
			float4 _Color1;
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _SubTex1);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed dissolve = tex2D(_SubTex1,i.uv.zw).r-_Amount1;
				clip( dissolve);
				dissolve = step(dissolve,_Amount2);
				fixed4 col =_Color1* dissolve+ tex2D(_MainTex, i.uv.xy)*_Color*(1- dissolve);
				return col;
			}
			ENDCG
		}
	}
}
