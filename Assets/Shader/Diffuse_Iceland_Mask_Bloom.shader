Shader "Game/Extra/Diffuse_Iceland_Mask_Bloom"
{
	Properties
	{
		_MainTex("Main Tex",2D) = "white"{}
		_Color("Color",Color) = (1,1,1,1)
		_Amount3("Color Blink Speed",Range(0,10))=2
	}
	SubShader
	{
		name "MAIN"
		Tags{"RenderType" = "BloomMask" }
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "AutoLight.cginc"
		#include "Lighting.cginc"

		float _Amount3;
		ENDCG

		Pass		//Base Pass
		{
			Name "MAIN"
			Tags{"RenderType"="Opaque" "LightMode" = "ForwardBase" "Queue" = "Geometry"}
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
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
				float3 worldPos:TEXCOORD2;
				float diffuse:TEXCOORD3;
				SHADOW_COORDS(4)
			};

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Lambert;
			v2f vert (appdata v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX( v.uv, _MainTex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos =mul(unity_ObjectToWorld,v.vertex);
				fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject)); //法线方向n
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(o.worldPos));
				o.diffuse = saturate(dot(worldLightDir,worldNormal));
				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 mainCol = tex2D(_MainTex, i.uv.xy);
				float3 albedo = mainCol.rgb* _Color;
				float colorMask = mainCol.a;
				if (colorMask ==0)
					return fixed4(albedo* abs(sin(_Time.y*_Amount3)), 1);
				else if (colorMask< 1)
					return fixed4(albedo, 1);

				UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
				atten = atten * _Lambert + (1 - _Lambert);
				fixed3 ambient = albedo*UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 diffuse = albedo* _LightColor0.rgb*i.diffuse*atten;
				return fixed4(ambient+diffuse,1);
			}
			ENDCG
		}
			USEPASS "Game/Common/Diffuse_Base/FORWARDADD"
		USEPASS "Game/Common/Diffuse_Base/SHADOWCASTER"
	}
}
