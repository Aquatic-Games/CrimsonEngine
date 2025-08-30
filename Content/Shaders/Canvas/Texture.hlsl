struct VSInput
{
    float2 Position: TEXCOORD0;
    float2 TexCoord: TEXCOORD1;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
}

cbuffer CameraMatrices : register(b0, space1)
{
    float4x4 Projection;
    float4x4 Transform;
}

Texture2D Texture    : register(t0, space0);
SamplerState Sampler : register(s0, space0);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Projection, mul(Transform, float4(input.Position, 0.0, 1.0)));
    output.TexCoord = input.TexCoord;

    return output;
}

float4 PSMain(const in VSOutput input): SV_Target0
{
    return Texture.Sample(Sampler, input.TexCoord);
}
