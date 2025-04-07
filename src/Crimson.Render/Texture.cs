using Crimson.Core;
using Crimson.Math;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Crimson.Render;

/// <summary>
/// A texture image that is used during rendering.
/// </summary>
public class Texture : IDisposable
{
    internal ID3D11Texture2D TextureHandle;
    internal ID3D11ShaderResourceView ResourceView;

    public readonly Size<int> Size;
    
    internal Texture(ID3D11Device device, ID3D11DeviceContext context, in Size<int> size, byte[] data, PixelFormat format)
    {
        Size = size;
        
        Format fmt = format switch
        {
            PixelFormat.RGBA8 => Format.R8G8B8A8_UNorm,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        uint rowPitch = format switch
        {
            PixelFormat.RGBA8 => (uint) size.Width * 4,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        Texture2DDescription textureDesc = new()
        {
            Width = (uint) size.Width,
            Height = (uint) size.Height,
            Format = fmt,
            ArraySize = 1,
            MipLevels = 0,
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
            SampleDescription = new SampleDescription(1, 0),
            CPUAccessFlags = CpuAccessFlags.None,
            MiscFlags = ResourceOptionFlags.GenerateMips
        };

        Logger.Trace("Creating texture.");
        TextureHandle = device.CreateTexture2D(in textureDesc);
        context.UpdateSubresource(data, TextureHandle, rowPitch: rowPitch);

        ShaderResourceViewDescription viewDesc = new()
        {
            ViewDimension = ShaderResourceViewDimension.Texture2D,
            Format = fmt,
            Texture2D = new Texture2DShaderResourceView()
            {
                MipLevels = uint.MaxValue,
                MostDetailedMip = 0
            }
        };
        
        Logger.Trace("Creating resource view.");
        ResourceView = device.CreateShaderResourceView(TextureHandle, viewDesc);
        
        Logger.Trace("Generating mipmaps.");
        context.GenerateMips(ResourceView);
    }

    /// <summary>
    /// Dispose of this <see cref="Texture"/>.
    /// </summary>
    public void Dispose()
    {
        ResourceView.Dispose();
        TextureHandle.Dispose();
    }
}