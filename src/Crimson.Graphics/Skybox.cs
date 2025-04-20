using System.Diagnostics;
using Crimson.Core;
using Crimson.Graphics.Primitives;
using Crimson.Graphics.Utils;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Crimson.Graphics;

public sealed class Skybox : IDisposable
{
    private readonly ID3D11DeviceContext _context;
    
    private readonly ID3D11Texture2D _textureHandle;
    private readonly ID3D11ShaderResourceView _resourceView;
    
    private readonly ID3D11VertexShader _skyboxVtx;
    private readonly ID3D11PixelShader _skyboxPxl;
    private readonly ID3D11InputLayout _skyboxLayout;

    private readonly ID3D11Buffer _vertexBuffer;
    private readonly ID3D11Buffer _indexBuffer;

    private readonly ID3D11DepthStencilState _depthState;
    private readonly ID3D11RasterizerState _rasterizerState;

    public unsafe Skybox(Renderer renderer, Bitmap[] bitmaps)
    {
        Debug.Assert(bitmaps.Length == 6);

        ID3D11Device device = renderer.Device;
        _context = renderer.Context;

        Format fmt = bitmaps[0].Format.ToD3D((uint) bitmaps[0].Size.Width, out uint rowPitch);

        Texture2DDescription textureDesc = new()
        {
            Width = (uint) bitmaps[0].Size.Width,
            Height = (uint) bitmaps[0].Size.Height,
            Format = fmt,
            ArraySize = 6,
            MipLevels = 1,
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.ShaderResource,
            SampleDescription = new SampleDescription(1, 0),
            CPUAccessFlags = CpuAccessFlags.None,
            MiscFlags = ResourceOptionFlags.TextureCube
        };
        
        // lol
        fixed (void* pBitmap0 = bitmaps[0].Data)
        fixed (void* pBitmap1 = bitmaps[1].Data)
        fixed (void* pBitmap2 = bitmaps[2].Data)
        fixed (void* pBitmap3 = bitmaps[3].Data)
        fixed (void* pBitmap4 = bitmaps[4].Data)
        fixed (void* pBitmap5 = bitmaps[5].Data)
        {
            Span<SubresourceData> subData =
            [
                new SubresourceData(pBitmap0, rowPitch),
                new SubresourceData(pBitmap1, rowPitch),
                new SubresourceData(pBitmap2, rowPitch),
                new SubresourceData(pBitmap3, rowPitch),
                new SubresourceData(pBitmap4, rowPitch),
                new SubresourceData(pBitmap5, rowPitch),
            ];

            _textureHandle = device.CreateTexture2D(in textureDesc, subData);
        }

        ShaderResourceViewDescription viewDesc = new()
        {
            Format = fmt,
            ViewDimension = ShaderResourceViewDimension.TextureCube,
            TextureCube = new TextureCubeShaderResourceView()
            {
                MipLevels = 1,
                MostDetailedMip = 0
            }
        };

        _resourceView = device.CreateShaderResourceView(_textureHandle, viewDesc);
        
        Logger.Trace("Creating skybox resources.");
        ShaderUtils.LoadGraphicsShader(device, "Environment/Skybox", out _skyboxVtx!, out _skyboxPxl!,
            out byte[] skyboxCode);
        Debug.Assert(skyboxCode != null);

        InputElementDescription[] skyboxLayout =
            [new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0)];

        _skyboxLayout = device.CreateInputLayout(skyboxLayout, skyboxCode);

        Cube cube = new Cube();
        _vertexBuffer = device.CreateBuffer(cube.Vertices, BindFlags.VertexBuffer);
        _indexBuffer = device.CreateBuffer(cube.Indices, BindFlags.IndexBuffer);

        _depthState =
            device.CreateDepthStencilState(new DepthStencilDescription(true, DepthWriteMask.Zero,
                ComparisonFunction.LessEqual));
        _rasterizerState = device.CreateRasterizerState(RasterizerDescription.CullNone);
    }

    internal void Render()
    {
        _context.IASetInputLayout(_skyboxLayout);
        _context.VSSetShader(_skyboxVtx);
        _context.PSSetShader(_skyboxPxl);
        
        _context.OMSetDepthStencilState(_depthState);
        _context.RSSetState(_rasterizerState);
        
        _context.IASetVertexBuffer(0, _vertexBuffer, Vertex.SizeInBytes);
        _context.IASetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
        
        _context.PSSetShaderResource(0, _resourceView);
        
        _context.DrawIndexed(36, 0, 0);
    }
    
    public void Dispose()
    {
        _skyboxLayout.Dispose();
        _skyboxPxl.Dispose();
        _skyboxVtx.Dispose();
        
        _resourceView.Dispose();
        _textureHandle.Dispose();
    }
}