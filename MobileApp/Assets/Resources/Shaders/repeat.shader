Shader "Unlit/repeat"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_StepSize ("StepSize", Float) = 0.005
		_StepCnt ("StepCnt", Float) = 10
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
			float _StepSize;
            float _StepCnt;

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
				float sumR = 0;
				float sumG = 0;
				float sumB = 0;
				float cnt = 0;

				for(int j = -_StepCnt; j <= _StepCnt; j++)
				{
					float y = i.uv.y + j * _StepSize;
					if(y >= 0 && y <= 1)
					{
						float4 c = tex2D(_MainTex, float2(0, y));
						sumR += c.r;
						sumG += c.g;
						sumB += c.b;
						cnt++;
					}
				}

				return float4(sumR / cnt, sumG / cnt, sumB / cnt, 1.0);
			}
			ENDCG
		}
	}
}
