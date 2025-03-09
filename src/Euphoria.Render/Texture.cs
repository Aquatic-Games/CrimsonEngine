global using GrabsTexture = grabs.Graphics.Texture;
using Euphoria.Math;
using grabs.Core;
using grabs.Graphics;

namespace Euphoria.Render;

/// <summary>
/// A texture image that is used during rendering.
/// </summary>
public class Texture : IDisposable
{
    internal GrabsTexture TextureHandle;

    /// <summary>
    /// Create a <see cref="Texture"/>.
    /// </summary>
    /// <param name="size">The size, in pixels.</param>
    /// <param name="data">The data.</param>
    /// <param name="format">The <see cref="Format"/> of the texture.</param>
    public Texture(in Size<int> size, byte[] data, Format format = Format.R8G8B8A8_UNorm)
    {
        Device device = Graphics.Device;

        TextureInfo info = TextureInfo.Texture2D(new Size2D((uint) size.Width, (uint) size.Height), format,
            TextureUsage.Sampled);

        TextureHandle = device.CreateTexture<byte>(in info, data.AsSpan());
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
        TextureHandle.Dispose();
    }
}