Shader "Hidden/BodhiDonselaar/MotionBlur"
{
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 noiseUV : TEXCOORD1;
			};
			float4 _WPOS_TexelSize, _Noise_TexelSize;
			v2f vert (float4 vertex : POSITION, float2 uv : TEXCOORD0)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.uv = uv;
				o.noiseUV = _WPOS_TexelSize.zw*_Noise_TexelSize.xy*uv;
				return o;
			}
			uniform sampler2D _WPOS, _WPOS_PREV, _Noise;
			uniform float4x4 _World2Screen;
			float4 frag (v2f i) : SV_Target
			{
				float3 current = tex2D(_WPOS, i.uv).xyz;
				float3 previous = tex2D(_WPOS_PREV, i.uv).xyz;
				float4 screenPos = mul(_World2Screen,float4(previous,1) );
				float2 previousUv = (screenPos.xy / screenPos.w)*0.5 + 0.5;
				return float4(previousUv - i.uv,1,1);
			}
			ENDCG
		}
	}
}
