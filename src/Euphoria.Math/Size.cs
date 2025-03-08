using System.Numerics;

namespace Euphoria.Math;

public readonly record struct Size<T> where T : INumber<T>
{
    public readonly T X;

    public readonly T Y;

    public Size(T x, T y)
    {
        X = x;
        Y = y;
    }

    public Size(T wh) : this(wh, wh) { }
}