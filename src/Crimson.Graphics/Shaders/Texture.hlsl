#pragma vertex VSMain
#pragma pixel PSMain

#include "Crimson.hlsli"

struct VSInput
{
    float2 Position: TEXCOORD0;
    float2 TexCoord: TEXCOORD1;
    float4 Tint:     TEXCOORD2;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
    float4 Tint:     COLOR0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

SAMPLER2D(Texture, 0)

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(vCamera.Projection, mul(vCamera.View, float4(input.Position, 0.0, 1.0)));
    output.TexCoord = input.TexCoord;
    output.Tint = input.Tint;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = SAMPLE(Texture, input.TexCoord) * input.Tint;
    
    return output;
}