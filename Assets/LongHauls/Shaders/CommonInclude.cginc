#ifndef COMMON_INCLUDE
#define COMMON_INCLUDE

float sqrdistance(float3 pA, float3 pB)
{
	float3 offset = pA - pB;
	return dot(offset, offset);
}

#endif