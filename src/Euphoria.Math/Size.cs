using System.Numerics;

namespace Euphoria.Math;

public readonly record struct Size<T> where T : INumber<T>
{
    public readonly T Width;

    public readonly T Height;

    public Size(T width, T height)
    {
        Width = width;
        Height = height;
    }

    public Size(T wh) : this(wh, wh) { }
}