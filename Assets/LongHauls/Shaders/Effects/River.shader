Shader "Game/Effect/River"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Color UV TEX",2D) = "white"{}
		_TexUVScale("Main Tex UV Scale",float)=10
		_Color("Color Tint",Color) = (1,1,1,1)
		[NoScaleOffset]_DistortTex("Distort Texure",2D) = "white"{}
		_SpecularRange("Specular Range",Range(.95,1)) = 1
		_WaveParam("Wave: X|Strength Y|Frequency ZW|Direction",Vector) = (1,1,1,1)
		_DistortParam("Distort: X|Refraction Distort Y|Frequency Z|Specular Distort",Vector) = (1,1,1,1)
		_FresnelParam("Fresnel: X | Base Y| Max Z| Scale ",Vector)=(1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"  }
		Cull Back
		CGINCLUDE
		#include "UnityCG.cginc"
			#include "Lighting.cginc"
		ENDCG
		Pass		//Base Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing


			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal:NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				float4 screenPos:TEXCOORD2;
				float3 worldNormal:TEXCOORD3;
			};

			
			sampler2D _CameraOpaqueTexture;
			sampler2D _MainTex;
			float _SpecularRange;
			float _TexUVScale;
			sampler2D _DistortTex;
			float4 _Color;
			float4 _WaveParam;
			float4 _DistortParam;
			float Wave(float3 worldPos)
			{
				return  sin(worldPos.xz* _WaveParam.zw +_Time.y*_WaveParam.y)*_WaveParam.x;
			}
			float2 Distort(float2 uv)
			{
				return tex2D(_DistortTex, uv+ _WaveParam.zw *_Time.y*_DistortParam.y).rg*_DistortParam.x;
			}

			float4 _FresnelParam;
			float Fresnel(float3 normal, float3 viewDir) {
				return lerp( _FresnelParam.x ,_FresnelParam.y, saturate(  _FresnelParam.z* (1 - dot(normal, viewDir))));
			}

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldPos += float3(0, Wave(o.worldPos),0);
				o.uv = o.worldPos.xz / _TexUVScale;
				o.pos = UnityWorldToClipPos(o.worldPos);
				o.screenPos= ComputeScreenPos(o.pos);
				o.worldNormal =UnityObjectToWorldNormal(v.normal) ;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 distort = Distort(i.uv);

				float3 normal = normalize(i.worldNormal);
				float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				
				float fresnel = Fresnel(normal, viewDir);
				
				float specular =  dot(normalize(normal), normalize(viewDir + lightDir));
				specular = smoothstep(_SpecularRange,1, specular-distort.x*_DistortParam.z);
				float4 specularColor = float4(_LightColor0.rgb*specular, specular);

				float4 albedo = float4((tex2D(_MainTex, i.uv+distort)*_Color).rgb,1);
				return lerp(tex2D(_CameraOpaqueTexture, i.screenPos.xy / i.screenPos.w + distort), albedo, fresnel)+ specularColor;
			}
			ENDCG
		}
	}

}
