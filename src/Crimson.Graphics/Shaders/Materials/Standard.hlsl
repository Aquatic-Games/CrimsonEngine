#pragma vertex VSMain 2 0
#pragma pixel PSMain 1 6

#include "../GBuffer.hlsli"
#include "../Crimson.hlsli"
#include "Material.hlsli"

#include "../Math.hlsli"

struct VSOutput
{
    float4 Position: SV_Position;
    float4 Color: COLOR0;
    float2 TexCoord: TEXCOORD0;
    float3 Normal: NORMAL0;
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
    output.WorldSpace = mul(vWorld, float4(input.Position, 1.0)).xyz;
    output.Position = mul(vCamera.Projection, mul(vCamera.View, float4(output.WorldSpace, 1.0)));
    output.TexCoord = input.TexCoord;
    output.Normal = mul((float3x3) vWorld, input.Normal);
    
    return output;
}

GBufferOutput PSMain(const in VSOutput input)
{
    GBufferOutput output;

    const float4 albedo = SAMPLE(Albedo, input.TexCoord) * input.Color * gMaterial.AlbedoTint;
    clip(albedo.a - GetBayerValue(input.Position.xy));
    
    const float3 normal = SAMPLE(Normal, input.TexCoord).rgb * 2.0 - 1.0;
    const float metallic = SAMPLE(Metallic, input.TexCoord).r * gMaterial.MetallicMultiplier;
    const float roughness = SAMPLE(Roughness, input.TexCoord).r * gMaterial.RoughnessMultiplier;
    const float occlusion = SAMPLE(Occlusion, input.TexCoord).r;
    const float emission = SAMPLE(Emission, input.TexCoord).r;

    const float3 q1 = ddx(input.WorldSpace);
    const float3 q2 = ddy(input.WorldSpace);
    const float2 uv1 = ddx(input.TexCoord);
    const float2 uv2 = ddy(input.TexCoord);

    const float3 n = normalize(input.Normal);
    const float3 t = normalize(q1 * uv2.y - q2 * uv1.y);
    const float3 b = -normalize(cross(n, t));

    float3x3 tbn = float3x3(t, b, n);

    output.Albedo = float4(albedo.rgb, 1.0);
    output.Position = float4(input.WorldSpace, 1.0);
    output.Normal = float4(normalize(mul(normal, tbn)), 1.0);
    output.MetallicRoughness = float4(metallic, roughness, occlusion, emission);
    
    return output;
}