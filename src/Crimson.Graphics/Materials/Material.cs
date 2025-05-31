using Crimson.Graphics.Utils;
using SDL3;

namespace Crimson.Graphics.Materials;

/// <summary>
/// A material that is used during rendering.
/// </summary>
public class Material : IDisposable
{
    private readonly IntPtr _device;
    
    internal readonly IntPtr Pipeline;
    
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
    public unsafe Material(Renderer renderer, in MaterialDefinition definition)
    {
        Albedo = definition.Albedo;
        Normal = definition.Normal ?? renderer.NormalTexture;
        Metallic = definition.Metallic ?? renderer.WhiteTexture;
        Roughness = definition.Roughness ?? renderer.BlackTexture;
        Occlusion = definition.Occlusion ?? renderer.WhiteTexture;
        Emission = definition.Emission ?? renderer.BlackTexture;

        _device = renderer.Device;

        // TODO: Probably best not to load this shader every time a material is created.
        IntPtr vertexShader =
            ShaderUtils.LoadGraphicsShader(_device, SDL.GPUShaderStage.Vertex, "Materials/Standard", "VSMain", 2, 0);
        IntPtr pixelShader =
            ShaderUtils.LoadGraphicsShader(_device, SDL.GPUShaderStage.Fragment, "Materials/Standard", "PSMain", 0, 6);

        SDL.GPUVertexBufferDescription vertexBufferDesc = new()
        {
            Slot = 0,
            InputRate = SDL.GPUVertexInputRate.Vertex,
            InstanceStepRate = 0,
            Pitch = Vertex.SizeInBytes
        };

        SDL.GPUVertexAttribute* vertexAttributes = stackalloc SDL.GPUVertexAttribute[]
        {
            new SDL.GPUVertexAttribute // Position
                { Format = SDL.GPUVertexElementFormat.Float3, Offset = 0, BufferSlot = 0, Location = 0 },
            new SDL.GPUVertexAttribute // TexCoord
                { Format = SDL.GPUVertexElementFormat.Float2, Offset = 12, BufferSlot = 0, Location = 1 },
            new SDL.GPUVertexAttribute // Color
                { Format = SDL.GPUVertexElementFormat.Float4, Offset = 20, BufferSlot = 0, Location = 2 },
            new SDL.GPUVertexAttribute // Normal
                { Format = SDL.GPUVertexElementFormat.Float3, Offset = 36, BufferSlot = 0, Location = 3 }
        };

        SDL.GPUColorTargetDescription* colorTargets = stackalloc SDL.GPUColorTargetDescription[]
        {
            new SDL.GPUColorTargetDescription { Format = SDL.GPUTextureFormat.R8G8B8A8Unorm }, // Albedo
            new SDL.GPUColorTargetDescription { Format = SDL.GPUTextureFormat.R32G32B32A32Float }, // Position
            new SDL.GPUColorTargetDescription { Format = SDL.GPUTextureFormat.R8G8B8A8Unorm }, // Normal
            new SDL.GPUColorTargetDescription { Format = SDL.GPUTextureFormat.R8G8B8A8Unorm } // MetallicRoughness
        };

        SDL.GPUGraphicsPipelineCreateInfo pipelineInfo = new()
        {
            VertexShader = vertexShader,
            FragmentShader = pixelShader,
            TargetInfo = new SDL.GPUGraphicsPipelineTargetInfo()
            {
                NumColorTargets = 4,
                ColorTargetDescriptions = (nint) colorTargets,
                HasDepthStencilTarget = 1,
                DepthStencilFormat = SDL.GPUTextureFormat.D32Float
            },
            VertexInputState = new SDL.GPUVertexInputState()
            {
                NumVertexBuffers = 1,
                VertexBufferDescriptions = new IntPtr(&vertexBufferDesc),
                NumVertexAttributes = 4,
                VertexAttributes = (nint) vertexAttributes
            },
            PrimitiveType = SDL.GPUPrimitiveType.TriangleList,
            DepthStencilState = new SDL.GPUDepthStencilState()
            {
                EnableDepthTest = 1,
                EnableDepthWrite = 1,
                CompareOp = SDL.GPUCompareOp.Less
            },
            RasterizerState = new SDL.GPURasterizerState()
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
            }
        };

        Pipeline = SDL.CreateGPUGraphicsPipeline(_device, in pipelineInfo);
        
        SDL.ReleaseGPUShader(_device, pixelShader);
        SDL.ReleaseGPUShader(_device, vertexShader);
    }

    /// <summary>
    /// Dispose of this <see cref="Material"/>.
    /// </summary>
    public void Dispose()
    {
        SDL.ReleaseGPUGraphicsPipeline(_device, Pipeline);
    }
}