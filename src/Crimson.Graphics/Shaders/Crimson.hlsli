#pragma once

#include "Common.hlsli"

struct Vertex
{
    float3 Position: TEXCOORD0;
    float2 TexCoord: TEXCOORD1;
    float3 Color:    TEXCOORD2;
    float3 Normal:   TEXCOORD3;
};

cbuffer CameraBuffer : register(b0, space1)
{
    Camera gCamera;
}

cbuffer WorldMatrix : register(b1, space1)
{
    float4x4 gWorld;
}