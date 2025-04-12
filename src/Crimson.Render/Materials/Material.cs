using Vortice.Direct3D11;

namespace Crimson.Render.Materials;

/// <summary>
/// A material that is used during rendering.
/// </summary>
public class Material : IDisposable
{
    internal ID3D11RasterizerState RasterizerState;
    
    public Texture Albedo;

    public Texture Normal;

    public Texture Metallic;

    public Texture Roughness;

    public Texture Occlusion;

    public Texture Emission;
    
    /// <summary>
    /// Create a <see cref="Material"/> from the given definition.
    /// </summary>
    /// <param name="graphics">A <see cref="Graphics"/> instance.</param>
    /// <param name="definition">The <see cref="MaterialDefinition"/> that describes how the material should be created.</param>
    public Material(Graphics graphics, in MaterialDefinition definition)
    {
        Albedo = definition.Albedo;
        Normal = definition.Normal ?? graphics.NormalTexture;
        Metallic = definition.Metallic ?? graphics.WhiteTexture;
        Roughness = definition.Roughness ?? graphics.BlackTexture;
        Occlusion = definition.Occlusion ?? graphics.WhiteTexture;
        Emission = definition.Emission ?? graphics.BlackTexture;

        CullMode cullMode = definition.RenderFace switch
        {
            RenderFace.Front => CullMode.Back,
            RenderFace.Back => CullMode.Front,
            RenderFace.Both => CullMode.None,
            _ => throw new ArgumentOutOfRangeException()
        };

        RasterizerDescription rasterizerDesc = new()
        {
            FillMode = FillMode.Solid,
            FrontCounterClockwise = definition.WindingOrder == WindingOrder.CounterClockwise,
            CullMode = cullMode
        };
        
        RasterizerState = graphics.Device.CreateRasterizerState(rasterizerDesc);
    }

    /// <summary>
    /// Dispose of this <see cref="Material"/>.
    /// </summary>
    public void Dispose()
    {
        RasterizerState.Dispose();
    }
}