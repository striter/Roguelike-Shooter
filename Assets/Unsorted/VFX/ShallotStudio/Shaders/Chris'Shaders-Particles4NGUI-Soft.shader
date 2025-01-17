﻿Shader "Chris'Shaders/Particles4NGUI-Soft" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Overlay" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off ZTest Always
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
	FallBack "Diffuse"
}
