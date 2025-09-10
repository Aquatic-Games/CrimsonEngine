using System.Numerics;
using Crimson.Graphics.Renderers.Structs;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Graphite;

namespace Crimson.Graphics.Renderers;

internal class DeferredRenderer : IDisposable
{
    private const Format GBufferFormat = Format.R32G32B32A32_Float;
    
    private readonly Device _device;
    
    private GrTexture _albedoTexture;
    private GrTexture _positionTexture;
    private GrTexture _normalTexture;
    private GrTexture _metallicRoughnessTexture;

    private readonly Pipeline _passPipeline;
    private readonly Sampler _passSampler;

    private readonly DescriptorLayout _cameraLayout; // The camera. Set at the start of the frame and left.
    private readonly DescriptorLayout _materialLayout; // The material. Each material contains its own set, which is set on draw.
    private readonly DescriptorLayout _perDrawLayout; // The per-draw properties, such as a world matrix and tint color.
    
    private readonly DescriptorLayout _cameraSet;

    private readonly List<WorldRenderable> _drawQueue;

    public Texture[] DebugTextures;
    
    public DeferredRenderer(Device device, Size<int> size, Format outFormat)
    {
        _device = device;

        _albedoTexture = CreateGBufferTexture(size);
        // TODO: SDL.SetGPUTextureName(_device, _albedoTexture, "Albedo GBuffer");
        _positionTexture = CreateGBufferTexture(size);
        // TODO: SDL.SetGPUTextureName(_device, _positionTexture, "Position GBuffer");
        _normalTexture = CreateGBufferTexture(size);
        // TODO: SDL.SetGPUTextureName(_device, _normalTexture, "Normal GBuffer");
        _metallicRoughnessTexture = CreateGBufferTexture(size);
        // TODO: SDL.SetGPUTextureName(_device, _metallicRoughnessTexture, "Metallic-Roughness-Occlusion GBuffer");

        DebugTextures =
        [
            new Texture(_albedoTexture, size, "Albedo"),
            new Texture(_positionTexture, size, "Position"),
            new Texture(_normalTexture, size, "Normals"),
            new Texture(_metallicRoughnessTexture, size, "Metallic-Roughness-Occlusion")
        ];

        ShaderUtils.LoadGraphicsShader(device, "Deferred/DeferredPass", out ShaderModule? passVtx,
            out ShaderModule? passPxl);

        GraphicsPipelineInfo passPipelineInfo = new()
        {
            VertexShader = passVtx,
            PixelShader = passPxl,
            ColorTargets = [new ColorTargetInfo(outFormat)]
        };

        _passPipeline = _device.CreateGraphicsPipeline(in passPipelineInfo);
        
        passPxl.Dispose();
        passVtx.Dispose();

        _passSampler = _device.CreateSampler(SamplerInfo.LinearWrap);

        _drawQueue = [];

        _cameraLayout = _device.CreateDescriptorLayout(new DescriptorLayoutInfo
        {
            Bindings = [new DescriptorBinding(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex)]
        });

        _materialLayout = _device.CreateDescriptorLayout(new DescriptorLayoutInfo
        {
            Bindings =
            [
                new DescriptorBinding(0, DescriptorType.ConstantBuffer, ShaderStage.VertexPixel), // gMaterial
                new DescriptorBinding(1, DescriptorType.Texture, ShaderStage.Pixel), // Texture0/Albedo
                new DescriptorBinding(2, DescriptorType.Texture, ShaderStage.Pixel), // Texture1/Normal
                new DescriptorBinding(3, DescriptorType.Texture, ShaderStage.Pixel), // Texture2/Metallic
                new DescriptorBinding(4, DescriptorType.Texture, ShaderStage.Pixel), // Texture3/Roughness
                new DescriptorBinding(5, DescriptorType.Texture, ShaderStage.Pixel), // Texture4/Occlusion
                new DescriptorBinding(6, DescriptorType.Texture, ShaderStage.Pixel), // Texture5/Emission
            ]
        });

        _perDrawLayout = _device.CreateDescriptorLayout(new DescriptorLayoutInfo
        {
            Bindings = [new DescriptorBinding(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex)],
            // TODO: Buffer offset in SetDescriptorSet
            PushDescriptor = true
        });
    }

    public void AddToQueue(Renderable renderable, Matrix4x4 worldMatrix)
    {
        _drawQueue.Add(new WorldRenderable(renderable, worldMatrix));
    }

    public unsafe bool Render(IntPtr cb, GrTexture compositeTarget, GrTexture depthTexture, CameraMatrices camera)
    {
        // Don't bother rendering if there is nothing to draw.
        if (_drawQueue.Count == 0)
            return false;
        
        SDL.PushGPUVertexUniformData(cb, 0, new IntPtr(&camera), CameraMatrices.SizeInBytes);
        
        #region GBuffer Pass
        
        SdlUtils.PushDebugGroup(cb, "GBuffer Pass");

        SDL.GPUColorTargetInfo* gBufferTargets = stackalloc SDL.GPUColorTargetInfo[]
        {
            new SDL.GPUColorTargetInfo
            {
                Texture = _albedoTexture, ClearColor = new SDL.FColor(), LoadOp = SDL.GPULoadOp.Clear,
                StoreOp = SDL.GPUStoreOp.Store
            },
            new SDL.GPUColorTargetInfo
            {
                Texture = _positionTexture, ClearColor = new SDL.FColor(), LoadOp = SDL.GPULoadOp.Clear,
                StoreOp = SDL.GPUStoreOp.Store
            },
            new SDL.GPUColorTargetInfo
            {
                Texture = _normalTexture, ClearColor = new SDL.FColor(), LoadOp = SDL.GPULoadOp.Clear,
                StoreOp = SDL.GPUStoreOp.Store
            },
            new SDL.GPUColorTargetInfo
            {
                Texture = _metallicRoughnessTexture, ClearColor = new SDL.FColor(), LoadOp = SDL.GPULoadOp.Clear,
                StoreOp = SDL.GPUStoreOp.Store
            }
        };

        SDL.GPUDepthStencilTargetInfo depthInfo = new()
        {
            Texture = depthTexture,
            ClearDepth = 1.0f,
            LoadOp = SDL.GPULoadOp.Clear,
            StoreOp = SDL.GPUStoreOp.Store
        };

        IntPtr gBufferPass = SDL.BeginGPURenderPass(cb, (nint) gBufferTargets, 4, in depthInfo)
            .Check("Begin gbuffer pass");

        // TODO: Position field in CameraMatrices
        if (!Matrix4x4.Invert(camera.View, out Matrix4x4 invView))
            invView = Matrix4x4.Identity;
        Vector3 cameraPosition = invView.Translation;
        
        IOrderedEnumerable<WorldRenderable> frontToBack = _drawQueue.OrderBy(renderable =>
            Vector3.Distance(cameraPosition, renderable.WorldMatrix.Translation));

        const int numSamplerBindings = 6;
        SDL.GPUTextureSamplerBinding* bindings = stackalloc SDL.GPUTextureSamplerBinding[numSamplerBindings];
        
        foreach ((Renderable renderable, Matrix4x4 world) in frontToBack)
        {
            SDL.PushGPUVertexUniformData(cb, 1, new IntPtr(&world), 64);
            
            MaterialProperties matProps = MaterialProperties.FromMaterial(renderable.Material);
            SDL.PushGPUFragmentUniformData(cb, 0, new IntPtr(&matProps), (uint) sizeof(MaterialProperties));

            // TODO: Have a sampler per material.
            bindings[0] = new SDL.GPUTextureSamplerBinding
            {
                Texture = renderable.Material.Albedo.TextureHandle,
                Sampler = _passSampler
            };
            bindings[1] = new SDL.GPUTextureSamplerBinding
            {
                Texture = renderable.Material.Normal.TextureHandle,
                Sampler = _passSampler,
            };
            bindings[2] = new SDL.GPUTextureSamplerBinding
            {
                Texture = renderable.Material.Metallic.TextureHandle,
                Sampler = _passSampler,
            };
            bindings[3] = new SDL.GPUTextureSamplerBinding
            {
                Texture = renderable.Material.Roughness.TextureHandle,
                Sampler = _passSampler,
            };
            bindings[4] = new SDL.GPUTextureSamplerBinding
            {
                Texture = renderable.Material.Occlusion.TextureHandle,
                Sampler = _passSampler,
            };
            bindings[5] = new SDL.GPUTextureSamplerBinding
            {
                Texture = renderable.Material.Emission.TextureHandle,
                Sampler = _passSampler,
            };

            SDL.BindGPUFragmentSamplers(gBufferPass, 0, (nint) bindings, numSamplerBindings);

            SDL.BindGPUGraphicsPipeline(gBufferPass, renderable.Material.Pipeline);
            
            SDL.GPUBufferBinding vertexBinding = new()
            {
                Buffer = renderable.VertexBuffer,
                Offset = 0
            };
            
            SDL.BindGPUVertexBuffers(gBufferPass, 0, new IntPtr(&vertexBinding), 1);

            SDL.GPUBufferBinding indexBinding = new()
            {
                Buffer = renderable.IndexBuffer,
                Offset = 0
            };
            
            SDL.BindGPUIndexBuffer(gBufferPass, in indexBinding, SDL.GPUIndexElementSize.IndexElementSize32Bit);

            SDL.DrawGPUIndexedPrimitives(gBufferPass, renderable.NumIndices, 1, 0, 0, 0);
        }
        
        SDL.EndGPURenderPass(gBufferPass);
        
        SdlUtils.PopDebugGroup(cb);
        
        #endregion

        #region Lighting Pass
        
        SdlUtils.PushDebugGroup(cb, "Deferred Lighting Pass");

        SDL.GPUColorTargetInfo compositeInfo = new()
        {
            Texture = compositeTarget,
            LoadOp = SDL.GPULoadOp.Clear,
            StoreOp = SDL.GPUStoreOp.Store,
            ClearColor = new SDL.FColor(0.0f, 0.0f, 0.0f, 1.0f)
        };

        IntPtr lightingPass = SDL.BeginGPURenderPass(cb, new IntPtr(&compositeInfo), 1, IntPtr.Zero)
            .Check("Begin lighting pass");
        
        SDL.PushGPUFragmentUniformData(cb, 0, new IntPtr(&camera), CameraMatrices.SizeInBytes);
        
        SDL.BindGPUGraphicsPipeline(lightingPass, _passPipeline);

        SDL.GPUTextureSamplerBinding* passBindings = stackalloc SDL.GPUTextureSamplerBinding[]
        {
            new SDL.GPUTextureSamplerBinding
            {
                Texture = _albedoTexture,
                Sampler = _passSampler
            },
            new SDL.GPUTextureSamplerBinding
            {
                Texture = _positionTexture,
                Sampler = _passSampler
            },
            new SDL.GPUTextureSamplerBinding
            {
                Texture = _normalTexture,
                Sampler = _passSampler
            },
            new SDL.GPUTextureSamplerBinding
            {
                Texture = _metallicRoughnessTexture,
                Sampler = _passSampler
            }
        };

        SDL.BindGPUFragmentSamplers(lightingPass, 0, (IntPtr) passBindings, 4);
        
        SDL.DrawGPUPrimitives(lightingPass, 6, 1, 0, 0);
        
        SDL.EndGPURenderPass(lightingPass);
        
        SdlUtils.PopDebugGroup(cb);

        #endregion
        
        // TODO: Multi camera support.
        _drawQueue.Clear();

        return true;
    }

    public void Resize(Size<int> newSize)
    {
        SDL.ReleaseGPUTexture(_device, _albedoTexture);
        SDL.ReleaseGPUTexture(_device, _positionTexture);
        SDL.ReleaseGPUTexture(_device, _normalTexture);
        SDL.ReleaseGPUTexture(_device, _metallicRoughnessTexture);

        _albedoTexture = CreateGBufferTexture(_device, newSize, GBufferFormat);
        _positionTexture = CreateGBufferTexture(_device, newSize, GBufferFormat);
        _normalTexture = CreateGBufferTexture(_device, newSize, GBufferFormat);
        _metallicRoughnessTexture = CreateGBufferTexture(_device, newSize, GBufferFormat);
        
        DebugTextures =
        [
            new Texture(_albedoTexture, newSize, "Albedo"),
            new Texture(_positionTexture, newSize, "Position"),
            new Texture(_normalTexture, newSize, "Normals"),
            new Texture(_metallicRoughnessTexture, newSize, "Metallic-Roughness-Occlusion")
        ];
    }
    
    public void Dispose()
    {
        SDL.ReleaseGPUSampler(_device, _passSampler);
        SDL.ReleaseGPUGraphicsPipeline(_device, _passPipeline);
        SDL.ReleaseGPUTexture(_device, _metallicRoughnessTexture);
        SDL.ReleaseGPUTexture(_device, _normalTexture);
        SDL.ReleaseGPUTexture(_device, _positionTexture);
        SDL.ReleaseGPUTexture(_device, _albedoTexture);
    }

    private GrTexture CreateGBufferTexture(Size<int> size)
    {
        return _device.CreateTexture(TextureInfo.Texture2D(GBufferFormat, size.ToGraphite(), 1,
            TextureUsage.ShaderResource | TextureUsage.ColorTarget));
    }
}