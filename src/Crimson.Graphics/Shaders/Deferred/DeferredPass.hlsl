#pragma vertex VSMain
#pragma pixel PSMain

#include "../Crimson.hlsli"
#include "../Lighting/BRDF.hlsli"

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

SAMPLER2D(Albedo, 0)
SAMPLER2D(Position, 1)
SAMPLER2D(Normal, 2)
SAMPLER2D(MetallicRoughness, 3)

VSOutput VSMain(const uint vertexId: SV_VertexID)
{
    const float2 vertices[] = {
        float2(-1.0, +1.0),
        float2(+1.0, +1.0),
        float2(+1.0, -1.0),
        float2(-1.0, -1.0),
    };

    const float2 texCoords[] = {
        float2(0.0, 0.0),
        float2(1.0, 0.0),
        float2(1.0, 1.0),
        float2(0.0, 1.0)
    };

    const uint indices[] = {
        0, 1, 3,
        1, 2, 3
    };

    VSOutput output;

    output.Position = float4(vertices[indices[vertexId]], 0.0, 1.0);
    output.TexCoord = texCoords[indices[vertexId]];
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    const float3 albedo = SAMPLE(Albedo, input.TexCoord).rgb;
    const float4 position = SAMPLE(Position, input.TexCoord);
    const float4 normal = SAMPLE(Normal, input.TexCoord);
    const float4 metallicRoughness = SAMPLE(MetallicRoughness, input.TexCoord);

    const float metallic = metallicRoughness.r;
    const float roughness = metallicRoughness.g;
    const float occlusion = metallicRoughness.b;
    
    const float3 camPos = pCamera.Position.xyz;
    const float3 worldPos = position.xyz;
    
    const float3 n = normalize(normal.xyz);
    const float3 v = normalize(camPos - worldPos);

    const float3 lightPos = float3(0.0, 2.0, 0.0);
    
    const float3 l = normalize(lightPos - worldPos);
    const float3 h = normalize(v + l);
    const float distance = length(lightPos - worldPos);
    const float attenuation = 1.0 / (distance * distance);
    const float3 radiance = (float3) 1.0 * attenuation;

    const float d = SpecularD(roughness, n, h);
    const float f = SpecularF(v, h);
    const float g = SpecularG(n, v, h, roughness);

    const float brdf = BRDF(d * f * g, n, v, l);

    const float3 kS = f;
    float3 kD = (float3) 1.0 - kS;
    kD *= 1.0 - metallic;

    float nDotL = max(dot(n, l), 0.0);

    const float light = (kD * albedo / M_PI + brdf) * radiance * nDotL;
    
    output.Color = float4((float3) light + (float3) 0.13 * albedo * occlusion, 1.0);
    
    return output;
}