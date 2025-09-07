using Crimson.Math;

namespace Crimson.Graphics;

/// <summary>
/// Describes the underlying windowing information for a surface.
/// </summary>
public readonly struct SurfaceInfo
{
    /// <summary>
    /// The underlying surface info provided to Graphite.
    /// </summary>
    public readonly Graphite.SurfaceInfo Info;

    /// <summary>
    /// The surface's size.
    /// </summary>
    public readonly Size<int> Size;

    public SurfaceInfo(Graphite.SurfaceInfo info, Size<int> size)
    {
        Info = info;
        Size = size;
    }
}