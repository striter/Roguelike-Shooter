Shader "Game/Effect/BloomSpecific/Unlit"
{
	Properties
	{
	    _MainTex("Texture", 2D) = "white" {}
	    _Color("_Color",Color)=(1,1,1,1)
	}

	SubShader
	{ 
		Tags {"RenderType" = "BloomColor" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Cull Back Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
		Blend SrcAlpha One

		Cull Back
		USEPASS "Hidden/PostEffect/PE_BloomSpecific_Render/BLOOM_COLOR"
	}
}
