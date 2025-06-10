#pragma vertex VSMain
#pragma pixel PSMain

#include "../GBuffer.hlsli"
#include "../Crimson.hlsli"

struct VSOutput
{
    float4 Position: SV_Position;
    float4 Color: COLOR0;
    float2 TexCoord: TEXCOORD0;
    float3 WorldSpace: POSITION0;
};

SAMPLER2D(Albedo, 0)
SAMPLER2D(Normal, 1)
SAMPLER2D(Metallic, 2)
SAMPLER2D(Roughness, 3)
SAMPLER2D(Occlusion, 4)
SAMPLER2D(Emission, 5)

VSOutput VSMain(const in Vertex input)
{
    VSOutput output;

    output.Color = input.Color;
    output.WorldSpace = mul(gWorld, float4(input.Position, 1.0)).xyz;
    output.Position = mul(gCamera.Projection, mul(gCamera.View, float4(output.WorldSpace, 1.0)));
    output.TexCoord = input.TexCoord;
    
    return output;
}

GBufferOutput PSMain(const in VSOutput input)
{
    GBufferOutput output;

    const float3 albedo = SAMPLE(Albedo, input.TexCoord).rgb;
    const float3 normal = SAMPLE(Normal, input.TexCoord).rgb;
    const float metallic = SAMPLE(Metallic, input.TexCoord).r;
    const float roughness = SAMPLE(Roughness, input.TexCoord).r;
    const float occlusion = SAMPLE(Occlusion, input.TexCoord).r;
    const float emission = SAMPLE(Emission, input.TexCoord).r;

    output.Albedo = float4(albedo * input.Color.rgb, 1.0);
    output.Position = float4(input.WorldSpace, 1.0);
    output.Normal = float4(normal, 1.0);
    output.MetallicRoughness = float4(metallic, roughness, occlusion, emission);
    
    return output;
}