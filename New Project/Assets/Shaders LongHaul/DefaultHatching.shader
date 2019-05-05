﻿Shader "Common/DefaultHatching"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Outline("Thick of Outline",range(0,0.1)) = 0.02
		_Factor("Factor",range(0,1)) = 0.5
		_OutLineColor("OutLineColor",Color) = (1,1,1,1)
		_TileFactor("Tile Factor",Float)=1
		_Hatch0("Hatch 0",2D)="white"{}
		_Hatch1("Hatch 1",2D) = "white"{}
		_Hatch2("Hatch 2",2D) = "white"{}
		_Hatch3("Hatch 3",2D) = "white"{}
		_Hatch4("Hatch 4",2D) = "white"{}
		_Hatch5("Hatch 5",2D) = "white"{}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"  "Queue"="Geometry"}

		UsePass "Common/DefaultToonDiffuse/OUTLINE"

		Pass
		{
			Tags{"LightMode"="ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
#pragma multi_compile_fwdbase

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _TileFactor;
			sampler2D _Hatch0, _Hatch1, _Hatch2, _Hatch3, _Hatch4, _Hatch5;

			#include "UnityCG.cginc"
#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal:NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed3 hatchWeight012 : TEXCOORD1;
				fixed3 hatchWeight345 : TEXCOORD2;
				float3 worldPos:TEXCOORD3;
				SHADOW_COORDS(4)
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv*_TileFactor;

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				fixed3 worldNormal = normalize( UnityObjectToWorldNormal(v.normal));
				fixed3 worldLightDir = normalize(  UnityWorldSpaceLightDir(o.worldPos));

				fixed diff = max(0, dot(worldLightDir, worldNormal));
				o.hatchWeight012 = fixed3(0, 0, 0);
				o.hatchWeight345 = fixed3(0, 0, 0);

				float hatchFactor = diff * 7.0;

				if (hatchFactor > 6)
				{
				}
				else if (hatchFactor > 5)
				{
					o.hatchWeight012.x = hatchFactor - 5;
				}
				else if (hatchFactor > 4)
				{
					o.hatchWeight012.x = hatchFactor - 4;
					o.hatchWeight012.y = 1 - o.hatchWeight012.x;
				}
				else if (hatchFactor > 3)
				{
					o.hatchWeight012.y = hatchFactor - 3;
					o.hatchWeight012.z = 1 - o.hatchWeight012.y;
				}
				else if (hatchFactor > 2)
				{
					o.hatchWeight012.z = hatchFactor - 2;
					o.hatchWeight345.x = 1 - o.hatchWeight012.z;
				}
				else if(hatchFactor>1)
				{
					o.hatchWeight345.x = hatchFactor - 1;
					o.hatchWeight345.y = 1 - o.hatchWeight345.x;
				}
				else
				{
					o.hatchWeight345.y = hatchFactor;
					o.hatchWeight345.z = 1 - o.hatchWeight345.y;
				}

				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 hatchColor=fixed4(0,0,0,0);
			hatchColor += tex2D(_Hatch0, i.uv)*i.hatchWeight012.x;
			hatchColor +=  tex2D(_Hatch1, i.uv)*i.hatchWeight012.y;
			hatchColor += tex2D(_Hatch2, i.uv)*i.hatchWeight012.z;
			hatchColor += tex2D(_Hatch3, i.uv)*i.hatchWeight345.x;
			hatchColor += tex2D(_Hatch4, i.uv)*i.hatchWeight345.y;
			hatchColor += tex2D(_Hatch5, i.uv)*i.hatchWeight345.z;

			fixed4 whiteCol = fixed4(1, 1, 1, 1)*(1 - i.hatchWeight012.x - i.hatchWeight012.y - i.hatchWeight012.z - i.hatchWeight345.x - i.hatchWeight345.y - i.hatchWeight345.z);
			hatchColor += whiteCol;
			UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

			fixed3 col = tex2D(_MainTex, i.uv).rgb*hatchColor.rgb;

				return fixed4( col,1);
			}
			ENDCG
		}
	}
}
