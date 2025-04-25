using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Crimson.Core;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Hexa.NET.ImGui;
using SDL3;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

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

    public ImGuiContextPtr Context => _imguiContext;
    
    public unsafe ImGuiRenderer(IntPtr device, Size<int> size, SDL.GPUTextureFormat outFormat)
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

        IntPtr vertexShader =
            ShaderUtils.LoadGraphicsShader(_device, SDL.GPUShaderStage.Vertex, "Debug/ImGui", "VSMain", 1, 0);
        IntPtr pixelShader =
            ShaderUtils.LoadGraphicsShader(_device, SDL.GPUShaderStage.Fragment, "Debug/ImGui", "PSMain", 0, 1);

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
                { Format = SDL.GPUVertexElementFormat.Byte4Norm, Offset = 16, BufferSlot = 0, Location = 2 }
        };

        SDL.GPUGraphicsPipelineCreateInfo pipelineInfo = new()
        {
            VertexShader = vertexShader,
            FragmentShader = pixelShader,
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

        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(size.Width, size.Height);
        io.BackendFlags = ImGuiBackendFlags.RendererHasVtxOffset;
        io.IniFilename = null;
        io.LogFilename = null;
        
        io.Fonts.AddFontDefault();
        RecreateFontTexture();
        
        ImGui.NewFrame();
    }

    public unsafe void Render(IntPtr cb)
    {
        ImGui.SetCurrentContext(_imguiContext);
        
        ImGui.Render();
        ImDrawDataPtr drawData = ImGui.GetDrawData();

        if (drawData.TotalVtxCount >= _vBufferSize)
        {
            throw new NotImplementedException();
            Logger.Trace("Recreate vertex buffer.");
            SDL.ReleaseGPUBuffer(_device, _vertexBuffer);
            _vBufferSize = (uint) drawData.TotalVtxCount + 5000;
            _vertexBuffer = SdlUtils.CreateBuffer(_device, SDL.GPUBufferUsageFlags.Vertex,
                (uint) (_vBufferSize * sizeof(ImDrawVert)));
        }
        
        if (drawData.TotalIdxCount >= _iBufferSize)
        {
            throw new NotImplementedException();
            Logger.Trace("Recreate index buffer.");
            SDL.ReleaseGPUBuffer(_device, _indexBuffer);
            _iBufferSize = (uint) drawData.TotalIdxCount + 10000;
            _indexBuffer = SdlUtils.CreateBuffer(_device, SDL.GPUBufferUsageFlags.Index, _iBufferSize * sizeof(uint));
        }

        uint vertexOffset = 0;
        uint indexOffset = 0;

        MappedSubresource vMap = context.Map(_vertexBuffer, MapMode.WriteDiscard);
        MappedSubresource iMap = context.Map(_indexBuffer, MapMode.WriteDiscard);
        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[i];

            uint vertexSize = (uint) (cmdList.VtxBuffer.Size * sizeof(ImDrawVert));
            uint indexSize = (uint) (cmdList.IdxBuffer.Size * sizeof(ushort));

            Unsafe.CopyBlock((byte*) vMap.DataPointer + vertexOffset, cmdList.VtxBuffer.Data, vertexSize);
            Unsafe.CopyBlock((byte*) iMap.DataPointer + indexOffset, cmdList.IdxBuffer.Data, indexSize);

            vertexOffset += vertexSize;
            indexOffset += indexSize;
        }
        context.Unmap(_indexBuffer);
        context.Unmap(_vertexBuffer);

        context.UpdateBuffer(_cameraBuffer,
            Matrix4x4.CreateOrthographicOffCenter(drawData.DisplayPos.X, drawData.DisplayPos.X + drawData.DisplaySize.X,
                drawData.DisplayPos.Y + drawData.DisplaySize.Y, drawData.DisplayPos.Y, -1, 1));
        
        context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
        context.RSSetViewport(drawData.DisplayPos.X, drawData.DisplayPos.Y, drawData.DisplaySize.X, drawData.DisplaySize.Y);
        context.IASetInputLayout(_inputLayout);
        
        context.VSSetShader(_vertexShader);
        context.PSSetShader(_pixelShader);
        
        context.OMSetDepthStencilState(_depthState);
        context.RSSetState(_rasterizerState);
        context.OMSetBlendState(_blendState);
        context.PSSetSampler(0, _samplerState);
        
        context.IASetVertexBuffer(0, _vertexBuffer, (uint) sizeof(ImDrawVert));
        context.IASetIndexBuffer(_indexBuffer, Format.R16_UInt, 0);
        context.VSSetConstantBuffer(0, _cameraBuffer);

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

                context.RSSetScissorRect((int) clipMin.X, (int) clipMin.Y, (int) clipMax.X - (int) clipMin.X,
                    (int) clipMax.Y - (int) clipMin.Y);
                
                context.PSSetShaderResource(0, _resourceView!);

                context.DrawIndexed(drawCmd.ElemCount, drawCmd.IdxOffset + indexOffset,
                    (int) (drawCmd.VtxOffset + vertexOffset));
            }
            
            vertexOffset += (uint) cmdList.VtxBuffer.Size;
            indexOffset += (uint) cmdList.IdxBuffer.Size;
        }
        
        ImGui.NewFrame();
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
        SDL.ReleaseGPUGraphicsPipeline(_device, _pipeline);
        SDL.ReleaseGPUTransferBuffer(_device, _transferBuffer);
        SDL.ReleaseGPUBuffer(_device, _indexBuffer);
        
        ImGui.DestroyContext(_imguiContext);
    }
}