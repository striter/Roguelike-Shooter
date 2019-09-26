Shader "Game/Extra/Diffuse_Texture_DetailMask"
{
	Properties
	{
		_MainTex(" Mask Tex",2D) = "white"{}
		_TexScale("Mask Scale",float) = 8
		_SubMultiply("Detail Multiply",Range(0,1))=.5
		_Lambert("Lambert Param",Range(0,1))=.5
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
		Cull Back

		Blend SrcAlpha OneMinusSrcAlpha
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"
		sampler2D _MainTex;
		float _TexScale;
		ENDCG
		Pass		//Base Pass
		{
			Tags { "LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal:NORMAL;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				float diffuse:TEXCOORD2;
				SHADOW_COORDS(3)
			};

			float _Lambert;
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos =mul(unity_ObjectToWorld,v.vertex);
				o.uv = float2(o.worldPos.x + o.worldPos.y, o.worldPos.z + o.worldPos.y) / _TexScale;
				fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject)); //法线方向n
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(o.worldPos));
				o.diffuse = saturate(dot(worldLightDir,worldNormal));
				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 albedo = tex2D(_MainTex, i.uv);
				UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
				fixed3 ambient = albedo*UNITY_LIGHTMODEL_AMBIENT.xyz;
				atten = atten * _Lambert + (1- _Lambert);
				float3 diffuse = albedo* _LightColor0.rgb*i.diffuse*atten;
				return fixed4(ambient+diffuse	,1);
			}
			ENDCG
		}

		USEPASS "Game/Common/Diffuse_Texture/FORWARDADD"
		USEPASS "Game/Common/Diffuse_Texture_Normalmap/SHADOWCASTER"
	}

}
