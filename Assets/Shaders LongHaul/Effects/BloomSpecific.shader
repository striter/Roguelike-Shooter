Shader "Effect/BloomSpecific"
{
	Properties
	{
	 _MainTex("Sprite Texture", 2D) = "white" {}
	_Color("_Color",Color)=(1,1,1,1)
	}

	SubShader
	{ 
		Tags {"RenderType" = "BloomTarget" "RenderQueue"="Opaque"}
		Cull Back
		ZTest On
		ZWrite On
		USEPASS "PostEffect/PE_BloomSpecific_Render/BLOOM_TARGET"
	}
}
