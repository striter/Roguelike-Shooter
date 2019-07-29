﻿Shader "Game/Effect/BloomSpecific/Particles/Addictive"
{
	Properties
	{
		_MainTex("Main Tex",2D) = "white"{}
		_Color("Color",Color) = (1,1,1,1)
	}
	SubShader
	{ 
		Tags{ "RenderType" = "BloomParticles""IgnoreProjector" = "True" "Queue" = "Transparent"}
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
			UsePass "Game/Particle/Addictive/MAIN"
	}
}