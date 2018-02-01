Shader "Unlit/Watermark"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Rect("Rect", Vector) = (0, 0, 0.1, 0.1)
		_Alpha("Alpha", Range (0.0, 1.0)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Rect;
			half _Alpha;
			uniform float4 _MainTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.uv.x -= _Rect.x;
				o.uv.y -= _Rect.y;

				o.uv.x *= 1 / _Rect.z;
				o.uv.y *= 1 / _Rect.w;
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				clip(i.uv.x);
				clip(-i.uv.x + 1);
				clip(i.uv.y);
				clip(-i.uv.y + 1);

				fixed4 col = tex2D(_MainTex, i.uv);

				col.a *= _Alpha;

				return col;
			}
			ENDCG
		}
	}
}
