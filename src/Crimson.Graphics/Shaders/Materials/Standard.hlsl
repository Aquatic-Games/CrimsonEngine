#pragma vertex VSMain
#pragma pixel PSMain

#include "../GBuffer.hlsli"
#include "../Crimson.hlsli"

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
    float3 WorldSpace: POSITION0;
};

SamplerState Sampler : register(s0, space2);

Texture2D Albedo : register(t0, space2);
Texture2D Normal : register(t1, space2);
Texture2D Metallic : register(t2, space2);
Texture2D Roughness : register(t3, space2);
Texture2D Occlusion : register(t4, space2);
Texture2D Emission : register(t5, space2);

VSOutput VSMain(const in Vertex input)
{
    VSOutput output;

    output.WorldSpace = mul(gWorld, float4(input.Position, 1.0)).xyz;
    output.Position = mul(gCamera.Projection, mul(gCamera.View, float4(output.WorldSpace, 1.0)));
    output.TexCoord = input.TexCoord;
    
    return output;
}

GBufferOutput PSMain(const in VSOutput input)
{
    GBufferOutput output;

    const float3 albedo = Albedo.Sample(Sampler, input.TexCoord).rgb;
    const float3 normal = Normal.Sample(Sampler, input.TexCoord).rgb;
    const float metallic = Metallic.Sample(Sampler, input.TexCoord).r;
    const float roughness = Roughness.Sample(Sampler, input.TexCoord).r;
    const float occlusion = Occlusion.Sample(Sampler, input.TexCoord).r;
    const float emission = Emission.Sample(Sampler, input.TexCoord).r;

    output.Albedo = float4(albedo, 1.0);
    output.Position = float4(input.WorldSpace, 1.0);
    output.Normal = float4(normal, 1.0);
    output.MetallicRoughness = float4(metallic, roughness, occlusion, emission);
    
    return output;
}