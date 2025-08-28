using Crimson.Math;
using Graphite;
using Graphite.Core;

namespace Crimson.Graphics.Utils;

internal static class GraphiteUtils
{
    public static Format ToGraphite(this PixelFormat format)
    {
        return format switch
        {
            PixelFormat.RGBA8 => Format.R8G8B8A8_UNorm,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
    
    public static Size2D ToGraphite(this Size<int> size)
        => new Size2D((uint) size.Width, (uint) size.Height);

    public static Size<int> ToCrimson(this Size2D size)
        => new Size<int>((int) size.Width, (int) size.Height);

    public static Region3D ToGraphite(this Rectangle<int> rectangle)
        => new Region3D(rectangle.X, rectangle.Y, 0, (uint) rectangle.Width, (uint) rectangle.Height, 1);
}