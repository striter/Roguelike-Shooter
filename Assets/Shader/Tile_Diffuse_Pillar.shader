Shader "Game/Special/Tile_Diffuse_Pillar"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Top Tex 1",2D) = "white"{}
		[NoScaleOffset]_SideTex("Side Tex",2D)="white"{}
		_Color("Color Tint",Color) = (1,1,1,1)
		_FogStart("YFog Start",float) = -5
		_FogRange("YFog Range",float)=3
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }

			CGINCLUDE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			struct appdata
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
				float3 worldNormal:TEXCOORD2;
				float3 worldLightDir:TEXCOORD3;
				bool subMap : TEXCOORD4;
				SHADOW_COORDS(5)
					UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o); 
				o.uv = v.uv;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.worldLightDir = UnityWorldSpaceLightDir(o.worldPos);
				o.subMap = abs(dot(v.normal,float3(0,1,0)))<.3;
				TRANSFER_SHADOW(o);
				return o;
			}
			ENDCG

			Pass
			{
				NAME "FORWARDBASE"
				Tags{"LightMode" = "ForwardBase"}
				Cull Back
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma multi_compile_instancing
				sampler2D _MainTex;
				sampler2D _SideTex;
				float _Lambert;
				float _FogRange;
				float _FogStart;
				float4 _SkyColor;
				UNITY_INSTANCING_BUFFER_START(Props)
					UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
				UNITY_INSTANCING_BUFFER_END(Props)
				fixed4 frag(v2f i) : SV_Target
				{ 
					UNITY_SETUP_INSTANCE_ID(i);
					float3 albedo =UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
					if (i.subMap)
						albedo *= tex2D(_SideTex, i.uv);
					else
						albedo *= tex2D(_MainTex, i.uv);


					UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
					fixed3 ambient = albedo * UNITY_LIGHTMODEL_AMBIENT.xyz;
					atten *= saturate(dot(normalize( i.worldNormal), normalize(i.worldLightDir)));
					atten = atten * _Lambert + (1 - _Lambert);
					float3 diffuse = albedo * _LightColor0.rgb*atten;
					float fogParam = smoothstep(_FogStart, _FogStart + _FogRange, i.worldPos.y);
					fogParam = pow(fogParam, 2);
					return fixed4(lerp(_SkyColor, ambient + diffuse,fogParam)	,1);
				}
				ENDCG
			}

			Pass
			{
				Name "ForwardAdd"
				Tags{"LightMode" = "ForwardAdd"}
				Blend One One
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragAdd
				#pragma multi_compile_fwdadd_fullshadows
				#pragma multi_compile_instancing
				

				fixed4 fragAdd(v2f i) :SV_TARGET
				{
					fixed3 diffuse = saturate(dot(normalize( i.worldNormal),normalize( i.worldLightDir))) *_LightColor0.rgb;
					UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);
					return fixed4(diffuse * atten,1);
				}
				ENDCG
			}
	}
}