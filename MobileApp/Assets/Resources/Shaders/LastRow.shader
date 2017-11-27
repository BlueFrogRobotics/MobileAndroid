﻿Shader "Unlit/LastRow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_KernelHalfSize ("KernelHalfSize", Float) = 25
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull off //duty

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _KernelHalfSize;
            float4 _MainTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 sum = float3(0, 0, 0);

				for(int j = -_KernelHalfSize; j <= _KernelHalfSize; j++)
				{
					sum += tex2D(_MainTex, float2(i.uv.x + _MainTex_TexelSize.x * j, 1)).rgb;
				}

				return float4(sum / (2 * _KernelHalfSize + 1), 1.0);
			}
			ENDCG
		}
	}
}
