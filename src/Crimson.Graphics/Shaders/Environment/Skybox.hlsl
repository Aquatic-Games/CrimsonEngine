#pragma vertex VSMain 1 0
#pragma pixel PSMain 0 1

#include "../Crimson.hlsli"

struct VSOutput
{
    float4 Position: SV_Position;
    float3 TexCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

SAMPLERCUBE(Cube, 0)

VSOutput VSMain(const in float3 position: TEXCOORD0)
{
    VSOutput output;

    output.Position = mul(vCamera.Projection, mul((float3x3) vCamera.View, float4(position, 1.0))).xyww;
    output.TexCoord = position;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = SAMPLE(Cube, input.TexCoord);

    return output;
}