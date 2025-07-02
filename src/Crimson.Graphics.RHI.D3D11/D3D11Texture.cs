using Crimson.Math;
using Vortice.Direct3D11;

namespace Crimson.Graphics.RHI.D3D11;

internal sealed class D3D11Texture : Texture
{
    public readonly ID3D11Texture2D Texture;
    
    public readonly ID3D11ShaderResourceView? ResourceView;
    public readonly ID3D11RenderTargetView? RenderTarget;
    public readonly ID3D11DepthStencilView? DepthTarget;
    
    public D3D11Texture(ID3D11Device device, ID3D11Texture2D texture, Size<uint> size) : base(size)
    {
        Texture = texture;
        RenderTarget = device.CreateRenderTargetView(Texture);
    }

    public override void Dispose()
    {
        DepthTarget?.Dispose();
        RenderTarget?.Dispose();
        ResourceView?.Dispose();
    }
}