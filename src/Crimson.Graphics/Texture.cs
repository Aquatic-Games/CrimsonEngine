using System.Runtime.CompilerServices;
using Crimson.Core;
using Crimson.Graphics.Utils;
using Crimson.Math;
using SDL3;

namespace Crimson.Graphics;

/// <summary>
/// A texture image that is used during rendering.
/// </summary>
public class Texture : IDisposable
{
    private readonly IntPtr _device;
    
    internal readonly IntPtr TextureHandle;

    /// <summary>
    /// The texture's size in pixels.
    /// </summary>
    public readonly Size<int> Size;
    
    /// <summary>
    /// Create a <see cref="Texture"/>.
    /// </summary>
    /// <param name="size">The size, in pixels.</param>
    /// <param name="data">The data.</param>
    /// <param name="format">The <see cref="PixelFormat"/> of the texture.</param>
    public unsafe Texture(in Size<int> size, byte[] data, PixelFormat format)
    {
        Size = size;

        _device = Renderer.Device;

        SDL.GPUTextureCreateInfo textureInfo = new()
        {
            Type = SDL.GPUTextureType.Texturetype2D,
            Format = format.ToSdl(out uint rowPitch),
            Width = (uint) size.Width,
            Height = (uint) size.Height,
            LayerCountOrDepth = 1,
            NumLevels = SdlUtils.CalculateMipLevels((uint) size.Width, (uint) size.Height),
            Usage = SDL.GPUTextureUsageFlags.Sampler | SDL.GPUTextureUsageFlags.ColorTarget
        };

        Logger.Trace("Creating texture.");
        TextureHandle = SDL.CreateGPUTexture(_device, in textureInfo).Check("Create texture");

        SDL.GPUTransferBufferCreateInfo transInfo = new()
        {
            Usage = SDL.GPUTransferBufferUsage.Upload,
            Size = (uint) size.Width * (uint) size.Height * rowPitch
        };

        Logger.Trace("Creating transfer buffer");
        IntPtr transBuffer = SDL.CreateGPUTransferBuffer(_device, in transInfo);

        nint mappedData = SDL.MapGPUTransferBuffer(_device, transBuffer, false);
        fixed (byte* pData = data)
            Unsafe.CopyBlock((void*) mappedData, pData, transInfo.Size);
        SDL.UnmapGPUTransferBuffer(_device, transBuffer);

        IntPtr cb = SDL.AcquireGPUCommandBuffer(_device).Check("Acquire command buffer");
        IntPtr pass = SDL.BeginGPUCopyPass(cb).Check("Begin copy pass");

        SDL.GPUTextureTransferInfo source = new()
        {
            TransferBuffer = transBuffer,
            Offset = 0,
            PixelsPerRow = (uint) size.Width
        };

        SDL.GPUTextureRegion dest = new()
        {
            Texture = TextureHandle,
            X = 0,
            Y = 0,
            W = (uint) size.Width,
            H = (uint) size.Height,
            D = 1,
            MipLevel = 0
        };
        
        SDL.UploadToGPUTexture(pass, in source, in dest, false);
        
        SDL.EndGPUCopyPass(pass);

        if (textureInfo.NumLevels > 1)
        {
            Logger.Trace($"Generating mipmaps. (Level {textureInfo.NumLevels})");
            SDL.GenerateMipmapsForGPUTexture(cb, TextureHandle);
        }

        SDL.SubmitGPUCommandBuffer(cb).Check("Submit command buffer");
        SDL.ReleaseGPUTransferBuffer(_device, transBuffer);
    }

    /// <summary>
    /// Create a <see cref="Texture"/> from the given bitmap.
    /// </summary>
    /// <param name="bitmap">The <see cref="Bitmap"/> to use.</param>
    public Texture(Bitmap bitmap) : this(bitmap.Size, bitmap.Data, bitmap.Format) { }

    /// <summary>
    /// Create a <see cref="Texture"/> from the given path. 
    /// </summary>
    /// <param name="path">The path to load from.</param>
    public Texture(string path) : this(new Bitmap(path)) { }

    /// <summary>
    /// Dispose of this <see cref="Texture"/>.
    /// </summary>
    public void Dispose()
    {
        SDL.ReleaseGPUTexture(_device, TextureHandle);
    }
}