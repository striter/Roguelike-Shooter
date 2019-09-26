Shader "Game/UI/Sprite/Flow" {
	Properties {
		_FlowX("Flow X",Float)=1
		_FlowY("Flow Y",Float)=1

			[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
			_Color("Tint", Color) = (1,1,1,1)

			_StencilComp("Stencil Comparison", Float) = 8
			_Stencil("Stencil ID", Float) = 0
			_StencilOp("Stencil Operation", Float) = 0
			_StencilWriteMask("Stencil Write Mask", Float) = 255
			_StencilReadMask("Stencil Read Mask", Float) = 255
			_ColorMask("Color Mask", Float) = 15

			[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}
	SubShader { 
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
	float4 _Color;
	half _FlowX;
	half _FlowY;

	struct a2f
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};
	struct v2f
	{
		half2 uv  : TEXCOORD0;
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
	};
	v2f vert(a2f i)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(i.vertex);
		o.uv = i.texcoord;
		o.color = i.color*_Color;
		return o;
	}
	fixed4 frag(v2f v) :COLOR
	{
		float4 finalCol = (0,0,0,0);
		if (v.uv.x < _MainTex_TexelSize.z&&v.uv.y<_MainTex_TexelSize.w)
		finalCol = float4(1,1,1,1);
		return finalCol;
	}
	ENDCG
	}
	}
	FallBack "Diffuse"
}
