using Vortice.Direct3D11;

namespace Crimson.Graphics.Materials;

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
    /// <param name="renderer">A <see cref="Renderer"/> instance.</param>
    /// <param name="definition">The <see cref="MaterialDefinition"/> that describes how the material should be created.</param>
    public Material(Renderer renderer, in MaterialDefinition definition)
    {
        Albedo = definition.Albedo;
        Normal = definition.Normal ?? renderer.NormalTexture;
        Metallic = definition.Metallic ?? renderer.WhiteTexture;
        Roughness = definition.Roughness ?? renderer.BlackTexture;
        Occlusion = definition.Occlusion ?? renderer.WhiteTexture;
        Emission = definition.Emission ?? renderer.BlackTexture;

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
        
        RasterizerState = renderer.Device.CreateRasterizerState(rasterizerDesc);
    }

    /// <summary>
    /// Dispose of this <see cref="Material"/>.
    /// </summary>
    public void Dispose()
    {
        RasterizerState.Dispose();
    }
}