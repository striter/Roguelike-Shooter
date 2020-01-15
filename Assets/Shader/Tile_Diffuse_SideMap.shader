Shader "Game/Special/Tile_Diffuse_SideMap"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Top Tex 1",2D) = "white"{}
		[NoScaleOffset]_MainTex2("Top Tex 2",2D) = "white"{}
		[NoScaleOffset]_MainTex3("Top Tex 3",2D) = "white"{}
		[NoScaleOffset]_MainTex4("Top Tex 4",2D) = "white"{} 
		[NoScaleOffset]_SideTex("Side Tex",2D)="white"{}
		[KeywordEnum(Tex1,Tex2,Tex3,Tex4)]_TexSelection("Tex Selection",int)=0
		_Color("Color Tint",Color) = (1,1,1,1)
		_Lambert("Lambert Param",Range(0,1)) = .5
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
				sampler2D _MainTex,_MainTex2,_MainTex3,_MainTex4;
				sampler2D _SideTex;
				float _Lambert;
				UNITY_INSTANCING_BUFFER_START(Props)
					UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
					UNITY_DEFINE_INSTANCED_PROP(int,_TexSelection)
				UNITY_INSTANCING_BUFFER_END(Props)
				fixed4 frag(v2f i) : SV_Target
				{ 
					UNITY_SETUP_INSTANCE_ID(i);
					float3 albedo =UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
					float texSelection = UNITY_ACCESS_INSTANCED_PROP(Props, _TexSelection);
					if (i.subMap)
					{
						albedo *= tex2D(_SideTex, i.uv);
					}
					else
					{
						if (texSelection == 1)
							albedo *= tex2D(_MainTex2, i.uv);
						else if (texSelection == 2)
							albedo *= tex2D(_MainTex3, i.uv);
						else if (texSelection == 3)
							albedo *= tex2D(_MainTex4, i.uv);
						else
							albedo *= tex2D(_MainTex, i.uv);
					}
					UNITY_LIGHT_ATTENUATION(atten, i,i.worldPos)
					fixed3 ambient = albedo * UNITY_LIGHTMODEL_AMBIENT.xyz;
					atten *= saturate(dot(normalize( i.worldNormal), normalize(i.worldLightDir)));
					atten = atten * _Lambert + (1 - _Lambert);
					float3 diffuse = albedo * _LightColor0.rgb*atten;
					return fixed4(ambient + diffuse	,1);
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

			Pass
			{
				NAME "SHADOWCASTER"
				Tags{"LightMode" = "ShadowCaster"}
				CGPROGRAM
				#pragma vertex vertshadow
				#pragma fragment fragshadow
				#pragma multi_compile_instancing
				struct v2fs
				{
					V2F_SHADOW_CASTER;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				v2fs vertshadow(appdata_base v)
				{
					UNITY_SETUP_INSTANCE_ID(v);
					v2fs o;
					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
					return o;
				}

				fixed4 fragshadow(v2fs i) :SV_TARGET
				{
					SHADOW_CASTER_FRAGMENT(i);
				}
				ENDCG
			}
	}
}