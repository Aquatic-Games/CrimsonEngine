using Crimson.Graphics.Materials;
using Crimson.Math;

namespace Crimson.Graphics.Renderers.Structs;

public struct MaterialProperties
{
    public Color AlbedoTint;

    public float MetallicMultiplier;

    public float RoughnessMultiplier;
    
    float Align1;
    
    float Align2;

    public MaterialProperties(Color albedoTint, float metallicMultiplier, float roughnessMultiplier)
    {
        AlbedoTint = albedoTint;
        MetallicMultiplier = metallicMultiplier;
        RoughnessMultiplier = roughnessMultiplier;
    }

    public static MaterialProperties FromMaterial(Material material)
        => new MaterialProperties(material.AlbedoTint, material.MetallicMultiplier, material.RoughnessMultiplier);
}