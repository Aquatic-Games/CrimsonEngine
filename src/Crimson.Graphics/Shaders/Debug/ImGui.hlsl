#pragma vertex VSMain
#pragma pixel PSMain

//#include "../Common.hlsli"

struct VSInput
{
    float2 Position: TEXCOORD0;
    float2 TexCoord: TEXCOORD1;
    float4 Color: TEXCOORD2;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
    float4 Color: COLOR0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

cbuffer ProjectionMatrix : register(b0, space0)
{
    float4x4 Projection;
}

Texture2D Texture : register(t0, space1);
SamplerState Sampler : register(s0, space1);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Projection, float4(input.Position, 0.0, 1.0));
    output.TexCoord = input.TexCoord;
    output.Color = input.Color;

    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = Texture.Sample(Sampler, input.TexCoord) * input.Color;
    
    return output;
}