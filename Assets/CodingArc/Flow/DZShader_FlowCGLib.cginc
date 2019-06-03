#if !defined(FLOW_INCLUDED)
#define FLOW_INCLUDED

float2 DZ_FlowUV_01 (float2 uv, float time) 
{
	return uv + time;
}

float2 DZ_FlowUV_ByVector (float2 uv,float2 flowVector,float time) 
{
	float progress = frac(time);
	return uv + flowVector * progress;
}

#endif