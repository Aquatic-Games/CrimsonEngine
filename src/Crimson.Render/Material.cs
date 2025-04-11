using Vortice.Direct3D11;

namespace Crimson.Render;

/// <summary>
/// A material that is used during rendering.
/// </summary>
public class Material : IDisposable
{
    internal ID3D11RasterizerState RasterizerState;
    
    public Texture Albedo;

    /// <summary>
    /// Create a <see cref="Material"/> from the given definition.
    /// </summary>
    /// <param name="graphics">A <see cref="Graphics"/> instance.</param>
    /// <param name="definition">The <see cref="MaterialDefinition"/> that describes how the material should be created.</param>
    public Material(Graphics graphics, in MaterialDefinition definition)
    {
        Albedo = definition.Albedo;

        RasterizerState =
            graphics.Device.CreateRasterizerState(RasterizerDescription.CullBack with { FrontCounterClockwise = true });
    }

    /// <summary>
    /// Dispose of this <see cref="Material"/>.
    /// </summary>
    public void Dispose()
    {
        RasterizerState.Dispose();
    }
}