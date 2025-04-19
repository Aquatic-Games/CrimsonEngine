#pragma vertex VSMain
#pragma pixel PSMain

struct VSInput
{
    float2 Position: POSITION0;
    float2 TexCoord: TEXCOORD0;
    float4 Tint:     COLOR0;
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

cbuffer CameraMatrices : register(b0)
{
    float4x4 Projection;
    float4x4 Transform;
}

Texture2D Texture : register(t0);
SamplerState State : register(s0);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Projection, mul(Transform, float4(input.Position, 0.0, 1.0)));
    output.TexCoord = input.TexCoord;
    output.Tint = input.Tint;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = Texture.Sample(State, input.TexCoord) * input.Tint;
    
    return output;
}