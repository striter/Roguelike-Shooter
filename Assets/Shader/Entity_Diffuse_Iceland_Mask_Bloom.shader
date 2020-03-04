Shader "Game/Extra/Entity_Diffuse_Iceland_Mask_Bloom"
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
		#include "../LongHauls/Shaders/CommonLightingInclude.cginc"
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
			#pragma multi_compile_instancing

			struct a2fDV
			{
				float4 vertex : POSITION;
				float2 uv:TEXCOORD0;
				float3 normal:NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				float diffuse : TEXCOORD2;
				SHADOW_COORDS(3)
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (a2fDV v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.uv = v.uv;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.diffuse = GetDiffuse(mul(v.normal, (float3x3)unity_WorldToObject), UnityWorldSpaceLightDir(o.worldPos));
				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
			float4 mainCol = tex2D(_MainTex, i.uv.xy);
				float3 albedo =mainCol.rgb*UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
				float colorMask = mainCol.a;
				if (colorMask ==0)
					return fixed4(albedo* abs(sin(_Time.y*_Amount3)), 1);
				else if (colorMask< 1)
					return fixed4(albedo, 1);

				UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
				return fixed4(GetDiffuseBaseColor(albedo, UNITY_LIGHTMODEL_AMBIENT.xyz, _LightColor0.rgb, atten, i.diffuse),1);
			}
			ENDCG
		}
			USEPASS "Game/Common/Diffuse_Base/FORWARDADD"
		USEPASS "Game/Common/Diffuse_Base/SHADOWCASTER"
	}
}
