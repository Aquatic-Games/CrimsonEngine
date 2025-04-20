using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Crimson.Core;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Hexa.NET.ImGui;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Crimson.Graphics.Renderers;

internal sealed class ImGuiRenderer : IDisposable
{
    private readonly ID3D11Device _device;
    
    private readonly ImGuiContextPtr _imguiContext;

    private uint _vBufferSize;
    private uint _iBufferSize;

    private ID3D11Buffer _vertexBuffer;
    private ID3D11Buffer _indexBuffer;
    private readonly ID3D11Buffer _cameraBuffer;

    private readonly ID3D11VertexShader _vertexShader;
    private readonly ID3D11PixelShader _pixelShader;
    private readonly ID3D11InputLayout _inputLayout;

    private readonly ID3D11DepthStencilState _depthState;
    private readonly ID3D11RasterizerState _rasterizerState;
    private readonly ID3D11BlendState _blendState;
    private readonly ID3D11SamplerState _samplerState;

    private ID3D11Texture2D? _texture;
    private ID3D11ShaderResourceView? _resourceView;

    public ImGuiContextPtr Context => _imguiContext;
    
    public unsafe ImGuiRenderer(ID3D11Device device, Size<int> size)
    {
        _device = device;

        _imguiContext = ImGui.CreateContext();
        ImGui.SetCurrentContext(_imguiContext);

        _vBufferSize = 5000;
        _iBufferSize = 10000;

        _vertexBuffer = _device.CreateBuffer((uint) (_vBufferSize * sizeof(ImDrawVert)), BindFlags.VertexBuffer,
            ResourceUsage.Dynamic, CpuAccessFlags.Write);
        _indexBuffer = _device.CreateBuffer((uint) (_iBufferSize * sizeof(ushort)), BindFlags.IndexBuffer,
            ResourceUsage.Dynamic, CpuAccessFlags.Write);
        _cameraBuffer = _device.CreateBuffer((uint) sizeof(Matrix4x4), BindFlags.ConstantBuffer, ResourceUsage.Dynamic,
            CpuAccessFlags.Write);

        ShaderUtils.LoadGraphicsShader(_device, "Debug/ImGui", out _vertexShader!, out _pixelShader!,
            out byte[]? vertexBytes);
        Debug.Assert(vertexBytes != null);

        InputElementDescription[] inputElements =
        [
            new InputElementDescription("POSITION", 0, Format.R32G32_Float, 0),
            new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 8, 0),
            new InputElementDescription("COLOR", 0, Format.R8G8B8A8_UNorm, 16, 0)
        ];

        _inputLayout = _device.CreateInputLayout(inputElements, vertexBytes);

        _depthState = _device.CreateDepthStencilState(DepthStencilDescription.None);
        _rasterizerState = _device.CreateRasterizerState(RasterizerDescription.CullNone with { ScissorEnable = true });
        _blendState = _device.CreateBlendState(BlendDescription.NonPremultiplied);
        _samplerState = _device.CreateSamplerState(SamplerDescription.LinearWrap);

        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(size.Width, size.Height);
        io.BackendFlags = ImGuiBackendFlags.RendererHasVtxOffset;
        io.IniFilename = null;
        io.LogFilename = null;
        
        io.Fonts.AddFontDefault();
        RecreateFontTexture();
        
        ImGui.NewFrame();
    }

    public unsafe void Render(ID3D11DeviceContext context)
    {
        ImGui.SetCurrentContext(_imguiContext);
        
        ImGui.Render();
        ImDrawDataPtr drawData = ImGui.GetDrawData();

        if (drawData.TotalVtxCount >= _vBufferSize)
        {
            Logger.Trace("Recreate vertex buffer.");
            _vertexBuffer.Dispose();
            _vBufferSize = (uint) drawData.TotalVtxCount + 5000;
            _vertexBuffer = _device.CreateBuffer((uint) (_vBufferSize * sizeof(ImDrawVert)), BindFlags.VertexBuffer,
                ResourceUsage.Dynamic, CpuAccessFlags.Write);
        }
        
        if (drawData.TotalIdxCount >= _iBufferSize)
        {
            Logger.Trace("Recreate index buffer.");
            _indexBuffer.Dispose();
            _iBufferSize = (uint) drawData.TotalIdxCount + 10000;
            _indexBuffer = _device.CreateBuffer((uint) (_iBufferSize * sizeof(ushort)), BindFlags.IndexBuffer,
                ResourceUsage.Dynamic, CpuAccessFlags.Write);
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
        _resourceView?.Dispose();
        _texture?.Dispose();

        ImGuiIOPtr io = ImGui.GetIO();
        byte* imagePixels;
        int width, height;
        io.Fonts.GetTexDataAsRGBA32(&imagePixels, &width, &height);

        Texture2DDescription textureDesc = new()
        {
            Width = (uint) width,
            Height = (uint) height,
            Format = Format.R8G8B8A8_UNorm,
            ArraySize = 1,
            MipLevels = 1,
            BindFlags = BindFlags.ShaderResource,
            CPUAccessFlags = CpuAccessFlags.None,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default
        };

        _texture = _device.CreateTexture2D(in textureDesc, new SubresourceData(imagePixels, (uint) width * 4));

        ShaderResourceViewDescription viewDesc = new()
        {
            Format = Format.R8G8B8A8_UNorm,
            ViewDimension = ShaderResourceViewDimension.Texture2D,
            Texture2D = new Texture2DShaderResourceView()
            {
                MipLevels = 1,
                MostDetailedMip = 0
            }
        };

        _resourceView = _device.CreateShaderResourceView(_texture, viewDesc);
    }

    public void Dispose()
    {
        _resourceView?.Dispose();
        _texture?.Dispose();
        
        _inputLayout.Dispose();
        _pixelShader.Dispose();
        _vertexShader.Dispose();
        
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
        
        ImGui.DestroyContext(_imguiContext);
    }
}