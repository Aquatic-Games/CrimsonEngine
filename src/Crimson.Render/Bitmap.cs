using Crimson.Core;
using Crimson.Math;
using StbImageSharp;

namespace Crimson.Render;

/// <summary>
/// A bitmap image, used to create textures.
/// </summary>
public class Bitmap
{
    /// <summary>
    /// The size, in pixels.
    /// </summary>
    public readonly Size<int> Size;

    /// <summary>
    /// The bitmap data.
    /// </summary>
    public readonly byte[] Data;

    /// <summary>
    /// The format.
    /// </summary>
    public readonly PixelFormat Format;

    /// <summary>
    /// Load a bitmap from the given path.
    /// </summary>
    /// <param name="path">The path to load from.</param>
    public Bitmap(string path)
    {
        Logger.Trace($"Loading bitmap from path \"{path}\".");
        
        using FileStream stream = File.OpenRead(path); 
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        Size = new Size<int>(result.Width, result.Height);
        Data = result.Data;
        Format = PixelFormat.RGBA8;
    }
}