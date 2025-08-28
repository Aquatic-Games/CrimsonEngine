global using GrTexture = Graphite.Texture;
using System.Runtime.CompilerServices;
using Crimson.Content;
using Crimson.Core;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Graphite;
using Graphite.Core;

namespace Crimson.Graphics;

/// <summary>
/// A texture image that is used during rendering.
/// </summary>
public class Texture : IContentResource<Texture>, IDisposable
{
    private readonly bool _isOwnedByRenderer;

    internal readonly GrTexture GrTexture;

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

        TextureInfo info = new()
        {
            Type = TextureType.Texture2D,
            Size = new Size3D(size.ToGraphite()),
            Format = format.ToGraphite(),
            ArraySize = 1,
            MipLevels = 0,
            Usage = TextureUsage.ShaderResource | TextureUsage.GenerateMips
        };

        GrTexture = device.CreateTexture(in info);
        
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

    internal Texture(GrTexture grTexture, Size<int> size, string name)
    {
        Name = name;
        Size = size;
        GrTexture = grTexture;
        _isOwnedByRenderer = true;
    }

    public void Update(Rectangle<int> location, byte[] data)
    {
        Device device = Renderer.Device;
        device.UpdateTexture(GrTexture, location.ToGraphite(), data);

        CommandList cl = Renderer.CommandList;
        cl.Begin();
        cl.GenerateMipmaps(GrTexture);
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
        
        GrTexture.Dispose();
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