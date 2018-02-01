/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
#pragma exclude_renderers d3d11 gles

//Camera
float Buffer01Depth(float lin)
{
	return((1 / lin) - _ZBufferParams.y) / _ZBufferParams.x;
}

float3 CalculateWorldPosition(float3 position, float near, float far, float4x4 cameraProj, float4x4 inverseView)
{
	float2 p11_22 = float2(cameraProj._11, cameraProj._22);
	float2 p13_31 = float2(cameraProj._13, cameraProj._23);

	float depthY = far / near;
	float depthX = 1.0 - depthY;
	float d = ((1.0 / position.z) - depthY) / depthX;

	float vz = near * far / lerp(far, near, d);

	float3 vpos = float3((position.xy * 2 - 1 - p13_31) / p11_22 * vz, -vz);
	return mul(inverseView, float4(vpos, 1)).xyz;
}