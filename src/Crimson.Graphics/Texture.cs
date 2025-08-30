using System.Runtime.CompilerServices;
using Crimson.Content;
using Crimson.Core;
using Crimson.Graphics.Utils;
using Crimson.Math;
using SDL3;

namespace Crimson.Graphics;

/// <summary>
/// A texture image that is used during rendering.
/// </summary>
public class Texture : IContentResource<Texture>, IDisposable
{
    private readonly IntPtr _device;
    private readonly uint _numMipLevels;
    private readonly bool _isOwnedByRenderer;
    
    internal readonly IntPtr TextureHandle;

    /// <summary>
    /// The texture's friendly name, if any.
    /// </summary>
    public readonly string? Name;

    /// <summary>
    /// The texture's size in pixels.
    /// </summary>
    public readonly Size<int> Size;
    
    /// <summary>
    /// Create a <see cref="Texture"/>.
    /// </summary>
    /// <param name="size">The size, in pixels. If <see langword="null"/> is provided, a blank texture will be created.</param>
    /// <param name="data">The data.</param>
    /// <param name="format">The <see cref="PixelFormat"/> of the texture.</param>
    /// <param name="name">The texture's name used during debugging, if any.</param>
    public Texture(in Size<int> size, byte[]? data, PixelFormat format, string? name = null)
    {
        Name = name;
        Size = size;

        _device = Renderer.Device;
        _numMipLevels = SdlUtils.CalculateMipLevels((uint) size.Width, (uint) size.Height);

        SDL.GPUTextureCreateInfo textureInfo = new()
        {
            Type = SDL.GPUTextureType.Texturetype2D,
            Format = format.ToSdl(out _),
            Width = (uint) size.Width,
            Height = (uint) size.Height,
            LayerCountOrDepth = 1,
            NumLevels = _numMipLevels,
            Usage = SDL.GPUTextureUsageFlags.Sampler | SDL.GPUTextureUsageFlags.ColorTarget
        };

        Logger.Trace("Creating texture.");
        TextureHandle = SDL.CreateGPUTexture(_device, in textureInfo).Check("Create texture");
        
        if (name != null)
            SDL.SetGPUTextureName(_device, TextureHandle, name);
        
        if (data != null)
            Update(new Rectangle<int>(Vector2T<int>.Zero, Size), data);
    }

    /// <summary>
    /// Create a <see cref="Texture"/> from the given bitmap.
    /// </summary>
    /// <param name="bitmap">The <see cref="Bitmap"/> to use.</param>
    /// <param name="name">The texture's name used during debugging, if any.</param>
    public Texture(Bitmap bitmap, string? name = null) : this(bitmap.Size, bitmap.Data, bitmap.Format, name) { }

    /// <summary>
    /// Create a <see cref="Texture"/> from the given path. 
    /// </summary>
    /// <param name="path">The path to load from.</param>
    /// <param name="name">The texture's name used during debugging, if any. If nothing is provided, the path will be used.</param>
    public Texture(string path) : this(new Bitmap(path), path) { }

    internal Texture(IntPtr textureHandle, Size<int> size, string name)
    {
        Name = name;
        Size = size;
        TextureHandle = textureHandle;
        _isOwnedByRenderer = true;
    }

    public unsafe void Update(Rectangle<int> location, byte[] data)
    {
        // TODO: Obviously, creating/deleting transfer buffers and executing command lists etc is slow for every texture
        //       update, especially when updating often like in the case of the Font. Perhaps some way to keep the
        //       command buffer around, only submitting on use, or some way to update multiple times is needed?

        IntPtr cb = SDL.AcquireGPUCommandBuffer(_device).Check("Acquire command buffer");

        SDL.GPUTextureRegion dest = new()
        {
            Texture = TextureHandle,
            X = (uint) location.X,
            Y = (uint) location.Y,
            W = (uint) location.Width,
            H = (uint) location.Height,
            D = 1,
            MipLevel = 0
        };
        
        Renderer.UpdateTexture(cb, in dest, data);

        SDL.SubmitGPUCommandBuffer(cb).Check("Submit command buffer");

        if (_numMipLevels > 1)
            Renderer.MipmapQueue.Add(TextureHandle);
    }

    /// <summary>
    /// Dispose of this <see cref="Texture"/>.
    /// </summary>
    public void Dispose()
    {
        if (_isOwnedByRenderer)
            return;
        
        SDL.ReleaseGPUTexture(_device, TextureHandle);
    }
    
    public static Texture White { get; internal set; }
    
    public static Texture Black { get; internal set; }
    
    public static Texture EmptyNormal { get; internal set; }
    
    public static Texture LoadResource(string fullPath, bool hasExtension)
    {
        if (hasExtension)
            return new Texture(fullPath);
        
        fullPath = Path.ChangeExtension(fullPath, ".png");
        if (File.Exists(fullPath))
            return new Texture(fullPath);
        
        throw new FileNotFoundException(fullPath);
    }
}