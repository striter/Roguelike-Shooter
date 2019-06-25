Shader "Game/Effect/BloomSpecific/Unlit"
{
	Properties
	{
	    _MainTex("Sprite Texture", 2D) = "white" {}
	    _Color("_Color",Color)=(1,1,1,1)
	}

	SubShader
	{ 
		Tags {"RenderType" = "BloomCommon" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On
		USEPASS "Hidden/PostEffect/PE_BloomSpecific_Render/BLOOM_COMMON"
	}
}
