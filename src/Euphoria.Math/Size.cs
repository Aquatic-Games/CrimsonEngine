using System.Numerics;

namespace Euphoria.Math;

public record struct Size<T> where T : INumber<T>
{
    public T Width;

    public T Height;

    public Size(T wh)
    {
        Width = wh;
        Height = wh;
    }

    public Size(T width, T height)
    {
        Width = width;
        Height = height;
    }

    public Size<TOther> As<TOther>() where TOther : INumber<TOther>
        => new Size<TOther>(TOther.CreateChecked(Width), TOther.CreateChecked(Height));
}