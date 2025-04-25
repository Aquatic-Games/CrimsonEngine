#pragma vertex VSMain
#pragma pixel PSMain

#include "../Common.hlsli"

struct VSOutput
{
    float4 Position: SV_Position;
    float3 TexCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

cbuffer CameraBuffer : register(b0, space1)
{
    Camera gCamera;
}

TextureCube Cube : register(t0, space2);
SamplerState Sampler : register(s0, space2);

VSOutput VSMain(const in float3 position: POSITION0)
{
    VSOutput output;

    output.Position = mul(gCamera.Projection, mul((float3x3) gCamera.View, float4(position, 1.0))).xyww;
    output.TexCoord = position;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = Cube.Sample(Sampler, input.TexCoord);

    return output;
}