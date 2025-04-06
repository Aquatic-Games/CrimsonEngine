#pragma vertex VSMain
#pragma pixel PSMain

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

SamplerState Sampler : register(s0);

Texture2D Albedo : register(t0);

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

    float4 albedo = Albedo.Sample(Sampler, input.TexCoord);

    output.Color = float4(albedo.rgb, 1.0);
    
    return output;
}