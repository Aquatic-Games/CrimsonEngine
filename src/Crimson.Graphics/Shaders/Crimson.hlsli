#ifndef CS_CRIMSON_H
#define CS_CRIMSON_H

#include "Common.hlsli"

struct Vertex
{
    float3 Position: TEXCOORD0;
    float2 TexCoord: TEXCOORD1;
    float4 Color:    TEXCOORD2;
    float3 Normal:   TEXCOORD3;
};

CBUFFER_VTX(vCamera, 0, Camera)
CBUFFER_PXL(pCamera, 0, Camera)
CBUFFER_VTX(vWorld, 1, float4x4)

#endif