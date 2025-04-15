global using GrabsTexture = grabs.Graphics.Texture;
using Crimson.Core;
using Crimson.Math;
using grabs.Core;
using grabs.Graphics;

namespace Crimson.Render;

/// <summary>
/// A texture image that is used during rendering.
/// </summary>
public class Texture : IDisposable
{
    internal GrabsTexture TextureHandle;

    /// <summary>
    /// The texture's size in pixels.
    /// </summary>
    public readonly Size<int> Size;
    
    /// <summary>
    /// Create a <see cref="Texture"/>.
    /// </summary>
    /// <param name="graphics">A <see cref="Graphics"/> instance.</param>
    /// <param name="size">The size, in pixels.</param>
    /// <param name="data">The data.</param>
    /// <param name="format">The <see cref="PixelFormat"/> of the texture.</param>
    public Texture(Graphics graphics, in Size<int> size, byte[] data, PixelFormat format)
    {
        Size = size;

        Device device = graphics.Device;
        
        Format fmt = format switch
        {
            PixelFormat.RGBA8 => Format.R8G8B8A8_UNorm,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        TextureInfo info =
            TextureInfo.Texture2D(new Size2D((uint) size.Width, (uint) size.Height), fmt, TextureUsage.Sampled);

        Logger.Trace("Creating texture.");
        // TODO: CreateTexture overload that takes T[]
        TextureHandle = device.CreateTexture<byte>(in info, data);
        
        // TODO: Mipmaps
        //Logger.Trace("Generating mipmaps.");
    }

    /// <summary>
    /// Create a <see cref="Texture"/> from the given bitmap.
    /// </summary>
    /// <param name="graphics">A <see cref="Graphics"/> instance.</param>
    /// <param name="bitmap">The <see cref="Bitmap"/> to use.</param>
    public Texture(Graphics graphics, Bitmap bitmap) : this(graphics, bitmap.Size, bitmap.Data, bitmap.Format) { }

    /// <summary>
    /// Create a <see cref="Texture"/> from the given path. 
    /// </summary>
    /// <param name="graphics">A <see cref="Graphics"/> instance.</param>
    /// <param name="path">The path to load from.</param>
    public Texture(Graphics graphics, string path) : this(graphics, new Bitmap(path)) { }

    /// <summary>
    /// Dispose of this <see cref="Texture"/>.
    /// </summary>
    public void Dispose()
    {
        TextureHandle.Dispose();
    }
}