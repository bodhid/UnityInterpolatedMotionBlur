Shader "Hidden/BodhiDonselaar/Final"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#define FINALPOW (1.2)
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 noiseUV : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};
			float4 _MainTex_TexelSize, _Noise_TexelSize;
			v2f vert (float4 vertex : POSITION, float2 uv : TEXCOORD0)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.uv = uv;
				o.noiseUV = _MainTex_TexelSize.zw*_Noise_TexelSize.xy*uv;
				return o;
			}
			
			sampler2D _MainTex;
			uniform sampler2D _MotionPixels, _Noise;
			float4 frag (v2f i) : SV_Target
			{
				//return tex2D(_Noise,i.noiseUV);
				//return tex2D(_MainTex,i.uv);
				float2 sampleDistance = tex2D(_MotionPixels,i.uv) / 8.0;
				float4 color = float4(0,0,0,0);
				float noiseValue = tex2D(_Noise, i.noiseUV).r;
				for (int j = 0;j < 7;++j)
				{
					color += tex2D(_MainTex, sampleDistance*(j + noiseValue) + i.uv);
				}
				return color/7;
			}
			ENDCG
		}
	}
}
