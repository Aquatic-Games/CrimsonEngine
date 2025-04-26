#pragma vertex VSMain
#pragma pixel PSMain

#include "../Common.hlsli"

struct VSInput
{
    float3 Position: TEXCOORD0;
    float2 TexCoord: TEXCOORD1;
    float4 Color:    TEXCOORD2;
    float3 Normal:   TEXCOORD3;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
    float3 WorldSpace: POSITION0;
};

struct PSOutput
{
    float4 Albedo: SV_Target0;
    float4 Position: SV_Target1;
};

cbuffer CameraBuffer : register(b0, space1)
{
    Camera gCamera;
}

cbuffer WorldMatrix : register(b1, space1)
{
    float4x4 World;
}

SamplerState Sampler : register(s0, space2);

Texture2D Albedo : register(t0, space2);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.WorldSpace = mul(World, float4(input.Position, 1.0)).xyz;
    output.Position = mul(gCamera.Projection, mul(gCamera.View, float4(output.WorldSpace, 1.0)));
    output.TexCoord = input.TexCoord;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    const float3 albedo = Albedo.Sample(Sampler, input.TexCoord).rgb;

    output.Albedo = float4(albedo, 1.0);
    output.Position = float4(input.WorldSpace, 1.0);
    
    return output;
}