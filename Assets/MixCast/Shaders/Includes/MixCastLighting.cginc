/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/

#ifdef LIGHTING_FLAT
float _PlayerLightingFactor;
float _PlayerLightingBase;
float _PlayerLightingPower;

int _DirLightCount;
float4 _DirLightDirections[10];
float4 _DirLightColors[10];

int _PointLightCount;
float4 _PointLightPositions[100];	//w contains range
float4 _PointLightColors[100];

int _SpotLightCount;
float4 _SpotLightPositions[100];	//w contains range
float4 _SpotLightDirections[100];	//w contains angle
float4 _SpotLightColors[100];

//Lighting
float3 CalculateLightingFlatDirectional(float3 worldPos)
{
	float3 sum = 0;
	for (int i = 0; i < _DirLightCount; i++)
	{
		sum += _DirLightColors[i];
	}
	return sum;
}
float3 CalculateLightingFlatPoint(float3 worldPos)
{
	float3 sum = 0;
	for (int i = 0; i < _PointLightCount; i++)
	{
		float3 delta = worldPos - _PointLightPositions[i];
		float dist = sqrt(dot(delta, delta));	//Sqr mag
		float atten = clamp(1.0 - dist / _PointLightPositions[i].w, 0.0, 1.0);
		atten *= atten;
		sum += atten * _PointLightColors[i];
	}
	return sum;
}
float3 CalculateLightingFlatSpot(float3 worldPos)
{
	float3 sum = 0;
	for (int i = 0; i < _SpotLightCount; i++)
	{
		float range = _SpotLightPositions[i].w;
		float angle = _SpotLightDirections[i].w;

		float3 forwardDot = saturate(dot(worldPos - _SpotLightPositions[i].xyz, _SpotLightDirections[i].xyz));
		float forwardAtten = 1-smoothstep(0.95 * range, range, forwardDot);
		//forwardAtten = 1 - pow(1-forwardAtten,3);

		float3 projectedPos = _SpotLightPositions[i].xyz + _SpotLightDirections[i].xyz * forwardDot;
		float3 projectedToPos = worldPos - projectedPos;
		float projectedDist = sqrt(dot(projectedToPos, projectedToPos));
		float projectedSpotRadius = forwardDot * tan(angle);

		float radialAtten = 1 - smoothstep(0.8 * projectedSpotRadius, projectedSpotRadius, projectedDist);// clamp(1.0 - projectedDist / projectedSpotRadius, 0.0, 1.0);
		radialAtten *= radialAtten;

		sum += forwardAtten * radialAtten * _SpotLightColors[i].rgb;
	}
	return sum;
}
float3 CalculateLightingFlat(float3 worldPos)
{
	float3 sum = 0;
	sum += CalculateLightingFlatDirectional(worldPos);
	sum += CalculateLightingFlatPoint(worldPos);
	//sum += CalculateLightingFlatSpot(worldPos); //not right yet
	return sum;
}
float3 ApplyLightingFlat(float3 inputRGB, float3 worldPos)
{
	float3 lightingColor = CalculateLightingFlat(worldPos);

	return lerp(inputRGB, inputRGB * (lightingColor * _PlayerLightingPower + _PlayerLightingBase), _PlayerLightingFactor);
}
#endif

float3 ApplyLighting(float3 inputRGB, float3 worldPos, float3 worldNormal)
{
#ifdef LIGHTING_FLAT
	return ApplyLightingFlat(inputRGB, worldPos);
#else
	return inputRGB;
#endif
}