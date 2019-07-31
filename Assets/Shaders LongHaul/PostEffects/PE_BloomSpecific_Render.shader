	Shader "Hidden/PostEffect/PE_BloomSpecific_Render"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_Color("_Color",Color) = (1,1,1,1)
		
		_SubTex1("Dissolve Map",2D) = "white"{}
		_SubTex2("Sub Tex 2",2D) = "white"{}
		_Amount1("_Amount1",Range(0,1))=0
		_Amount2("_Amount2",float)=0
		_Amount3("_Amount3",float)=0
		_Color1("_Color1",Color)=(1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType" = "BloomColor" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			name "BLOOM_COLOR"
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
				float2 uv:TEXCOORD1;
			};
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX( v.uv, _MainTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex,i.uv)*_Color*i.color;
				return col;
			}
			ENDCG
		}
	}		

	SubShader
	{
		Tags{ "RenderType" = "BloomParticles" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			name "BLOOM_COLOR"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float4 color:COLOR;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color:TEXCOORD0;
				float2 uv:TEXCOORD1;
			};
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex,i.uv)*_Color*i.color;
				return col;
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags{"RenderType" = "BloomMask" "IgnoreProjectile"= "true" "Queue" = "Transparent"}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal:NORMAL;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv:TEXCOORD0;
				float2 uvMask:TEXCOORD1;
			};

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SubTex1;
			float4 _SubTex1_ST;
			sampler2D _SubTex2;
			float4 _SubTex2_ST;
			float _Amount1;
			float _Amount2;
			float _Amount3;

			v2f vert(appdata v)
			{
				v2f o;
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _SubTex1);
				o.uvMask = TRANSFORM_TEX(v.uv, _SubTex2);
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed dissolve = tex2D(_SubTex1,i.uv.zw).r - _Amount1 - _Amount2;
				clip(dissolve);

				float3 albedo = tex2D(_MainTex,i.uv.xy)* _Color;
				float4 colorMask = tex2D(_SubTex2, i.uvMask);
				if (colorMask.r == 1)
					return fixed4(albedo* abs(sin(_Time.y*_Amount3)), 1);
				else if (colorMask.r > 0)
					return fixed4(albedo, 1);

				return fixed4(0,0,0,1);
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags { "RenderType" = "BloomDissolve" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
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
				float2 uv:TEXCOORD1;
			};
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
				o.uv = TRANSFORM_TEX(v.uv, _SubTex1);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed dissolve = tex2D(_SubTex1,i.uv).r - _Amount1;
				clip(dissolve);
				dissolve = step(dissolve,_Amount2);
				fixed4 col = _Color1 * dissolve;
				return col;
			}
			ENDCG
		}

	}

	SubShader
	{
		Tags { "RenderType" = "Opaque"}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return float4(0,0,0,1);
			}
			ENDCG
		}
	}
}
