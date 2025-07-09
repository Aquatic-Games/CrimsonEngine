#ifndef CS_GBUFFER_H
#define CS_GBUFFER_H

struct GBufferOutput
{
    float4 Albedo: SV_Target0;
    float4 Position: SV_Target1;
    float4 Normal: SV_Target2;
    float4 MetallicRoughness: SV_Target3;
};

#endif