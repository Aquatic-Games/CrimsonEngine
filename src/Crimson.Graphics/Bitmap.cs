using System.Reflection;
using Crimson.Content;
using Crimson.Core;
using Crimson.Math;
using StbImageSharp;

namespace Crimson.Graphics;

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
    /// The pixel format.
    /// </summary>
    public readonly PixelFormat Format;

    /// <summary>
    /// Load a bitmap from the given path.
    /// </summary>
    /// <param name="path">The path to load from.</param>
    public Bitmap(string path)
    {
        Logger.Trace($"Loading bitmap from path \"{path}\".");

        ImageResult result;
        
#if !DEBUG
        if (!File.Exists(path))
        {
            Logger.Error($"Could not find bitmap with path \"{path}\"!");
            
            result = ImageResult.FromMemory(
                Resources.LoadEmbeddedResource("Crimson.Graphics.DEBUG.png", Assembly.GetExecutingAssembly()),
                ColorComponents.RedGreenBlueAlpha);
        }
        else
#endif
        {
            using FileStream stream = File.OpenRead(path); 
            result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        }
        
        Size = new Size<int>(result.Width, result.Height);
        Data = result.Data;
        Format = PixelFormat.RGBA8;
    }

    public Bitmap(byte[] imageBytes)
    {
        ImageResult result = ImageResult.FromMemory(imageBytes, ColorComponents.RedGreenBlueAlpha);
        Size = new Size<int>(result.Width, result.Height);
        Data = result.Data;
        Format = PixelFormat.RGBA8;
    }
}