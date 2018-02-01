// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
Shader "Hidden/BPR/AlphaWrite"
{
	Properties
	{
		_AlphaValue("Alpha", Range(0,1)) = 1
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		ColorMask A

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed _AlphaValue;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(1,1,1,_AlphaValue);
			}
			ENDCG
		}
	}
}
