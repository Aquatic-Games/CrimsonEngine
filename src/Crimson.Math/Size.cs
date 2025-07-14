using System.Numerics;

namespace Crimson.Math;

/// <summary>
/// A 2-dimensional size with a width and a height.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Size<T> :
    IEquatable<Size<T>>,
    IAdditionOperators<Size<T>, Size<T>, Size<T>>,
    ISubtractionOperators<Size<T>, Size<T>, Size<T>>,
    IUnaryNegationOperators<Size<T>, Size<T>>,
    IMultiplyOperators<Size<T>, Size<T>, Size<T>>,
    IMultiplyOperators<Size<T>, T, Size<T>>,
    IDivisionOperators<Size<T>, Size<T>, Size<T>>,
    IDivisionOperators<Size<T>, T, Size<T>>,
    IFormattable 
    where T : INumber<T>
{
    /// <summary>
    /// A size with a width and height of 0.
    /// </summary>
    public static Size<T> Zero => new Size<T>(T.Zero);

    /// <summary>
    /// A size with a width and height of 1.
    /// </summary>
    public static Size<T> One => new Size<T>(T.One);
    
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
    public Size(T wh)
    {
        Width = wh;
        Height = wh;
    }
    
    /// <summary>
    /// Cast this size to a <see cref="Size{T}"/> with a different component type.
    /// </summary>
    /// <typeparam name="TOther">A numeric type.</typeparam>
    /// <returns>The cast size.</returns>
    public Size<TOther> As<TOther>() where TOther : INumber<TOther>
        => new Size<TOther>(TOther.CreateChecked(Width), TOther.CreateChecked(Height));

    public static Size<T> operator +(Size<T> left, Size<T> right)
        => new Size<T>(left.Width + right.Width, left.Height + right.Height);

    public static Size<T> operator -(Size<T> left, Size<T> right)
        => new Size<T>(left.Width - right.Width, left.Height - right.Height);

    public static Size<T> operator -(Size<T> value)
        => new Size<T>(-value.Width, -value.Height);

    public static Size<T> operator *(Size<T> left, Size<T> right)
        => new Size<T>(left.Width * right.Width, left.Height * right.Height);

    public static Size<T> operator *(Size<T> left, T right)
        => new Size<T>(left.Width * right, left.Height * right);

    public static Size<T> operator /(Size<T> left, Size<T> right)
        => new Size<T>(left.Width / right.Width, left.Height / right.Height);

    public static Size<T> operator /(Size<T> left, T right)
        => new Size<T>(left.Width / right, left.Height / right);

    public bool Equals(Size<T> other)
        => this == other;

    public static explicit operator Vector2T<T>(Size<T> size)
        => new Vector2T<T>(size.Width, size.Height);

    public override bool Equals(object? obj)
    {
        return obj is Size<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public static bool operator ==(Size<T> left, Size<T> right)
    {
        return left.Width == right.Width && left.Height == right.Height;
    }

    public static bool operator !=(Size<T> left, Size<T> right)
    {
        return left.Width != right.Width || left.Height != right.Height;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        FormattableString formattable = $"{Width}x{Height}";
        return formattable.ToString(formatProvider);
    }

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }
}