//======= Copyright (c) Valve Corporation, All rights reserved. ===============
Shader "Hidden/BPR/AlphaOut" {
	Properties{ _MainTex("Base (RGB)", 2D) = "white" {} }

	SubShader {
		Pass{
			ZTest Always Cull Off ZWrite Off
			Fog{ Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = v.texcoord;
				return o;
			}

			float4 frag(v2f i) : COLOR{
				float4 color = tex2D(_MainTex, i.tex);
				return float4(color.a, color.a, color.a, color.a);
			}
			ENDCG
		}
	}
}
