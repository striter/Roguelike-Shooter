Shader "Game/Special/UI/MapFogBlur" {
	Properties{
			_BlurRadius("Blur Radius",Range(0,5)) = 1
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

			_StencilComp("Stencil Comparison", Float) = 8
			_Stencil("Stencil ID", Float) = 0
			_StencilOp("Stencil Operation", Float) = 0
			_StencilWriteMask("Stencil Write Mask", Float) = 255
			_StencilReadMask("Stencil Read Mask", Float) = 255
			_ColorMask("Color Mask", Float) = 15

			[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}
		SubShader{
				Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}
			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}
				LOD 200

				Cull Off
				Lighting Off
				ZWrite Off
				ZTest[unity_GUIZTestMode]
				Blend SrcAlpha OneMinusSrcAlpha
				ColorMask[_ColorMask]

				PASS
			{

			CGPROGRAM
	#pragma target 2.0
	#pragma vertex vert
	#pragma fragment frag
	#pragma multi_compile __ UNITY_UI_ALPHACLIP
	#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _BlurRadius;

		struct a2f
		{
			float4 vertex   : POSITION;
			float4 color    : COLOR;
			float2 texcoord : TEXCOORD0;
		};
		struct v2f
		{
			half2 uv[9]  : TEXCOORD0;
			float4 vertex   : SV_POSITION;
			fixed4 color : COLOR;
		};
		v2f vert(a2f i)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(i.vertex);
			half2 uv = i.texcoord;
			o.uv[0] = uv;
			o.uv[1] = uv + float2(1, 1)*_MainTex_TexelSize.x *_BlurRadius;
			o.uv[2] = uv + float2(1, -1)*_MainTex_TexelSize.x *_BlurRadius;
			o.uv[3] = uv + float2(-1, -1)*_MainTex_TexelSize.x *_BlurRadius;
			o.uv[4] = uv + float2(-1, 1)*_MainTex_TexelSize.x *_BlurRadius;
			o.uv[5] = uv + float2(0, 1)*_MainTex_TexelSize.x *_BlurRadius;
			o.uv[6] = uv + float2(1, 0)*_MainTex_TexelSize.x *_BlurRadius;
			o.uv[7] = uv + float2(0, -1)*_MainTex_TexelSize.x *_BlurRadius;
			o.uv[8] = uv + float2(-1, 0)*_MainTex_TexelSize.x *_BlurRadius;
			o.color = i.color;
			return o;
		}

		float4 frag(v2f i) :SV_TARGET
		{
			float4 albedo = tex2D(_MainTex, i.uv[0]);
			fixed sum = albedo.a;
			sum += tex2D(_MainTex, i.uv[1]).a;
			sum += tex2D(_MainTex, i.uv[2]).a;
			sum += tex2D(_MainTex, i.uv[3]).a;
			sum += tex2D(_MainTex, i.uv[4]).a;
			sum += tex2D(_MainTex, i.uv[5]).a;
			sum += tex2D(_MainTex, i.uv[6]).a;
			sum += tex2D(_MainTex, i.uv[7]).a;
			sum += tex2D(_MainTex, i.uv[8]).a;
			return float4(albedo.rgb, sum/=9);
		}
		ENDCG
		}
		}
}
