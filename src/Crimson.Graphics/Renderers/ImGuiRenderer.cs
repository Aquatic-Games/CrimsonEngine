using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Crimson.Core;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Hexa.NET.ImGui;
using SDL3;

namespace Crimson.Graphics.Renderers;

internal sealed class ImGuiRenderer : IDisposable
{
    private readonly IntPtr _device;
    
    private readonly ImGuiContextPtr _imguiContext;

    private uint _vBufferSize;
    private uint _iBufferSize;

    private IntPtr _vertexBuffer;
    private IntPtr _indexBuffer;
    private IntPtr _transferBuffer;

    private IntPtr _pipeline;

    private IntPtr? _texture;
    private IntPtr _sampler;

    public ImGuiContextPtr Context => _imguiContext;
    
    public unsafe ImGuiRenderer(IntPtr device, Size<int> size, SDL.GPUTextureFormat outFormat, RendererOptions.ImGuiInfo info)
    {
        _device = device;

        _imguiContext = ImGui.CreateContext();
        ImGui.SetCurrentContext(_imguiContext);

        _vBufferSize = 5000;
        _iBufferSize = 10000;

        uint vBufferSizeBytes = (uint) (_vBufferSize * sizeof(ImDrawVert));
        uint iBufferSizeBytes = _iBufferSize * sizeof(uint);
        
        _vertexBuffer = SdlUtils.CreateBuffer(_device, SDL.GPUBufferUsageFlags.Vertex, vBufferSizeBytes);
        _indexBuffer = SdlUtils.CreateBuffer(_device, SDL.GPUBufferUsageFlags.Index, iBufferSizeBytes);

        _transferBuffer = SdlUtils.CreateTransferBuffer(_device, SDL.GPUTransferBufferUsage.Upload,
            vBufferSizeBytes + iBufferSizeBytes);

        ShaderUtils.LoadGraphicsShader(_device, "Debug/ImGui", out IntPtr? vertexShader, out IntPtr? pixelShader);

        SDL.GPUColorTargetDescription targetDesc = new()
        {
            Format = outFormat,
            BlendState = SdlUtils.NonPremultipliedBlend
        };

        SDL.GPUVertexBufferDescription vertexBufferDesc = new()
        {
            InputRate = SDL.GPUVertexInputRate.Vertex,
            Slot = 0,
            InstanceStepRate = 0,
            Pitch = (uint) sizeof(ImDrawVert)
        };

        SDL.GPUVertexAttribute* vertexAttributes = stackalloc SDL.GPUVertexAttribute[]
        {
            new SDL.GPUVertexAttribute // Position
                { Format = SDL.GPUVertexElementFormat.Float2, Offset = 0, BufferSlot = 0, Location = 0 },
            new SDL.GPUVertexAttribute // TexCoord
                { Format = SDL.GPUVertexElementFormat.Float2, Offset = 8, BufferSlot = 0, Location = 1 },
            new SDL.GPUVertexAttribute // Color
                { Format = SDL.GPUVertexElementFormat.Ubyte4Norm, Offset = 16, BufferSlot = 0, Location = 2 }
        };

        SDL.GPUGraphicsPipelineCreateInfo pipelineInfo = new()
        {
            VertexShader = vertexShader.Value,
            FragmentShader = pixelShader.Value,
            TargetInfo = new SDL.GPUGraphicsPipelineTargetInfo()
            {
                NumColorTargets = 1,
                ColorTargetDescriptions = new IntPtr(&targetDesc)
            },
            VertexInputState = new SDL.GPUVertexInputState()
            {
                NumVertexBuffers = 1,
                VertexBufferDescriptions = new IntPtr(&vertexBufferDesc),
                NumVertexAttributes = 3,
                VertexAttributes = (nint) vertexAttributes
            },
            PrimitiveType = SDL.GPUPrimitiveType.TriangleList
        };

        _pipeline = SDL.CreateGPUGraphicsPipeline(_device, in pipelineInfo).Check("Create pipeline");
        
        SDL.ReleaseGPUShader(_device, pixelShader.Value);
        SDL.ReleaseGPUShader(_device, vertexShader.Value);

        SDL.GPUSamplerCreateInfo samplerInfo = new()
        {
            MinFilter = SDL.GPUFilter.Linear,
            MagFilter = SDL.GPUFilter.Linear,
            MipmapMode = SDL.GPUSamplerMipmapMode.Linear,
            AddressModeU = SDL.GPUSamplerAddressMode.Repeat,
            AddressModeV = SDL.GPUSamplerAddressMode.Repeat,
            MaxLod = 1000
        };

        _sampler = SDL.CreateGPUSampler(_device, in samplerInfo).Check("Create sampler");

        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(size.Width, size.Height);
        io.BackendFlags = ImGuiBackendFlags.RendererHasVtxOffset;
        io.IniFilename = null;
        io.LogFilename = null;
        
        if (info.Font != null)
        {
            Debug.Assert(info.FontSize != null);
            string path = Content.Content.GetFullyQualifiedName(info.Font);
            io.Fonts.AddFontFromFileTTF(path, info.FontSize.Value);
        }
        else
            io.Fonts.AddFontDefault();

        RecreateFontTexture();
        
        ImGui.NewFrame();
    }

    public unsafe bool Render(IntPtr cb, IntPtr colorTarget, bool shouldClear)
    {
        ImGui.SetCurrentContext(_imguiContext);
        
        ImGui.Render();
        ImDrawDataPtr drawData = ImGui.GetDrawData();

        // Don't bother rendering if there is nothing to draw.
        if (drawData.CmdListsCount == 0)
        {
            ImGui.NewFrame();
            return false;
        }
        
        SdlUtils.PushDebugGroup(cb, "ImGUI Buffer Copy");

        bool hasResizedBuffer = false;
        
        if (drawData.TotalVtxCount >= _vBufferSize)
        {
            Logger.Trace("Recreate vertex buffer.");
            SDL.ReleaseGPUBuffer(_device, _vertexBuffer);
            _vBufferSize = (uint) drawData.TotalVtxCount + 5000;
            _vertexBuffer = SdlUtils.CreateBuffer(_device, SDL.GPUBufferUsageFlags.Vertex,
                (uint) (_vBufferSize * sizeof(ImDrawVert)));
            hasResizedBuffer = true;
        }
        
        if (drawData.TotalIdxCount >= _iBufferSize)
        {
            Logger.Trace("Recreate index buffer.");
            SDL.ReleaseGPUBuffer(_device, _indexBuffer);
            _iBufferSize = (uint) drawData.TotalIdxCount + 10000;
            _indexBuffer = SdlUtils.CreateBuffer(_device, SDL.GPUBufferUsageFlags.Index, _iBufferSize * sizeof(uint));
            hasResizedBuffer = true;
        }

        if (hasResizedBuffer)
        {
            uint vBufferSizeBytes = (uint) (_vBufferSize * sizeof(ImDrawVert));
            uint iBufferSizeBytes = _iBufferSize * sizeof(uint);
            
            Logger.Trace("Recreate transfer buffer.");
            SDL.ReleaseGPUTransferBuffer(_device, _transferBuffer);
            _transferBuffer = SdlUtils.CreateTransferBuffer(_device, SDL.GPUTransferBufferUsage.Upload,
                vBufferSizeBytes + iBufferSizeBytes);
        }

        uint vertexOffset = 0;
        uint indexOffset = 0;

        void* mappedPtr =
            (void*) SDL.MapGPUTransferBuffer(_device, _transferBuffer, true).Check("Map transfer buffer");
        
        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[i];

            uint vertexSize = (uint) (cmdList.VtxBuffer.Size * sizeof(ImDrawVert));
            uint indexSize = (uint) (cmdList.IdxBuffer.Size * sizeof(ushort));

            Unsafe.CopyBlock((byte*) mappedPtr + vertexOffset, cmdList.VtxBuffer.Data, vertexSize);
            Unsafe.CopyBlock((byte*) mappedPtr + (_vBufferSize * sizeof(ImDrawVert)) + indexOffset,
                cmdList.IdxBuffer.Data, indexSize);

            vertexOffset += vertexSize;
            indexOffset += indexSize;
        }
        
        SDL.UnmapGPUTransferBuffer(_device, _transferBuffer);

        IntPtr copyPass = SDL.BeginGPUCopyPass(cb).Check("Begin copy pass");

        SDL.GPUTransferBufferLocation vertexSource = new()
        {
            TransferBuffer = _transferBuffer,
            Offset = 0
        };

        SDL.GPUBufferRegion vertexDest = new()
        {
            Buffer = _vertexBuffer,
            Offset = 0,
            Size = vertexOffset
        };
        
        SDL.UploadToGPUBuffer(copyPass, in vertexSource, in vertexDest, false);

        SDL.GPUTransferBufferLocation indexSource = new()
        {
            TransferBuffer = _transferBuffer,
            Offset = _vBufferSize * (uint) sizeof(ImDrawVert)
        };

        SDL.GPUBufferRegion indexDest = new()
        {
            Buffer = _indexBuffer,
            Offset = 0,
            Size = indexOffset
        };
        
        SDL.UploadToGPUBuffer(copyPass, in indexSource, in indexDest, false);
        
        SDL.EndGPUCopyPass(copyPass);
        
        SdlUtils.PopDebugGroup(cb);

        Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(drawData.DisplayPos.X,
            drawData.DisplayPos.X + drawData.DisplaySize.X, drawData.DisplayPos.Y + drawData.DisplaySize.Y,
            drawData.DisplayPos.Y, -1, 1);
        
        SDL.PushGPUVertexUniformData(cb, 0, new IntPtr(&projection), 64);
        
        SdlUtils.PushDebugGroup(cb, "ImGUI Pass");

        SDL.GPUColorTargetInfo targetInfo = new()
        {
            Texture = colorTarget,
            ClearColor = new SDL.FColor(0.0f, 0.0f, 0.0f, 1.0f),
            LoadOp = shouldClear ? SDL.GPULoadOp.Clear : SDL.GPULoadOp.Load,
            StoreOp = SDL.GPUStoreOp.Store
        };

        IntPtr renderPass = SDL.BeginGPURenderPass(cb, new IntPtr(&targetInfo), 1, IntPtr.Zero)
            .Check("Begin render pass");
        
        SDL.BindGPUGraphicsPipeline(renderPass, _pipeline);

        SDL.GPUViewport viewport = new()
        {
            X = drawData.DisplayPos.X,
            Y = drawData.DisplayPos.Y,
            W = drawData.DisplaySize.X,
            H = drawData.DisplaySize.Y,
            MinDepth = 0,
            MaxDepth = 1
        };
        SDL.SetGPUViewport(renderPass, in viewport);

        SDL.GPUBufferBinding vertexBinding = new()
        {
            Buffer = _vertexBuffer,
            Offset = 0
        };
        
        SDL.BindGPUVertexBuffers(renderPass, 0, new IntPtr(&vertexBinding), 1);

        SDL.GPUBufferBinding indexBinding = new()
        {
            Buffer = _indexBuffer,
            Offset = 0
        };

        SDL.BindGPUIndexBuffer(renderPass, in indexBinding, SDL.GPUIndexElementSize.IndexElementSize16Bit);

        vertexOffset = 0;
        indexOffset = 0;
        Vector2 clipOff = drawData.DisplayPos;

        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[i];

            for (int j = 0; j < cmdList.CmdBuffer.Size; j++)
            {
                ImDrawCmd drawCmd = cmdList.CmdBuffer[j];
                
                if (drawCmd.UserCallback != null)
                    continue;

                IntPtr texture = _texture!.Value;

                if (drawCmd.TextureId != ImTextureID.Null)
                    texture = (IntPtr) drawCmd.TextureId.Handle;
                
                Vector2 clipMin = new Vector2(drawCmd.ClipRect.X - clipOff.X, drawCmd.ClipRect.Y - clipOff.Y);
                Vector2 clipMax = new Vector2(drawCmd.ClipRect.Z - clipOff.X, drawCmd.ClipRect.W - clipOff.Y);
                
                if (clipMax.X <= clipMin.X || clipMax.Y <= clipMin.Y)
                    continue;

                SDL.Rect scissorRect = new()
                {
                    X = (int) clipMin.X,
                    Y = (int) clipMin.Y,
                    W = (int) clipMax.X - (int) clipMin.X,
                    H = (int) clipMax.Y - (int) clipMin.Y
                };

                SDL.SetGPUScissor(renderPass, in scissorRect);

                SDL.GPUTextureSamplerBinding samplerBinding = new()
                {
                    Texture = texture,
                    Sampler = _sampler
                };

                SDL.BindGPUFragmentSamplers(renderPass, 0, new IntPtr(&samplerBinding), 1);

                SDL.DrawGPUIndexedPrimitives(renderPass, drawCmd.ElemCount, 1, drawCmd.IdxOffset + indexOffset,
                    (short) (drawCmd.VtxOffset + vertexOffset), 0);
            }
            
            vertexOffset += (uint) cmdList.VtxBuffer.Size;
            indexOffset += (uint) cmdList.IdxBuffer.Size;
        }
        
        SDL.EndGPURenderPass(renderPass);
        
        SdlUtils.PopDebugGroup(cb);
        
        ImGui.NewFrame();

        return true;
    }

    public void Resize(Size<int> size)
    {
        ImGui.GetIO().DisplaySize = new Vector2(size.Width, size.Height);
    }

    private unsafe void RecreateFontTexture()
    {
        if (_texture != null)
            SDL.ReleaseGPUTexture(_device, _texture.Value);

        ImGuiIOPtr io = ImGui.GetIO();
        byte* imagePixels;
        int width, height;
        io.Fonts.GetTexDataAsRGBA32(&imagePixels, &width, &height);

        _texture = SdlUtils.CreateTexture2D(_device, (nint) imagePixels, (uint) width, (uint) height,
            SDL.GPUTextureFormat.R8G8B8A8Unorm, 1);
    }

    public void Dispose()
    {
        SDL.ReleaseGPUTexture(_device, _texture!.Value);
        SDL.ReleaseGPUSampler(_device, _sampler);
        SDL.ReleaseGPUGraphicsPipeline(_device, _pipeline);
        SDL.ReleaseGPUTransferBuffer(_device, _transferBuffer);
        SDL.ReleaseGPUBuffer(_device, _indexBuffer);
        SDL.ReleaseGPUBuffer(_device, _vertexBuffer);
        
        ImGui.DestroyContext(_imguiContext);
    }
}