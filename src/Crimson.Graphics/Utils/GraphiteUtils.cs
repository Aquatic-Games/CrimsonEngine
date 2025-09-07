using Crimson.Math;
using Graphite.Core;

namespace Crimson.Graphics.Utils;

internal static class GraphiteUtils
{
    public static Size2D ToGraphite(this Size<int> size)
        => new Size2D((uint) size.Width, (uint) size.Height);
}