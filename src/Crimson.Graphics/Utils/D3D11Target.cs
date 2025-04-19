using Crimson.Math;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Crimson.Graphics.Utils;

internal class D3D11Target : IDisposable
{
    public readonly ID3D11Texture2D Texture;
    
    public readonly ID3D11ShaderResourceView? ResourceView;

    public readonly ID3D11RenderTargetView? RenderTarget;

    public readonly ID3D11DepthStencilView? DepthTarget;

    public D3D11Target(ID3D11Device device, Format format, Size<int> size, bool shaderResource = true)
    {
        BindFlags bindFlags = shaderResource ? BindFlags.ShaderResource : BindFlags.None;

        bool isDepth = false;

        if (format is Format.D32_Float or Format.D24_UNorm_S8_UInt)
        {
            bindFlags |= BindFlags.DepthStencil;
            isDepth = true;
        }
        else
            bindFlags |= BindFlags.RenderTarget;
        
        Texture2DDescription desc = new()
        {
            Width = (uint) size.Width,
            Height = (uint) size.Height,
            Format = format,
            BindFlags = bindFlags,
            ArraySize = 1,
            MipLevels = 1,
            SampleDescription = new SampleDescription(1, 0),
            CPUAccessFlags = CpuAccessFlags.None,
            Usage = ResourceUsage.Default,
            MiscFlags = ResourceOptionFlags.None
        };

        Texture = device.CreateTexture2D(in desc);

        if (shaderResource)
        {
            ShaderResourceViewDescription viewDesc = new()
            {
                ViewDimension = ShaderResourceViewDimension.Texture2D,
                Format = format,
                Texture2D = new Texture2DShaderResourceView()
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            };

            ResourceView = device.CreateShaderResourceView(Texture, viewDesc);
        }

        if (isDepth)
        {
            DepthStencilViewDescription depthDesc = new()
            {
                ViewDimension = DepthStencilViewDimension.Texture2D,
                Format = format,
                Texture2D = new Texture2DDepthStencilView()
                {
                    MipSlice = 0
                }
            };

            DepthTarget = device.CreateDepthStencilView(Texture, depthDesc);
        }
        else
        {
            RenderTargetViewDescription rtDesc = new()
            {
                ViewDimension = RenderTargetViewDimension.Texture2D,
                Format = format,
                Texture2D = new Texture2DRenderTargetView()
                {
                    MipSlice = 0
                }
            };

            RenderTarget = device.CreateRenderTargetView(Texture, rtDesc);
        }
    }
    
    public void Dispose()
    {
        RenderTarget?.Dispose();
        DepthTarget?.Dispose();
        ResourceView?.Dispose();
        Texture.Dispose();
    }
}