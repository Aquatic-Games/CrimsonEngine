using Euphoria.Math;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Euphoria.Render.Utils;

internal class D3D11Target : IDisposable
{
    public readonly ID3D11Texture2D Texture;
    
    public readonly ID3D11ShaderResourceView ResourceView;

    public readonly ID3D11RenderTargetView RenderTarget;

    public D3D11Target(ID3D11Device device, Format format, Size<int> size)
    {
        Texture2DDescription desc = new()
        {
            Width = (uint) size.Width,
            Height = (uint) size.Height,
            Format = format,
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            ArraySize = 1,
            MipLevels = 1,
            SampleDescription = new SampleDescription(1, 0),
            CPUAccessFlags = CpuAccessFlags.None,
            Usage = ResourceUsage.Default,
            MiscFlags = ResourceOptionFlags.None
        };

        Texture = device.CreateTexture2D(in desc);

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
    
    public void Dispose()
    {
        RenderTarget.Dispose();
        ResourceView.Dispose();
        Texture.Dispose();
    }
}