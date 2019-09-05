﻿Shader "Hidden/PostEffect/PE_BloomSpecific_Render"
{
	//Additional SubShader 
	SubShader
	{
		Tags{"RenderType" = "BloomMask" "IgnoreProjectile" = "true" "Queue" = "Transparent"}
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
		UsePass "Game/Effect/BloomSpecific/Dissolve_Diffuse_Iceland_Mask/MAIN"
	}

	SubShader
	{
		Tags { "RenderType" = "BloomColor"  "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
		UsePass "Game/Effect/BloomSpecific/Color/MAIN"
	}

	SubShader
	{
		Tags{ "RenderType" = "BloomParticlesAdditive" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
		UsePass "Game/Particle/Additive/MAIN"
	}

	SubShader
	{
		Tags{ "RenderType" = "BloomParticlesAlphaBlend" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
		UsePass "Game/Particle/AlphaBlend/MAIN"
	}

	SubShader
	{
		Tags { "RenderType" = "BloomParticlesAdditiveNoiseFlow" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
		UsePass "Game/Particle/Additive_NoiseFlow/MAIN"
	}

	SubShader
	{
		Tags { "RenderType" = "BloomDissolveEdge" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
		UsePass "Game/Effect/BloomSpecific/Bloom_DissolveEdge/MAIN"
	}

	SubShader
	{
		Tags { "RenderType" = "BloomViewDirDraw" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
		UsePass "Game/Effect/BloomSpecific/Color_ViewDirDraw/MAIN"
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue"="Geometry"}
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
