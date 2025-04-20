using System.Diagnostics;
using System.Numerics;
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

    private Size<int> _size;
    private readonly ImGuiContextPtr _imguiContext;

    private uint _vBufferSize;
    private uint _iBufferSize;

    private ID3D11Buffer _vertexBuffer;
    private ID3D11Buffer _indexBuffer;
    private ID3D11Buffer _cameraBuffer;

    private readonly ID3D11VertexShader _vertexShader;
    private readonly ID3D11PixelShader _pixelShader;
    private readonly ID3D11InputLayout _inputLayout;

    private ID3D11Texture2D? _texture;
    private ID3D11ShaderResourceView? _resourceView;
    
    public unsafe ImGuiRenderer(ID3D11Device device, Size<int> size)
    {
        _device = device;
        _size = size;

        _imguiContext = ImGui.CreateContext();
        ImGui.SetCurrentContext(_imguiContext);

        _vBufferSize = 5000;
        _iBufferSize = 10000;

        _vertexBuffer = _device.CreateBuffer(_vBufferSize, BindFlags.VertexBuffer, ResourceUsage.Dynamic,
            CpuAccessFlags.Write);
        _indexBuffer = _device.CreateBuffer(_iBufferSize, BindFlags.IndexBuffer, ResourceUsage.Dynamic,
            CpuAccessFlags.Write);
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

        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(size.Width, size.Height);
        io.BackendFlags = ImGuiBackendFlags.RendererHasVtxOffset;
        
        io.Fonts.AddFontDefault();
        RecreateFontTexture();
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