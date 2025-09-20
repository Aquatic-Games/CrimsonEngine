using Crimson.Math;

namespace Crimson.Graphics.Materials;

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
    /// Tints the <see cref="Albedo"/> texture to the given color. Use <see cref="Color.White"/> for no tint.
    /// </summary>
    public Color AlbedoTint;

    /// <summary>
    /// The multiplier for the <see cref="Metallic"/> texture.
    /// </summary>
    public float MetallicMultiplier;

    /// <summary>
    /// The multiplier for the <see cref="Roughness"/> texture.
    /// </summary>
    public float RoughnessMultiplier;

    /// <summary>
    /// The face(s) to render.
    /// </summary>
    public RenderFace RenderFace;

    /// <summary>
    /// The winding order of the front face.
    /// </summary>
    public WindingOrder WindingOrder;

    public MaterialDefinition()
    {
        Albedo = Texture.White;
        AlbedoTint = Color.White;
        MetallicMultiplier = 1;
        RoughnessMultiplier = 1;
        RenderFace = RenderFace.Front;
        WindingOrder = WindingOrder.CounterClockwise;
    }

    /// <summary>
    /// Define a material with an albedo texture and the default values.
    /// </summary>
    /// <param name="albedo">The albedo texture.</param>
    public MaterialDefinition(Texture albedo) : this()
    {
        Albedo = albedo;
        Normal = null;
        Metallic = null;
        Roughness = null;
        Occlusion = null;
    }
}