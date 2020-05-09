﻿
Shader "Game/UI/Sprite/Outline" {   Properties
{
	[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	_Color("Tint", Color) = (1,1,1,1)

	_OutlineColor("Outline Color",Color)=(1,1,1,1)
	_Width("Outline Width",float)=1

	_StencilComp("Stencil Comparison", Float) = 8
	_Stencil("Stencil ID", Float) = 0
	_StencilOp("Stencil Operation", Float) = 0
	_StencilWriteMask("Stencil Write Mask", Float) = 255
	_StencilReadMask("Stencil Read Mask", Float) = 255

	_ColorMask("Color Mask", Float) = 15

	[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
}

SubShader
	{
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

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};

			float4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;

				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _Width;
			float4 _OutlineColor;

			fixed4 frag(v2f IN) : SV_Target
			{
				float outline =
				tex2D(_MainTex,IN.texcoord + float2(1,0)*_MainTex_TexelSize*_Width).a +
				tex2D(_MainTex,IN.texcoord + float2(-1,0)*_MainTex_TexelSize*_Width).a +
				tex2D(_MainTex,IN.texcoord + float2(0,1)*_MainTex_TexelSize*_Width).a +
				tex2D(_MainTex,IN.texcoord + float2(0,-1)*_MainTex_TexelSize*_Width).a;

				float4 color = tex2D(_MainTex, IN.texcoord);
				if (color.a <= .5)
					return _OutlineColor * outline;
				else
					return color*IN.color;
			}
		ENDCG
		}
	}
}
