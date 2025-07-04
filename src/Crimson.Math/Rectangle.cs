using System.Numerics;

namespace Crimson.Math;

public readonly struct Rectangle<T> where T : INumber<T>
{
    public readonly Vector2T<T> Position;

    public readonly Size<T> Size;

    public T X => Position.X;

    public T Y => Position.Y;

    public T Width => Size.Width;

    public T Height => Size.Height;

    public Rectangle(Vector2T<T> position, Size<T> size)
    {
        Position = position;
        Size = size;
    }

    public Rectangle(T x, T y, T width, T height) : this(new Vector2T<T>(x, y), new Size<T>(width, height)) { }

    public bool Contains(Vector2T<T> point)
    {
        return point.X >= X && point.X < X + Width &&
               point.Y >= Y && point.Y < Y + Height;
    }
}