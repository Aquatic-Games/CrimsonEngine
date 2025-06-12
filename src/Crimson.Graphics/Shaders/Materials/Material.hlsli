#include "../Common.hlsli"

struct Material
{
    float4 AlbedoTint;
    float MetallicMultiplier;
    float RoughnessMultiplier;
    float Align1;
    float Align2;
};

CBUFFER_PXL(gMaterial, 0, Material)