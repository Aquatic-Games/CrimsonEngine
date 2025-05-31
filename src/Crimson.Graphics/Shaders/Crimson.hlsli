#pragma once

#include "Common.hlsli"

struct Vertex
{
    float3 Position: TEXCOORD0;
    float2 TexCoord: TEXCOORD1;
    float3 Color:    TEXCOORD2;
    float3 Normal:   TEXCOORD3;
};

CBUFFER(gCamera, 0, Camera)
CBUFFER(gWorld, 1, float4x4)