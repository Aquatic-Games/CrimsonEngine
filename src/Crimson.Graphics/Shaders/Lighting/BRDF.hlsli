#ifndef CS_BRDF_H
#define CS_BRDF_H

#include "../Math.hlsli"

// Based on "Real Shading in Unreal Engine 4
// https://cdn2.unrealengine.com/Resources/files/2013SiggraphPresentationsNotes-26915738.pdf

float4 DiffuseBRDF(const float4 albedo)
{
    return albedo / M_PI;
}

float SpecularD(const float roughness, const float n, const float h)
{
    // a = Roughness^2
    const float a = roughness * roughness;
    const float a2 = a * a;
    const float nDotH = max(dot(n, h), 0.0);

    const float denominator = (nDotH * nDotH) * (a2 - 1.0) + 1.0;

    return a2 / (M_PI * denominator * denominator);
}

float SpecularG(const float n, const float v, const float l, const float roughness)
{
    const float rough = roughness + 1;
    const float k = (rough * rough) / 8.0;

    const float nDotV = max(dot(n, v), 0.0);
    const float nDotL = max(dot(n, l), 0.0);
    
    const float gv = nDotV / (nDotV * (1 - k) + k);
    const float gl = nDotL / (nDotL * (1 - k) + k);

    return gv * gl;
}

float SpecularF(const float v, const float h)
{
    const float f0 = 0.04;

    const float vDotH = max(dot(v, h), 0.0);

    return f0 + (1 - f0) * pow(clamp(1 - vDotH, 0.0, 1.0), 5.0);
}

float BRDF(const float dfg, const float n, const float v, const float l)
{
    const float nDotL = max(dot(n, l), 0.0);
    const float nDotV = max(dot(n, v), 0.0);

    return dfg / (4.0 * nDotL * nDotV) + 0.0001;
}

#endif