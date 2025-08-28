using Crimson.Math;
using Graphite.Core;

namespace Crimson.Graphics.Utils;

internal static class GraphiteExtensions
{
    public static Size2D ToGraphite(this Size<int> size)
        => new Size2D((uint) size.Width, (uint) size.Height);

    public static Size<int> ToCrimson(this Size2D size)
        => new Size<int>((int) size.Width, (int) size.Height);
}