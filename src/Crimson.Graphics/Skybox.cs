using System.Diagnostics;
using System.Runtime.CompilerServices;
using Crimson.Content;
using Crimson.Core;
using Crimson.Graphics.Primitives;
using Crimson.Graphics.Renderers.Structs;
using Crimson.Graphics.Utils;
using SDL3;

namespace Crimson.Graphics;

public sealed class Skybox : IContentResource<Skybox>, IDisposable
{
    private readonly IntPtr _device;
    
    private readonly IntPtr _textureHandle;
    private readonly IntPtr _sampler;

    private readonly IntPtr _vertexBuffer;
    private readonly IntPtr _indexBuffer;

    private readonly IntPtr _pipeline;

    /// <summary>
    /// Create a <see cref="Skybox"/> consisting of 6 textures in a cube.
    /// </summary>
    /// <param name="right">The right (X+) bitmap image.</param>
    /// <param name="left">The left (X-) bitmap image.</param>
    /// <param name="up">The up (Y+) bitmap image.</param>
    /// <param name="down">The down (Y-) bitmap image.</param>
    /// <param name="front">The back (Z+) bitmap image.</param>
    /// <param name="back">The front (Z-) bitmap image.</param>
    public unsafe Skybox(Bitmap right, Bitmap left, Bitmap up, Bitmap down, Bitmap front, Bitmap back)
    {
        Debug.Assert(right.Size == left.Size);
        Debug.Assert(right.Size == up.Size);
        Debug.Assert(right.Size == down.Size);
        Debug.Assert(right.Size == front.Size);
        Debug.Assert(right.Size == back.Size);
        
        _device = Renderer.Device;

        SDL.GPUTextureFormat fmt = right.Format.ToSdl(out uint rowPitch);

        SDL.GPUTextureCreateInfo textureInfo = new()
        {
            Format = fmt,
            Width = (uint) right.Size.Width,
            Height = (uint) right.Size.Height,
            LayerCountOrDepth = 6,
            NumLevels = SdlUtils.CalculateMipLevels((uint) right.Size.Width, (uint) right.Size.Height),
            Type = SDL.GPUTextureType.TexturetypeCube,
            Usage = SDL.GPUTextureUsageFlags.Sampler | SDL.GPUTextureUsageFlags.ColorTarget,
            SampleCount = SDL.GPUSampleCount.SampleCount1
        };

        _textureHandle = SDL.CreateGPUTexture(_device, in textureInfo).Check("Create texture");

        SDL.GPUSamplerCreateInfo samplerInfo = new()
        {
            MinFilter = SDL.GPUFilter.Linear,
            MagFilter = SDL.GPUFilter.Linear,
            MipmapMode = SDL.GPUSamplerMipmapMode.Linear,
            AddressModeU = SDL.GPUSamplerAddressMode.ClampToEdge,
            AddressModeV = SDL.GPUSamplerAddressMode.ClampToEdge,
            AddressModeW = SDL.GPUSamplerAddressMode.ClampToEdge,
            MaxLod = 1000
        };

        _sampler = SDL.CreateGPUSampler(_device, in samplerInfo).Check("Create sampler");

        // Multiply by 6 as we are uploading 6 textures.
        uint textureSize = (uint) right.Size.Width * (uint) right.Size.Height * rowPitch;
        SDL.GPUTransferBufferCreateInfo transferInfo = new()
        {
            Size = textureSize * 6,
            Usage = SDL.GPUTransferBufferUsage.Upload
        };

        IntPtr transferBuffer = SDL.CreateGPUTransferBuffer(_device, in transferInfo).Check("Create transfer buffer");

        void* data = (void*) SDL.MapGPUTransferBuffer(_device, transferBuffer, false).Check("Map transfer buffer");
        
        // lol
        fixed (void* pBitmap0 = right.Data)
        fixed (void* pBitmap1 = left.Data)
        fixed (void* pBitmap2 = up.Data)
        fixed (void* pBitmap3 = down.Data)
        fixed (void* pBitmap4 = front.Data)
        fixed (void* pBitmap5 = back.Data)
        {
            Unsafe.CopyBlock((byte*) data + textureSize * 0, pBitmap0, textureSize);
            Unsafe.CopyBlock((byte*) data + textureSize * 1, pBitmap1, textureSize);
            Unsafe.CopyBlock((byte*) data + textureSize * 2, pBitmap2, textureSize);
            Unsafe.CopyBlock((byte*) data + textureSize * 3, pBitmap3, textureSize);
            Unsafe.CopyBlock((byte*) data + textureSize * 4, pBitmap4, textureSize);
            Unsafe.CopyBlock((byte*) data + textureSize * 5, pBitmap5, textureSize);
        }
        
        SDL.UnmapGPUTransferBuffer(_device, transferBuffer);

        IntPtr cb = SDL.AcquireGPUCommandBuffer(_device).Check("Acquire command buffer");
        IntPtr copyPass = SDL.BeginGPUCopyPass(cb).Check("Begin copy pass");

        SDL.GPUTextureTransferInfo source = new()
        {
            TransferBuffer = transferBuffer,
            PixelsPerRow = (uint) right.Size.Width,
            RowsPerLayer = (uint) right.Size.Height,
            Offset = 0
        };

        for (uint i = 0; i < 6; i++)
        {
            source.Offset = textureSize * i;
            
            SDL.UploadToGPUTexture(copyPass, in source,
                new SDL.GPUTextureRegion()
                {
                    Texture = _textureHandle,
                    X = 0,
                    Y = 0,
                    W = (uint) right.Size.Width,
                    H = (uint) right.Size.Height,
                    D = 1,
                    MipLevel = 0,
                    Layer = i
                }, false);
        }

        SDL.EndGPUCopyPass(copyPass);
        SDL.SubmitGPUCommandBuffer(cb).Check("Submit command buffer");

        Renderer.MipmapQueue.Add(_textureHandle);
        
        SDL.ReleaseGPUTransferBuffer(_device, transferBuffer);

        Cube cube = new Cube();
        _vertexBuffer = SdlUtils.CreateBuffer(_device, SDL.GPUBufferUsageFlags.Vertex, cube.Vertices);
        _indexBuffer = SdlUtils.CreateBuffer(_device, SDL.GPUBufferUsageFlags.Index, cube.Indices);

        ShaderUtils.LoadGraphicsShader(_device, "Environment/Skybox", out IntPtr? vertexShader,
            out IntPtr? pixelShader);

        SDL.GPUColorTargetDescription colorTarget = new()
        {
            Format = Renderer.MainTargetFormat
        };

        SDL.GPUVertexBufferDescription vertexBufferDesc = new()
        {
            InputRate = SDL.GPUVertexInputRate.Vertex,
            Pitch = Vertex.SizeInBytes,
            InstanceStepRate = 0,
            Slot = 0
        };

        SDL.GPUVertexAttribute vertexAttribute = new()
        {
            Format = SDL.GPUVertexElementFormat.Float3,
            Offset = 0,
            Location = 0,
            BufferSlot = 0
        };
        
        SDL.GPUGraphicsPipelineCreateInfo pipelineInfo = new()
        {
            VertexShader = vertexShader.Value,
            FragmentShader = pixelShader.Value,
            TargetInfo = new SDL.GPUGraphicsPipelineTargetInfo()
            {
                NumColorTargets = 1,
                ColorTargetDescriptions = new IntPtr(&colorTarget),
                HasDepthStencilTarget = 1,
                DepthStencilFormat = SDL.GPUTextureFormat.D32Float
            },
            VertexInputState = new SDL.GPUVertexInputState()
            {
                NumVertexBuffers = 1,
                VertexBufferDescriptions = new IntPtr(&vertexBufferDesc),
                NumVertexAttributes = 1,
                VertexAttributes = new IntPtr(&vertexAttribute)
            },
            PrimitiveType = SDL.GPUPrimitiveType.TriangleList,
            DepthStencilState = new SDL.GPUDepthStencilState()
            {
                EnableDepthTest = 1,
                EnableDepthWrite = 0,
                CompareOp = SDL.GPUCompareOp.LessOrEqual
            },
            RasterizerState = new SDL.GPURasterizerState()
            {
                CullMode = SDL.GPUCullMode.Front,
                FrontFace = SDL.GPUFrontFace.CounterClockwise,
                FillMode = SDL.GPUFillMode.Fill
            }
        };

        _pipeline = SDL.CreateGPUGraphicsPipeline(_device, in pipelineInfo).Check("Create pipeline");
        
        SDL.ReleaseGPUShader(_device, pixelShader.Value);
        SDL.ReleaseGPUShader(_device, vertexShader.Value);
    }

    internal unsafe bool Render(IntPtr cb, IntPtr texture, IntPtr depthTarget, bool shouldClear, CameraMatrices matrices)
    {
        SDL.GPUColorTargetInfo targetInfo = new()
        {
            Texture = texture,
            ClearColor = new SDL.FColor(0.0f, 0.0f, 0.0f, 1.0f),
            LoadOp = shouldClear ? SDL.GPULoadOp.Clear : SDL.GPULoadOp.Load,
            StoreOp = SDL.GPUStoreOp.Store
        };
        
        SDL.GPUDepthStencilTargetInfo depthTargetInfo = new()
        {
            Texture = depthTarget,
            LoadOp = shouldClear ? SDL.GPULoadOp.Clear : SDL.GPULoadOp.Load,
            StoreOp = SDL.GPUStoreOp.Store,
            ClearDepth = 1.0f
        };
        
        SdlUtils.PushDebugGroup(cb, "Skybox Pass");
        
        IntPtr pass = SDL.BeginGPURenderPass(cb, new IntPtr(&targetInfo), 1, in depthTargetInfo)
            .Check("Begin render pass");
        
        SDL.PushGPUVertexUniformData(cb, 0, new IntPtr(&matrices), CameraMatrices.SizeInBytes);

        SDL.GPUTextureSamplerBinding samplerBinding = new()
        {
            Sampler = _sampler,
            Texture = _textureHandle
        };
        
        SDL.BindGPUFragmentSamplers(pass, 0, new IntPtr(&samplerBinding), 1);
        
        SDL.BindGPUGraphicsPipeline(pass, _pipeline);

        SDL.GPUBufferBinding vertexBinding = new()
        {
            Buffer = _vertexBuffer,
            Offset = 0
        };
        
        SDL.BindGPUVertexBuffers(pass, 0, new IntPtr(&vertexBinding), 1);

        SDL.GPUBufferBinding indexBinding = new()
        {
            Buffer = _indexBuffer,
            Offset = 0
        };
        
        SDL.BindGPUIndexBuffer(pass, in indexBinding, SDL.GPUIndexElementSize.IndexElementSize32Bit);
        
        SDL.DrawGPUIndexedPrimitives(pass, 36, 1, 0, 0, 0);
        
        SDL.EndGPURenderPass(pass);
        
        SdlUtils.PopDebugGroup(cb);

        return true;
    }
    
    public void Dispose()
    {
        SDL.ReleaseGPUGraphicsPipeline(_device, _pipeline);
        SDL.ReleaseGPUBuffer(_device, _indexBuffer);
        SDL.ReleaseGPUBuffer(_device, _vertexBuffer);
        SDL.ReleaseGPUSampler(_device, _sampler);
        SDL.ReleaseGPUTexture(_device, _textureHandle);
    }

    public static Skybox LoadResource(string fullPath, bool hasExtension)
    {
        if (hasExtension)
            throw new NotImplementedException();

        if (Directory.Exists(fullPath))
        {
            string right = Path.Combine(fullPath, Path.ChangeExtension("right", ".png"));
            string left = Path.Combine(fullPath, Path.ChangeExtension("left", ".png"));
            string up = Path.Combine(fullPath, Path.ChangeExtension("top", ".png"));
            string down = Path.Combine(fullPath, Path.ChangeExtension("bottom", ".png"));
            string front = Path.Combine(fullPath, Path.ChangeExtension("front", ".png"));
            string back = Path.Combine(fullPath, Path.ChangeExtension("back", ".png"));
            
            return new Skybox(
                new Bitmap(right),
                new Bitmap(left),
                new Bitmap(up),
                new Bitmap(down),
                new Bitmap(front),
                new Bitmap(back)
            );
        }
        else
            throw new NotImplementedException();
    }
}