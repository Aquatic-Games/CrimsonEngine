namespace Crimson.Render;

/// <summary>
/// Defines how a material will be created.
/// </summary>
public struct MaterialDefinition
{
    /// <summary>
    /// The albedo/base texture.
    /// </summary>
    public Texture Albedo;

    /// <summary>
    /// The normal texture. If none is provided, a default will be used.
    /// </summary>
    public Texture? Normal;

    /// <summary>
    /// The metallic texture. If none is provided, a default will be used.
    /// </summary>
    public Texture? Metallic;

    /// <summary>
    /// The roughness texture. If none is provided, a default will be used.
    /// </summary>
    public Texture? Roughness;

    /// <summary>
    /// The occlusion texture. If none is provided, a default will be used.
    /// </summary>
    public Texture? Occlusion;

    /// <summary>
    /// The emission texture. If none is provided, a default will be used.
    /// </summary>
    public Texture? Emission;

    /// <summary>
    /// Define a material with an albedo texture and the default values.
    /// </summary>
    /// <param name="albedo">The albedo texture.</param>
    public MaterialDefinition(Texture albedo)
    {
        Albedo = albedo;
        Normal = null;
        Metallic = null;
        Roughness = null;
        Occlusion = null;
    }
    
    
}