using Crimson.Math;

namespace Crimson.Graphics.Materials;

/// <summary>
/// A material that is used during rendering.
/// </summary>
public abstract class Material : IDisposable
{
    public Texture Albedo;

    public Texture Normal;

    public Texture Metallic;

    public Texture Roughness;

    public Texture Occlusion;

    public Texture Emission;

    public Color AlbedoTint;

    public float MetallicMultiplier;

    public float RoughnessMultiplier;
    
    /// <summary>
    /// Create a <see cref="Material"/> from the given definition.
    /// </summary>
    /// <param name="definition">The <see cref="MaterialDefinition"/> that describes how the material should be created.</param>
    protected Material()
    {
    }

    /// <summary>
    /// Dispose of this <see cref="Material"/>.
    /// </summary>
    public abstract void Dispose();
}