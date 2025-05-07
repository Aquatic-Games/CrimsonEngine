#include "../Math.hlsli"

// Based on "Real Shading in Unreal Engine 4
// https://cdn2.unrealengine.com/Resources/files/2013SiggraphPresentationsNotes-26915738.pdf

float4 DiffuseBRDF(const float4 albedo)
{
    return albedo / M_PI;
}

float4 SpecularGGX(const float roughness, const float n, const float h)
{
    // a = Roughness^2
    const float a = roughness * roughness;
    const float a2 = a * a;
    const float nDotH = max(dot(n, h), 0.0);

    const float denominator = (nDotH * nDotH) * (a2 - 1) + 1;

    return a2 / (M_PI * denominator * denominator);
}

float4 