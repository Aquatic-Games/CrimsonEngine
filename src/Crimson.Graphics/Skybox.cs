using System.Diagnostics;
using Crimson.Graphics.Utils;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Crimson.Graphics;

public sealed class Skybox : IDisposable
{
    internal readonly ID3D11Texture2D TextureHandle;
    internal readonly ID3D11ShaderResourceView ResourceView;

    public unsafe Skybox(Renderer renderer, Bitmap[] bitmaps)
    {
        Debug.Assert(bitmaps.Length == 6);

        ID3D11Device device = renderer.Device;

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

            TextureHandle = device.CreateTexture2D(in textureDesc, subData);
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

        ResourceView = device.CreateShaderResourceView(TextureHandle, viewDesc);
    }
    
    public void Dispose()
    {
        ResourceView.Dispose();
        TextureHandle.Dispose();
    }
}