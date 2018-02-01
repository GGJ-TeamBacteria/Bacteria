/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/

#ifdef CROP_PLAYER
float3 _PlayerHeadPos;
float3 _PlayerLeftHandPos;
float3 _PlayerRightHandPos;
float3 _PlayerBasePos;

float _PlayerScale;

float _PlayerHeadCropRadius;
float _PlayerHandCropRadius;
float _PlayerFootCropRadius;

float2 GetPlayerBoxCoord(float3 worldPos, float4x4 worldToCam, float4x4 cameraProj, float4x4 camToWorld)
{
	float3 playerUp = _PlayerScale * normalize(_PlayerHeadPos - _PlayerBasePos);
	float3 playerRight = _PlayerScale * -normalize(cross(playerUp, mul(camToWorld, float3(0, 0, 1).xyz)));

	float4x4 mvp = mul(cameraProj, worldToCam);

	float2 screenPos = mul(mvp, worldPos);

	float2 playerScreenUp = (mul(mvp, _PlayerHeadPos) - mul(mvp, _PlayerBasePos)).xy;

	float dotHeadUp = dot(playerScreenUp, mul(mvp, _PlayerHeadPos + playerUp * _PlayerHeadCropRadius).xy);
	float dotBaseUp = dot(playerScreenUp, mul(mvp, _PlayerBasePos - playerUp * _PlayerFootCropRadius).xy);
	float dotLeftHandUp = dot(playerScreenUp, mul(mvp, _PlayerLeftHandPos + playerUp * _PlayerHandCropRadius).xy);
	float dotRightHandUp = dot(playerScreenUp, mul(mvp, _PlayerRightHandPos + playerUp * _PlayerHandCropRadius).xy);
	float maxDotUp = max(dotHeadUp, max(dotLeftHandUp, dotRightHandUp));

	float dotThisUp = dot(playerScreenUp, screenPos);

	float yFactor = (dotThisUp - dotBaseUp) / (maxDotUp - dotBaseUp);// smoothstep(dotBaseUp, maxDotUp, dotThisUp);

	float2 playerScreenRight = normalize(cross(float3(playerScreenUp.x, playerScreenUp.y, 0), float3(0, 0, 1)).xy);

	float dotHeadRightMin = dot(playerScreenRight, mul(mvp, _PlayerHeadPos - playerRight * _PlayerHeadCropRadius).xy);
	float dotLeftHandRightMin = dot(playerScreenRight, mul(mvp, _PlayerLeftHandPos - playerRight * _PlayerHandCropRadius).xy);
	float dotRightHandRightMin = dot(playerScreenRight, mul(mvp, _PlayerRightHandPos - playerRight * _PlayerHandCropRadius).xy);

	float dotHeadRightMax = dot(playerScreenRight, mul(mvp, _PlayerHeadPos + playerRight * _PlayerHeadCropRadius).xy);
	float dotLeftHandRightMax = dot(playerScreenRight, mul(mvp, _PlayerLeftHandPos + playerRight * _PlayerHandCropRadius).xy);
	float dotRightHandRightMax = dot(playerScreenRight, mul(mvp, _PlayerRightHandPos + playerRight * _PlayerHandCropRadius).xy);

	float minDotRight = min(dotHeadRightMin, min(dotLeftHandRightMin, dotRightHandRightMin));
	float maxDotRight = max(dotHeadRightMax, max(dotLeftHandRightMax, dotRightHandRightMax));

	float dotThisRight = dot(playerScreenRight, screenPos);

	float xFactor = (dotThisRight - minDotRight) / (maxDotRight - minDotRight);// smoothstep(minDotRight, maxDotRight, dotThisRight);

	return float2(xFactor, yFactor);
}

#endif

float GetPixelForwardDistance(float3 worldPos, float4x4 camToWorld)
{
	float3 camPos = mul(camToWorld, float4(0, 0, 0, 1)).xyz;
	float3 camForward = mul(camToWorld, float4(0, 0, 1, 0)).xyz;
	return 1-step(0, dot(worldPos - camPos, camForward));
}

void ApplyCropping(float3 worldPos, float4x4 worldToCam, float4x4 camProjection, float4x4 camToWorld, inout float alpha) 
{
#ifdef CROP_PLAYER
	float2 boxCoord = GetPlayerBoxCoord(worldPos, worldToCam, camProjection, camToWorld);
	alpha *= step(0.001, 0.5 - abs(0.5 - boxCoord.x));
	alpha *= step(0.001, 0.5 - abs(0.5 - boxCoord.y));
	alpha *= GetPixelForwardDistance(worldPos, camToWorld);
#endif
}