/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/

//HSV <-> RGB
float3 HUEtoRGB(in float H)
{
	float R = abs(H * 6 - 3) - 1;
	float G = 2 - abs(H * 6 - 2);
	float B = 2 - abs(H * 6 - 4);
	return saturate(float3(R, G, B));
}

float Epsilon = 1e-10;

float3 RGBtoHCV(in float3 RGB)
{
	// Based on work by Sam Hocevar and Emil Persson
	float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0 / 3.0) : float4(RGB.gb, 0.0, -1.0 / 3.0);
	float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
	float C = Q.x - min(Q.w, Q.y);
	float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
	return float3(H, C, Q.x);
}
float3 RGBtoHSV(in float3 RGB)
{
	float3 HCV = RGBtoHCV(RGB);
	float S = HCV.y / (HCV.z + Epsilon);
	return float3(HCV.x, S, HCV.z);
}
float3 HSVtoRGB(in float3 HSV)
{
	float3 RGB = HUEtoRGB(HSV.x);
	return ((RGB - 1) * HSV.y + 1) * HSV.z;
}


float3 RGB_to_HSV(float3 RGB)
{
	return RGBtoHSV(RGB);
}


float3 HSV_to_RGB(float3 HSV)
{
	return HSVtoRGB(HSV);
}


#ifdef BG_REMOVAL_CHROMA
float3 _KeyHsvMid;
float3 _KeyHsvRange;
float3 _KeyHsvFeathering;
float _KeyDesaturateBandWidth;
float _KeyDesaturateFalloffWidth;
#elif BG_REMOVAL_STATIC
sampler2D _KeyMidTex;
sampler2D _KeyRangeTex;
float3 _KeyHsvFeathering;
#endif

//Chroma
float CalculateChromaAlpha(float3 inputHSV, float3 keyHSV, float3 channelLimits, float3 channelFeathers, float3 channelFactors)
{
	float3 dists = float3(abs(inputHSV.x - keyHSV.x), abs(inputHSV.y - keyHSV.y), abs(inputHSV.z - keyHSV.z));
	if (dists.x > 0.5)
		dists.x = 1 - dists.x;

	float hueFactor = 1 - smoothstep(channelLimits.x, channelLimits.x + channelFeathers.x, dists.x);
	float saturationFactor = 1 - smoothstep(channelLimits.y, channelLimits.y + channelFeathers.y, dists.y);
	float valueFactor = 1 - smoothstep(channelLimits.z, channelLimits.z + channelFeathers.z, dists.z);

	float alpha = 1 - lerp(1, hueFactor, channelFactors.x) * lerp(1, saturationFactor, channelFactors.y) * lerp(1, valueFactor, channelFactors.z);
	return alpha;
}
#ifdef BG_REMOVAL_CHROMA
float3 ApplyChromaColorModification(float3 inputHSV, float3 keyHSV, float3 channelLimits)
{
	float hueDist = abs(inputHSV.x - keyHSV.x);
	if (hueDist > 0.5)
		hueDist = 1 - hueDist;
	hueDist = max(0.01, hueDist - channelLimits.x);
	hueDist *= 2;
	hueDist = hueDist / (1.0 - channelLimits.x * 2);
	float desat = smoothstep(_KeyDesaturateBandWidth, _KeyDesaturateBandWidth + _KeyDesaturateFalloffWidth, hueDist );
	inputHSV.y *= desat;
	return inputHSV;
}
#endif

void ApplyBackgroundRemoval(inout float3 inputRGB, float2 inputUvs, inout float inputAlpha)
{
	float3 inputHSV = RGB_to_HSV(inputRGB);
#ifdef BG_REMOVAL_CHROMA
	inputAlpha *= CalculateChromaAlpha(inputHSV, _KeyHsvMid, _KeyHsvRange, _KeyHsvFeathering, float3(1, 1, 1));
	inputHSV = ApplyChromaColorModification(inputHSV, _KeyHsvMid, _KeyHsvRange);
	inputRGB = HSV_to_RGB(inputHSV);
#elif BG_REMOVAL_STATIC
	float3 keyHsvMid = tex2D(_KeyMidTex, inputUvs).rgb;
	float3 keyHsvRange = tex2D(_KeyRangeTex, inputUvs).rgb;
	inputAlpha *= CalculateChromaAlpha(inputHSV, keyHsvMid, keyHsvRange, _KeyHsvFeathering, float3(1, 1, 1));
#endif
	
}