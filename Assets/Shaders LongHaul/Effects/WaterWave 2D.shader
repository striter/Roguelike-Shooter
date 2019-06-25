Shader "Game/Effect/WaterWave 2D"
{
	Properties
	{
		_Color("Color Tint",Color)=(0,.15,.115,1)
		_MainTex ("Texture", 2D) = "white" {}
		_WaveMap("Wave Map",2D)="white"{}
		_Cubemap("Cubemap",CUBE)="_Skybox"{}
		_WaveSpeedX("Wave Speed Horizontal",Range(-.1,.1)) = .01
		_WaveSpeedY("Wave Speed Vertical",Range(-.1,.1)) = .01
		_Distortion("Distortion",Range(0,100)) = 10
	}
		SubShader
		{
		Tags { "RenderType" = "Opaque" "Queue" = "Transparent" }
		GrabPass{"_RefractionTex"}
		LOD 100
		CGINCLUDE
		fixed4 _Color;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D _WaveMap;
		float4 _WaveMap_ST;
		samplerCUBE _Cubemap;
		sampler2D _RefractionTex;
		float4 _RefractionTex_TexelSize;
		fixed _WaveSpeedX;
		fixed _WaveSpeedY;
		fixed _Distortion;
		ENDCG
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal:NORMAL;
				float4 tangent:TANGENT;
					float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 scrPos:TEXCOORD2;
				float2 uv : TEXCOORD0;
				float2 uvWave:TEXCOORD1;
				float4 TtoW1:TEXCOORD3;
				float4 TtoW2:TEXCOORD4;
				float4 TtoW3:TEXCOORD5;
			};

			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeGrabScreenPos(o.pos);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvWave = TRANSFORM_TEX(v.uv, _WaveMap);
				float3 worldPos=mul(unity_ObjectToWorld,v.vertex);
				float3 worldNormal=UnityObjectToWorldNormal(v.normal);
				float3 worldTangent=UnityObjectToWorldDir(v.tangent.xyz);
				float3 worldBinormal=cross(worldNormal,worldTangent)*v.tangent.w;
				o.TtoW1 = float4(worldTangent.x,worldBinormal.x,worldNormal.x,worldPos.x);
				o.TtoW2 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.TtoW3 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 worldPos = float3(i.TtoW1.w,i.TtoW2.w,i.TtoW3.w);
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir(worldPos));
				float2 speed = _Time.y*float2(_WaveSpeedX,_WaveSpeedY);

				fixed3 bump1 = UnpackNormal(tex2D(_WaveMap,i.uvWave+speed));
				fixed3 bump2 = UnpackNormal(tex2D(_WaveMap, i.uvWave - speed));
				fixed3 bump = normalize(bump1 + bump2);

				float2 offset = bump.xy*_Distortion*_RefractionTex_TexelSize.xy;
				i.scrPos.xy = offset +i.scrPos.xy;
					fixed3 refractionColor = tex2D(_RefractionTex, i.scrPos.xy / i.scrPos.w).rgb;

				bump = normalize(half3(dot(i.TtoW1.xyz,bump),dot(i.TtoW2.xyz,bump),dot(i.TtoW3.xyz,bump)));

				fixed4 texColor = tex2D(_MainTex, i.uv);

				fixed3 reflectionDir = reflect(-worldViewDir, bump);
				fixed3 reflectionColor = texCUBE(_Cubemap,reflectionDir).rgb*texColor*_Color.rgb;

				fixed fresnel = pow(1 - saturate(dot(worldViewDir, bump)), 4);
				return fixed4(reflectionColor*fresnel+refractionColor*(1-fresnel),1);
			}
			ENDCG
		}
	}
}
