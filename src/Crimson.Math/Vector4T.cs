using System.Numerics;
using System.Runtime.InteropServices;

namespace Crimson.Math;

/// <summary>
/// Represents a 4-dimensional vector comprised of an X and Y component.
/// </summary>
/// <typeparam name="T">A numeric type.</typeparam>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Vector4T<T> : 
    IEquatable<Vector4T<T>>,
    IAdditionOperators<Vector4T<T>, Vector4T<T>, Vector4T<T>>,
    ISubtractionOperators<Vector4T<T>, Vector4T<T>, Vector4T<T>>,
    IUnaryNegationOperators<Vector4T<T>, Vector4T<T>>,
    IMultiplyOperators<Vector4T<T>, Vector4T<T>, Vector4T<T>>,
    IMultiplyOperators<Vector4T<T>, T, Vector4T<T>>,
    IDivisionOperators<Vector4T<T>, Vector4T<T>, Vector4T<T>>,
    IDivisionOperators<Vector4T<T>, T, Vector4T<T>>,
    IFormattable 
    where T : INumber<T>
{
    /// <summary>
    /// Gets a <see cref="Vector4T{T}"/> where the components are zero.
    /// </summary>
    public static Vector4T<T> Zero => new Vector4T<T>(T.Zero);

    /// <summary>
    /// Gets a <see cref="Vector4T{T}"/> where the components are one.
    /// </summary>
    public static Vector4T<T> One => new Vector4T<T>(T.One);

    /// <summary>
    /// Gets a <see cref="Vector4T{T}"/> where the X component is one.
    /// </summary>
    public static Vector4T<T> UnitX => new Vector4T<T>(T.One, T.Zero, T.Zero, T.Zero);

    /// <summary>
    /// Gets a <see cref="Vector4T{T}"/> where the Y component is one.
    /// </summary>
    public static Vector4T<T> UnitY => new Vector4T<T>(T.Zero, T.One, T.Zero, T.Zero);

    /// <summary>
    /// Gets a <see cref="Vector4T{T}"/> where the Z component is one.
    /// </summary>
    public static Vector4T<T> UnitZ => new Vector4T<T>(T.Zero, T.Zero, T.One, T.Zero);

    /// <summary>
    /// Gets a <see cref="Vector4T{T}"/> where the W component is one.
    /// </summary>
    public static Vector4T<T> UnitW => new Vector4T<T>(T.Zero, T.Zero, T.Zero, T.One);
    
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public readonly T X;

    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public readonly T Y;

    /// <summary>
    /// The Z component of the vector.
    /// </summary>
    public readonly T Z;

    /// <summary>
    /// The W component of the vector.
    /// </summary>
    public readonly T W;

    /// <summary>
    /// Create a <see cref="Vector4T{T}"/> with separate X, Y, Z, and W components.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    /// <param name="z">The Z component.</param>
    /// <param name="w">The W component.</param>
    public Vector4T(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /// <summary>
    /// Create a <see cref="Vector4T{T}"/> from a scalar, where the all components have the same value.
    /// </summary>
    /// <param name="scalar"></param>
    public Vector4T(T scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
        W = scalar;
    }

    /// <summary>
    /// Get the value at the given index.
    /// </summary>
    /// <param name="index">The index to get the value.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid index is given.</exception>
    public T this[int index]
    {
        get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                3 => W,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }
    }

    /// <summary>
    /// Cast this vector to a <see cref="Vector4T{T}"/> with a different component type.
    /// </summary>
    /// <typeparam name="TOther">A numeric type.</typeparam>
    /// <returns>The cast vector.</returns>
    public Vector4T<TOther> As<TOther>() where TOther : INumber<TOther>
        => new Vector4T<TOther>(TOther.CreateChecked(X), TOther.CreateChecked(Y), TOther.CreateChecked(Z), TOther.CreateChecked(W));

    public static Vector4T<T> operator +(Vector4T<T> left, Vector4T<T> right)
        => new Vector4T<T>(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);

    public static Vector4T<T> operator -(Vector4T<T> left, Vector4T<T> right)
        => new Vector4T<T>(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);

    public static Vector4T<T> operator -(Vector4T<T> value)
        => new Vector4T<T>(-value.X, -value.Y, -value.Z, -value.W);

    public static Vector4T<T> operator *(Vector4T<T> left, Vector4T<T> right)
        => new Vector4T<T>(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);

    public static Vector4T<T> operator *(Vector4T<T> left, T right)
        => new Vector4T<T>(left.X * right, left.Y * right, left.Z * right, left.W * right);

    public static Vector4T<T> operator /(Vector4T<T> left, Vector4T<T> right)
        => new Vector4T<T>(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);

    public static Vector4T<T> operator /(Vector4T<T> left, T right)
        => new Vector4T<T>(left.X / right, left.Y / right, left.Z / right, left.W / right);

    public bool Equals(Vector4T<T> other)
        => this == other;

    public override bool Equals(object? obj)
    {
        return obj is Vector4T<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }

    public static bool operator ==(Vector4T<T> left, Vector4T<T> right)
    {
        return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
    }

    public static bool operator !=(Vector4T<T> left, Vector4T<T> right)
    {
        return left.X != right.X || left.Y != right.Y || left.Z != right.Z || left.W != right.W;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        FormattableString formattable = $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}";
        return formattable.ToString(formatProvider);
    }

    public override string ToString()
    {
        return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}";
    }
}