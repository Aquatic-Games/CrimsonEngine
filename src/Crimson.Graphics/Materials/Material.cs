using Crimson.Graphics.Utils;
using Crimson.Math;
using Graphite;

namespace Crimson.Graphics.Materials;

/// <summary>
/// A material that is used during rendering.
/// </summary>
public abstract class Material : IDisposable
{
    internal readonly Pipeline Pipeline;
    
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
    protected Material(in MaterialDefinition definition, string shader)
    {
        Albedo = definition.Albedo;
        Normal = definition.Normal ?? Texture.EmptyNormal;
        Metallic = definition.Metallic ?? Texture.White;
        Roughness = definition.Roughness ?? Texture.White;
        Occlusion = definition.Occlusion ?? Texture.White;
        Emission = definition.Emission ?? Texture.Black;

        AlbedoTint = definition.AlbedoTint;
        MetallicMultiplier = definition.MetallicMultiplier;
        RoughnessMultiplier = definition.RoughnessMultiplier;

        Device device = Renderer.Device;

        // TODO: Probably best not to load this shader every time a material is created.
        ShaderUtils.LoadGraphicsShader(device, shader, out ShaderModule? vertexShader, out ShaderModule? pixelShader);

        GraphicsPipelineInfo pipelineInfo = new()
        {
            VertexShader = vertexShader,
            PixelShader = pixelShader,
            ColorTargets =
            [
                new ColorTargetInfo(Format.R32G32B32A32_Float), // Albedo
                new ColorTargetInfo(Format.R32G32B32A32_Float), // Position
                new ColorTargetInfo(Format.R32G32B32A32_Float), // Normal
                new ColorTargetInfo(Format.R32G32B32A32_Float) // MetallicRoughness
            ],
            InputLayout =
            [
                new InputElementDescription(Format.R32G32B32_Float, 0, 0, 0), // Position
                new InputElementDescription(Format.R32G32_Float, 12, 1, 0), // TexCoord
                new InputElementDescription(Format.R32G32B32A32_Float, 20, 2, 0), // Color
                new InputElementDescription(Format.R32G32B32_Float, 36, 3, 0) // Normal
            ]
        };
        
        /*TODO: RasterizerState = new SDL.GPURasterizerState()
        {
            FillMode = SDL.GPUFillMode.Fill,
            CullMode = definition.RenderFace switch
            {
                RenderFace.Front => SDL.GPUCullMode.Back,
                RenderFace.Back => SDL.GPUCullMode.Front,
                RenderFace.Both => SDL.GPUCullMode.None,
                _ => throw new ArgumentOutOfRangeException()
            },
            FrontFace = definition.WindingOrder switch
            {
                WindingOrder.CounterClockwise => SDL.GPUFrontFace.CounterClockwise,
                WindingOrder.Clockwise => SDL.GPUFrontFace.Clockwise,
                _ => throw new ArgumentOutOfRangeException()
            },
        }*/

        Pipeline = device.CreateGraphicsPipeline(in pipelineInfo);
        
        pixelShader.Dispose();
        vertexShader.Dispose();
    }

    /// <summary>
    /// Dispose of this <see cref="Material"/>.
    /// </summary>
    public void Dispose()
    {
        Pipeline.Dispose();
    }
}