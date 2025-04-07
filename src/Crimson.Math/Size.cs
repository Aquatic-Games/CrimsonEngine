using System.Numerics;

namespace Crimson.Math;

/// <summary>
/// A 2-dimensional size with a width and a height.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly record struct Size<T> where T : INumber<T>
{
    /// <summary>
    /// The width.
    /// </summary>
    public readonly T Width;

    /// <summary>
    /// The height.
    /// </summary>
    public readonly T Height;

    /// <summary>
    /// Create a size with the given width and height.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public Size(T width, T height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Create a size with the given scalar.
    /// </summary>
    /// <param name="wh">The scalar width and height.</param>
    public Size(T wh) : this(wh, wh) { }
}