using System.Numerics;
using System.Runtime.CompilerServices;
using Crimson.Core;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Graphite;
using Graphite.Core;
using Hexa.NET.ImGui;
using Buffer = Graphite.Buffer;

namespace Crimson.Graphics.Renderers;

internal sealed class ImGuiRenderer : IDisposable
{
    private readonly Device _device;
    
    private readonly ImGuiContextPtr _imguiContext;

    private uint _vBufferSize;
    private uint _iBufferSize;

    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;
    
    private readonly Buffer _projBuffer;
    private readonly DescriptorLayout _projLayout;
    private readonly DescriptorSet _projSet;

    private readonly DescriptorLayout _textureLayout;

    private readonly Pipeline _pipeline;

    private GrTexture? _texture;
    private readonly Sampler _sampler;

    public ImGuiContextPtr Context => _imguiContext;
    
    public unsafe ImGuiRenderer(Device device, Size<int> size, Format outFormat)
    {
        _device = device;

        _imguiContext = ImGui.CreateContext();
        ImGui.SetCurrentContext(_imguiContext);

        _vBufferSize = 5000;
        _iBufferSize = 10000;

        uint vBufferSizeBytes = (uint) (_vBufferSize * sizeof(ImDrawVert));
        uint iBufferSizeBytes = _iBufferSize * sizeof(ushort);
        
        _vertexBuffer = _device.CreateBuffer(new BufferInfo(BufferUsage.VertexBuffer | BufferUsage.MapWrite, vBufferSizeBytes));
        _indexBuffer = _device.CreateBuffer(new BufferInfo(BufferUsage.IndexBuffer | BufferUsage.MapWrite, iBufferSizeBytes));
        _projBuffer = _device.CreateBuffer(new BufferInfo(BufferUsage.ConstantBuffer | BufferUsage.MapWrite, (uint) sizeof(Matrix4x4)));

        ShaderUtils.LoadGraphicsShader(_device, "Debug/ImGui", out ShaderModule? vertexShader, out ShaderModule? pixelShader);

        _projLayout = _device.CreateDescriptorLayout(new DescriptorLayoutInfo
        {
            Bindings = [new DescriptorBinding(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex)]
        });

        _projSet = _device.CreateDescriptorSet(_projLayout,
            new Descriptor(0, DescriptorType.ConstantBuffer, buffer: _projBuffer));

        _textureLayout = _device.CreateDescriptorLayout(new DescriptorLayoutInfo
        {
            Bindings = [new DescriptorBinding(0, DescriptorType.Texture, ShaderStage.Pixel)],
            PushDescriptor = true
        });
        
        GraphicsPipelineInfo pipelineInfo = new()
        {
            VertexShader = vertexShader,
            PixelShader = pixelShader,
            ColorTargets = [new ColorTargetInfo(outFormat, BlendStateDescription.NonPremultipliedAlpha)],
            InputLayout =
            [
                new InputElementDescription(Format.R32G32_Float, 0, 0, 0),
                new InputElementDescription(Format.R32G32_Float, 8, 1, 0),
                new InputElementDescription(Format.R8G8B8A8_UNorm, 16, 2, 0)
            ],
            Descriptors = [_projLayout, _textureLayout]
        };

        _pipeline = _device.CreateGraphicsPipeline(in pipelineInfo);
        
        pixelShader.Dispose();
        vertexShader.Dispose();

        _sampler = _device.CreateSampler(SamplerInfo.LinearWrap);

        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(size.Width, size.Height);
        io.BackendFlags = ImGuiBackendFlags.RendererHasVtxOffset;
        io.IniFilename = null;
        io.LogFilename = null;
        
        io.Fonts.AddFontDefault();
        RecreateFontTexture();
        
        ImGui.NewFrame();
    }

    public unsafe bool Render(CommandList cl, GrTexture colorTarget, bool shouldClear)
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
        
        //SdlUtils.PushDebugGroup(cb, "ImGUI Buffer Copy");
        
        if (drawData.TotalVtxCount >= _vBufferSize)
        {
            Logger.Trace("Recreate vertex buffer.");
            _vertexBuffer.Dispose();
            _vBufferSize = (uint) drawData.TotalVtxCount + 5000;
            _vertexBuffer = _device.CreateBuffer(new BufferInfo(BufferUsage.VertexBuffer | BufferUsage.MapWrite, (uint) (_vBufferSize * sizeof(ImDrawVert))));
        }
        
        if (drawData.TotalIdxCount >= _iBufferSize)
        {
            Logger.Trace("Recreate index buffer.");
            _indexBuffer.Dispose();
            _iBufferSize = (uint) drawData.TotalIdxCount + 10000;
            _indexBuffer = _device.CreateBuffer(new BufferInfo(BufferUsage.IndexBuffer | BufferUsage.MapWrite, _iBufferSize * sizeof(ushort)));
        }

        uint vertexOffset = 0;
        uint indexOffset = 0;

        void* vMap = (void*) _device.MapBuffer(_vertexBuffer);
        void* iMap = (void*) _device.MapBuffer(_indexBuffer);
        
        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[i];

            uint vertexSize = (uint) (cmdList.VtxBuffer.Size * sizeof(ImDrawVert));
            uint indexSize = (uint) (cmdList.IdxBuffer.Size * sizeof(ushort));

            Unsafe.CopyBlock((byte*) vMap + vertexOffset, cmdList.VtxBuffer.Data, vertexSize);
            Unsafe.CopyBlock((byte*) iMap + indexOffset, cmdList.IdxBuffer.Data, indexSize);

            vertexOffset += vertexSize;
            indexOffset += indexSize;
        }

        _device.UnmapBuffer(_indexBuffer);
        _device.UnmapBuffer(_vertexBuffer);
        
        //SdlUtils.PopDebugGroup(cb);

        Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(drawData.DisplayPos.X,
            drawData.DisplayPos.X + drawData.DisplaySize.X, drawData.DisplayPos.Y + drawData.DisplaySize.Y,
            drawData.DisplayPos.Y, -1, 1);
        
        _device.UpdateBuffer(_projBuffer, 0, projection);
        
        //SdlUtils.PushDebugGroup(cb, "ImGUI Pass");

        cl.BeginRenderPass(new ColorAttachmentInfo
        {
            Texture = colorTarget,
            ClearColor = new ColorF(0, 0, 0),
            LoadOp = shouldClear ? LoadOp.Clear : LoadOp.Load,
            StoreOp = StoreOp.Store
        });
        
        cl.SetGraphicsPipeline(_pipeline);

        Viewport viewport = new()
        {
            X = drawData.DisplayPos.X,
            Y = drawData.DisplayPos.Y,
            Width = drawData.DisplaySize.X,
            Height = drawData.DisplaySize.Y,
            MinDepth = 0,
            MaxDepth = 1
        };
        cl.SetViewport(in viewport);

        cl.SetVertexBuffer(0, _vertexBuffer, (uint) sizeof(ImDrawVert));
        cl.SetIndexBuffer(_indexBuffer, Format.R16_UInt);
        
        cl.SetDescriptorSet(0, _pipeline, _projSet);

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

                if (drawCmd.TextureId != ImTextureID.Null)
                    throw new NotImplementedException();
                
                Vector2 clipMin = new Vector2(drawCmd.ClipRect.X - clipOff.X, drawCmd.ClipRect.Y - clipOff.Y);
                Vector2 clipMax = new Vector2(drawCmd.ClipRect.Z - clipOff.X, drawCmd.ClipRect.W - clipOff.Y);
                
                if (clipMax.X <= clipMin.X || clipMax.Y <= clipMin.Y)
                    continue;

                Rect2D scissorRect = new()
                {
                    X = (int) clipMin.X,
                    Y = (int) clipMin.Y,
                    Width = (uint) (clipMax.X - clipMin.X),
                    Height = (uint) (clipMax.Y - (int) clipMin.Y)
                };
                cl.SetScissor(in scissorRect);

                cl.PushDescriptors(1, _pipeline,
                    new Descriptor(0, DescriptorType.Texture, texture: _texture, sampler: _sampler));

                cl.DrawIndexed(drawCmd.ElemCount, drawCmd.IdxOffset + indexOffset,
                    (int) (drawCmd.VtxOffset + vertexOffset));
            }
            
            vertexOffset += (uint) cmdList.VtxBuffer.Size;
            indexOffset += (uint) cmdList.IdxBuffer.Size;
        }
        
        cl.EndRenderPass();
        
        //SdlUtils.PopDebugGroup(cb);
        
        ImGui.NewFrame();

        return true;
    }

    public void Resize(Size<int> size)
    {
        ImGui.GetIO().DisplaySize = new Vector2(size.Width, size.Height);
    }

    private unsafe void RecreateFontTexture()
    {
        _texture?.Dispose();

        ImGuiIOPtr io = ImGui.GetIO();
        byte* imagePixels;
        int width, height;
        io.Fonts.GetTexDataAsRGBA32(&imagePixels, &width, &height);

        _texture = _device.CreateTexture(
            TextureInfo.Texture2D(Format.R8G8B8A8_UNorm, new Size2D((uint) width, (uint) height), 1,
                TextureUsage.ShaderResource), imagePixels);
    }

    public void Dispose()
    {
        _texture?.Dispose();
        
        _sampler.Dispose();
        _pipeline.Dispose();
        
        _textureLayout.Dispose();
        _projSet.Dispose();
        _projLayout.Dispose();
        
        _projBuffer.Dispose();
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
        
        ImGui.DestroyContext(_imguiContext);
    }
}