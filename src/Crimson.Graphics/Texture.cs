using Crimson.Content;
using Crimson.Core;
using Crimson.Math;
using Graphite;
using Graphite.Core;
using Buffer = Graphite.Buffer;

namespace Crimson.Graphics;

/// <summary>
/// A texture image that is used during rendering.
/// </summary>
public class Texture : IContentResource<Texture>, IDisposable
{
    private readonly uint _numMipLevels;
    private readonly bool _isOwnedByRenderer;
    
    internal readonly GrTexture TextureHandle;

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

        Device device = Renderer.Device;
        _numMipLevels = GraphiteUtils.CalculateMipLevels((uint) size.Width, (uint) size.Height);

        Format fmt = format switch
        {
            PixelFormat.RGBA8 => Format.R8G8B8A8_UNorm,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        TextureInfo info = TextureInfo.Texture2D(fmt, new Size2D((uint) size.Width, (uint) size.Height), _numMipLevels,
            TextureUsage.ShaderResource | TextureUsage.GenerateMips);

        Logger.Trace("Creating texture.");
        TextureHandle = device.CreateTexture(in info);
        
        // TODO: Texture names in Graphite
        /*if (name != null)
            SDL.SetGPUTextureName(_device, TextureHandle, name);*/
        
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

    internal Texture(GrTexture textureHandle, Size<int> size, string name)
    {
        Name = name;
        Size = size;
        TextureHandle = textureHandle;
        _isOwnedByRenderer = true;
    }

    public void Update(Rectangle<int> location, byte[] data)
    {
        // TODO: Obviously, creating/deleting transfer buffers and executing command lists etc is slow for every texture
        //       update, especially when updating often like in the case of the Font. Perhaps some way to keep the
        //       command buffer around, only submitting on use, or some way to update multiple times is needed?

        Device device = Renderer.Device;
        CommandList cl = Renderer.CommandList;
        
        Buffer buffer = device.CreateBuffer(BufferUsage.TransferBuffer, data);
        cl.Begin();
        cl.CopyBufferToTexture(buffer, 0, TextureHandle, new Size3D((uint) location.Width, (uint) location.Height),
            new Offset3D(location.X, location.Y));

        if (_numMipLevels > 1)
        {
            Logger.Trace($"Generating mipmaps. (Level {_numMipLevels})");
            cl.GenerateMipmaps(TextureHandle);
        }
        
        cl.End();
        device.ExecuteCommandList(cl);
    }

    /// <summary>
    /// Dispose of this <see cref="Texture"/>.
    /// </summary>
    public void Dispose()
    {
        if (_isOwnedByRenderer)
            return;
        
        TextureHandle.Dispose();
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