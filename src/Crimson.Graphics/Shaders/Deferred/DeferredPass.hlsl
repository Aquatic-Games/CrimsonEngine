#pragma vertex VSMain
#pragma pixel PSMain

#include "../Common.hlsli"

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

    const float4 albedo = SAMPLE(Albedo, input.TexCoord);

    output.Color = float4(albedo.rgb, 1.0);
    
    return output;
}