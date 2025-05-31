using System.Numerics;
using System.Runtime.InteropServices;

namespace Crimson.Math;

/// <summary>
/// Represents a 2-dimensional vector comprised of an X and Y component.
/// </summary>
/// <typeparam name="T">A numeric type.</typeparam>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Vector2T<T> : 
    IEquatable<Vector2T<T>>,
    IAdditionOperators<Vector2T<T>, Vector2T<T>, Vector2T<T>>,
    ISubtractionOperators<Vector2T<T>, Vector2T<T>, Vector2T<T>>,
    IUnaryNegationOperators<Vector2T<T>, Vector2T<T>>,
    IMultiplyOperators<Vector2T<T>, Vector2T<T>, Vector2T<T>>,
    IMultiplyOperators<Vector2T<T>, T, Vector2T<T>>,
    IDivisionOperators<Vector2T<T>, Vector2T<T>, Vector2T<T>>,
    IDivisionOperators<Vector2T<T>, T, Vector2T<T>>,
    IFormattable 
    where T : INumber<T>
{
    /// <summary>
    /// Gets a <see cref="Vector2T{T}"/> where the components are zero.
    /// </summary>
    public static Vector2T<T> Zero => new Vector2T<T>(T.Zero);

    /// <summary>
    /// Gets a <see cref="Vector2T{T}"/> where the components are one.
    /// </summary>
    public static Vector2T<T> One => new Vector2T<T>(T.One);

    /// <summary>
    /// Gets a <see cref="Vector2T{T}"/> where the X component is one.
    /// </summary>
    public static Vector2T<T> UnitX => new Vector2T<T>(T.One, T.Zero);

    /// <summary>
    /// Gets a <see cref="Vector2T{T}"/> where the Y component is one.
    /// </summary>
    public static Vector2T<T> UnitY => new Vector2T<T>(T.Zero, T.One);
    
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public readonly T X;

    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public readonly T Y;

    /// <summary>
    /// Create a <see cref="Vector2T{T}"/> with a separate X and Y component.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    public Vector2T(T x, T y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Create a <see cref="Vector2T{T}"/> from a scalar, where the X and Y components have the same value.
    /// </summary>
    /// <param name="scalar"></param>
    public Vector2T(T scalar)
    {
        X = scalar;
        Y = scalar;
    }

    /// <summary>
    /// Cast this vector to a <see cref="Vector2T{T}"/> with a different component type.
    /// </summary>
    /// <typeparam name="TOther">A numeric type.</typeparam>
    /// <returns>The cast vector.</returns>
    public Vector2T<TOther> As<TOther>() where TOther : INumber<TOther>
        => new Vector2T<TOther>(TOther.CreateChecked(X), TOther.CreateChecked(Y));

    public static Vector2T<T> operator +(Vector2T<T> left, Vector2T<T> right)
        => new Vector2T<T>(left.X + right.X, left.Y + right.Y);

    public static Vector2T<T> operator -(Vector2T<T> left, Vector2T<T> right)
        => new Vector2T<T>(left.X - right.X, left.Y - right.Y);

    public static Vector2T<T> operator -(Vector2T<T> value)
        => new Vector2T<T>(-value.X, -value.Y);

    public static Vector2T<T> operator *(Vector2T<T> left, Vector2T<T> right)
        => new Vector2T<T>(left.X * right.X, left.Y * right.Y);

    public static Vector2T<T> operator *(Vector2T<T> left, T right)
        => new Vector2T<T>(left.X * right, left.Y * right);

    public static Vector2T<T> operator /(Vector2T<T> left, Vector2T<T> right)
        => new Vector2T<T>(left.X / right.X, left.Y / right.Y);

    public static Vector2T<T> operator /(Vector2T<T> left, T right)
        => new Vector2T<T>(left.X / right, left.Y / right);

    public bool Equals(Vector2T<T> other)
        => this == other;

    public override bool Equals(object? obj)
    {
        return obj is Vector2T<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Vector2T<T> left, Vector2T<T> right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Vector2T<T> left, Vector2T<T> right)
    {
        return left.X != right.X || left.Y != right.Y;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        FormattableString formattable = $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        return formattable.ToString(formatProvider);
    }

    public override string ToString()
    {
        return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
    }
}