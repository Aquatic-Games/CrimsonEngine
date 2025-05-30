using System.Numerics;
using Crimson.Graphics.Renderers.Structs;
using Crimson.Graphics.Utils;
using Crimson.Math;
using SDL3;

namespace Crimson.Graphics.Renderers;

internal class DeferredRenderer : IDisposable
{
    // Most of the gbuffers are just representing RGBA8 pixel data. Only the position buffer needs any higher accuracy.
    // As such using R8G8B8A8_UNorm when possible significantly reduces data throughput.
    private const SDL.GPUTextureFormat GBufferInputFormat = SDL.GPUTextureFormat.R8G8B8A8Unorm;
    private const SDL.GPUTextureFormat GBufferPositionFormat = SDL.GPUTextureFormat.R32G32B32A32Float;
    
    private readonly IntPtr _device;
    
    private IntPtr _albedoTexture;
    private IntPtr _positionTexture;
    private IntPtr _normalTexture;
    private IntPtr _metallicRoughnessTexture;

    private readonly IntPtr _passPipeline;
    private readonly IntPtr _passSampler;

    private readonly List<WorldRenderable> _drawQueue;
    
    public unsafe DeferredRenderer(IntPtr device, Size<int> size, SDL.GPUTextureFormat outFormat)
    {
        _device = device;

        _albedoTexture = CreateGBufferTexture(_device, size, GBufferInputFormat);
        SDL.SetGPUTextureName(_device, _albedoTexture, "Albedo GBuffer");
        _positionTexture = CreateGBufferTexture(_device, size, GBufferPositionFormat);
        SDL.SetGPUTextureName(_device, _positionTexture, "Position GBuffer");
        _normalTexture = CreateGBufferTexture(_device, size, GBufferInputFormat);
        SDL.SetGPUTextureName(_device, _normalTexture, "Normal GBuffer");
        _metallicRoughnessTexture = CreateGBufferTexture(_device, size, GBufferInputFormat);
        SDL.SetGPUTextureName(_device, _metallicRoughnessTexture, "Metallic-Roughness-Occlusion GBuffer");

        IntPtr passVtx = ShaderUtils.LoadGraphicsShader(device, SDL.GPUShaderStage.Vertex, "Deferred/DeferredPass",
            "VSMain", 0, 0);
        IntPtr passPxl = ShaderUtils.LoadGraphicsShader(device, SDL.GPUShaderStage.Fragment, "Deferred/DeferredPass",
            "PSMain", 0, 1);

        SDL.GPUColorTargetDescription targetDesc = new()
        {
            Format = outFormat
        };

        SDL.GPUGraphicsPipelineCreateInfo passPipelineInfo = new()
        {
            VertexShader = passVtx,
            FragmentShader = passPxl,
            TargetInfo = new SDL.GPUGraphicsPipelineTargetInfo()
            {
                NumColorTargets = 1,
                ColorTargetDescriptions = new IntPtr(&targetDesc)
            },
            PrimitiveType = SDL.GPUPrimitiveType.TriangleList
        };

        _passPipeline = SDL.CreateGPUGraphicsPipeline(_device, in passPipelineInfo).Check("Create graphics pipeline");
        
        SDL.ReleaseGPUShader(_device, passPxl);
        SDL.ReleaseGPUShader(_device, passVtx);

        SDL.GPUSamplerCreateInfo samplerInfo = new()
        {
            MinFilter = SDL.GPUFilter.Linear,
            MagFilter = SDL.GPUFilter.Linear,
            MipmapMode = SDL.GPUSamplerMipmapMode.Linear,
            AddressModeU = SDL.GPUSamplerAddressMode.ClampToEdge,
            AddressModeV = SDL.GPUSamplerAddressMode.ClampToEdge
        };

        _passSampler = SDL.CreateGPUSampler(_device, in samplerInfo).Check("Create sampler");

        _drawQueue = [];
    }

    public void AddToQueue(Renderable renderable, Matrix4x4 worldMatrix)
    {
        _drawQueue.Add(new WorldRenderable(renderable, worldMatrix));
    }

    public unsafe bool Render(IntPtr cb, IntPtr compositeTarget, IntPtr depthTexture, CameraMatrices camera)
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
        
        SDL.BindGPUGraphicsPipeline(lightingPass, _passPipeline);

        SDL.GPUTextureSamplerBinding albedoTargetBinding = new()
        {
            Texture = _albedoTexture,
            Sampler = _passSampler
        };

        SDL.BindGPUFragmentSamplers(lightingPass, 0, new IntPtr(&albedoTargetBinding), 1);
        
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

        _albedoTexture = CreateGBufferTexture(_device, newSize, GBufferInputFormat);
        _positionTexture = CreateGBufferTexture(_device, newSize, GBufferPositionFormat);
        _normalTexture = CreateGBufferTexture(_device, newSize, GBufferInputFormat);
        _metallicRoughnessTexture = CreateGBufferTexture(_device, newSize, GBufferInputFormat);
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

    private static IntPtr CreateGBufferTexture(IntPtr device, Size<int> size, SDL.GPUTextureFormat format)
    {
        return SdlUtils.CreateTexture2D(device, (uint) size.Width, (uint) size.Height, format, 1,
            SDL.GPUTextureUsageFlags.Sampler | SDL.GPUTextureUsageFlags.ColorTarget);
    }
}