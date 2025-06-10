#pragma once

#define SAMPLER(Type, Name, Index) \
    Texture##Type Name : register(t##Index, space2);\
    SamplerState Name##Sampler : register(s##Index, space2);

#define SAMPLER2D(Name, Index) SAMPLER(2D, Name, Index)
#define SAMPLERCUBE(Name, Index) SAMPLER(Cube, Name, Index)

#define SAMPLE(Texture, TexCoord) Texture.Sample(Texture##Sampler, TexCoord)

#define CBUFFER_VTX(Name, Index, Type) cbuffer Name##Buffer : register(b##Index, space1) { Type Name; };
#define CBUFFER_PXL(Name, Index, Type) cbuffer Name##Buffer : register(b##Index, space3) { Type Name; };

struct Camera
{
    float4x4 Projection;
    float4x4 View;
};