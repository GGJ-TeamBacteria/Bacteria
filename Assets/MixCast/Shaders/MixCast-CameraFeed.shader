/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
Shader "MixCast/Camera Feed" {

	Properties{
		_Color("Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex("Base(RGB)", 2D) = "white"{}

		_KeyingFactor("Keying Factor", Range(0,1)) = 1
		_ColorExponent("Exponent", Float) = 1
	}

		SubShader{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "Opaque" }

		//ColorMask RGB
		Lighting Off
		//AlphaTest Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		CGPROGRAM

#pragma target 3.0
#pragma exclude_renderers d3d9

#pragma multi_compile BG_REMOVAL_NONE BG_REMOVAL_CHROMA BG_REMOVAL_STATIC
#pragma multi_compile DEPTH_OFF DEPTH_ON
#pragma multi_compile LIGHTING_OFF LIGHTING_FLAT
#pragma multi_compile CROP_OFF CROP_PLAYER

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "BPR_ShaderHelpers.cginc"
#include "Includes/MixCastCropping.cginc"
#include "Includes/MixCastBGRemoval.cginc"
#include "Includes/MixCastLighting.cginc"

	fixed4 _Color;
	sampler2D _MainTex;
	float4 _TextureTransform;

	float _CamNear;
	float _CamFar;
	float4x4 _CamProjection;
	float4x4 _WorldToCam;
	float4x4 _CamToWorld;
	fixed _PlayerDist;
#ifdef DEPTH_ON	
	fixed _GroundHeight;
#endif

	float _KeyingFactor;
	float _ColorExponent = 1;

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 pos : SV_POSITION;
		float3 worldNormal : TEXCOORD2;
	};

	v2f vert(appdata_full v)
	{
		v2f o;

		o.uv = v.texcoord;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.worldNormal = -UNITY_MATRIX_IT_MV[2].xyz;

		return o;
	}

	struct frag_output {
		float4 col:COLOR;
#ifdef DEPTH_ON
		float dep : DEPTH;
#endif
	};

	frag_output frag(v2f i) {
		frag_output o;

		float2 uvs = i.uv * _TextureTransform.xy + _TextureTransform.zw;
		clip(0.5 - abs(uvs.x - 0.5));	//instead of clamping the texture, just cut it off at the edges

		float playerDepth = (_PlayerDist - _CamNear) / (_CamFar - _CamNear);    //Calculate frustum-relative distance
		float3 worldPos = CalculateWorldPosition(float3(i.uv.x, i.uv.y, playerDepth), _CamNear, _CamFar, _CamProjection, _CamToWorld);

		float3 inputRGB = tex2D(_MainTex, uvs).rgb;

		float outputAlpha = 1;

		ApplyCropping(worldPos, _WorldToCam, _CamProjection, _CamToWorld, outputAlpha);
		ApplyBackgroundRemoval(inputRGB, uvs, outputAlpha);

		outputAlpha = lerp(1, outputAlpha, _KeyingFactor);
		clip(outputAlpha - 0.05);

		//Insert pre-lighting modifiers here
		

		//Correct values based on linear/gamma colors
		inputRGB = pow(inputRGB, _ColorExponent);
		_Color.rgb = pow(_Color.rgb, 1.0 / _ColorExponent);

		inputRGB = ApplyLighting(inputRGB, worldPos, i.worldNormal);

		//Insert post-lighting modifiers here

		//Assemble final output
		o.col = float4(2.0 * _Color.rgb * inputRGB, outputAlpha * _Color.a);

#ifdef DEPTH_ON
		o.dep = Buffer01Depth(saturate(playerDepth));
#endif

		return o;
	}

	
	ENDCG
	}
	}
}