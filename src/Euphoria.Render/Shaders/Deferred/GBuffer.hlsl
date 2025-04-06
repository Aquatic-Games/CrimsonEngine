#pragma vertex VSMain
#pragma pixel PSMain

#include "../Common.hlsli"

struct VSInput
{
    float3 Position: POSITION0;
    float2 TexCoord: TEXCOORD0;
    float4 Color:    COLOR0;
    float3 Normal:   NORMAL0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 Albedo: SV_Target0;
};

cbuffer CameraBuffer : register(b0)
{
    Camera gCamera;
}

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(gCamera.Projection, mul(gCamera.View, float4(input.Position, 1.0)));
    output.TexCoord = input.TexCoord;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Albedo = (float4) 1.0;
    
    return output;
}