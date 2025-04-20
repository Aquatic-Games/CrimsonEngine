#pragma vertex VSMain
#pragma pixel PSMain

struct VSInput
{
    float2 Position: POSITION0;
    float2 TexCoord: TEXCOORD0;
    float4 Color: COLOR0;
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

cbuffer ProjectionMatrix : register(b0)
{
    float4x4 Projection;
}

Texture2D Texture : register(t0);
SamplerState Sampler : register(s0);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = float4(input.Position, 0.0, 1.0);
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