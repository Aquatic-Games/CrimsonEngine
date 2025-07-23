using System.Numerics;
using System.Runtime.InteropServices;

namespace Crimson.Math;

/// <summary>
/// Represents a 3-dimensional vector comprised of an X and Y component.
/// </summary>
/// <typeparam name="T">A numeric type.</typeparam>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Vector3T<T> : 
    IEquatable<Vector3T<T>>,
    IAdditionOperators<Vector3T<T>, Vector3T<T>, Vector3T<T>>,
    ISubtractionOperators<Vector3T<T>, Vector3T<T>, Vector3T<T>>,
    IUnaryNegationOperators<Vector3T<T>, Vector3T<T>>,
    IMultiplyOperators<Vector3T<T>, Vector3T<T>, Vector3T<T>>,
    IMultiplyOperators<Vector3T<T>, T, Vector3T<T>>,
    IDivisionOperators<Vector3T<T>, Vector3T<T>, Vector3T<T>>,
    IDivisionOperators<Vector3T<T>, T, Vector3T<T>>,
    IFormattable 
    where T : INumber<T>
{
    /// <summary>
    /// Gets a <see cref="Vector3T{T}"/> where the components are zero.
    /// </summary>
    public static Vector3T<T> Zero => new Vector3T<T>(T.Zero);

    /// <summary>
    /// Gets a <see cref="Vector3T{T}"/> where the components are one.
    /// </summary>
    public static Vector3T<T> One => new Vector3T<T>(T.One);

    /// <summary>
    /// Gets a <see cref="Vector3T{T}"/> where the X component is one.
    /// </summary>
    public static Vector3T<T> UnitX => new Vector3T<T>(T.One, T.Zero, T.Zero);

    /// <summary>
    /// Gets a <see cref="Vector3T{T}"/> where the Y component is one.
    /// </summary>
    public static Vector3T<T> UnitY => new Vector3T<T>(T.Zero, T.One, T.Zero);

    /// <summary>
    /// Gets a <see cref="Vector3T{T}"/> where the Z component is one.
    /// </summary>
    public static Vector3T<T> UnitZ => new Vector3T<T>(T.Zero, T.Zero, T.One);
    
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
    /// Create a <see cref="Vector3T{T}"/> with separate X, Y, and Z components.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    /// <param name="z">The Z component.</param>
    public Vector3T(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Create a <see cref="Vector3T{T}"/> from a scalar, where the all components have the same value.
    /// </summary>
    /// <param name="scalar"></param>
    public Vector3T(T scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
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
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }
    }

    /// <summary>
    /// Cast this vector to a <see cref="Vector3T{T}"/> with a different component type.
    /// </summary>
    /// <typeparam name="TOther">A numeric type.</typeparam>
    /// <returns>The cast vector.</returns>
    public Vector3T<TOther> As<TOther>() where TOther : INumber<TOther>
        => new Vector3T<TOther>(TOther.CreateChecked(X), TOther.CreateChecked(Y), TOther.CreateChecked(Z));

    public static Vector3T<T> operator +(Vector3T<T> left, Vector3T<T> right)
        => new Vector3T<T>(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Vector3T<T> operator -(Vector3T<T> left, Vector3T<T> right)
        => new Vector3T<T>(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Vector3T<T> operator -(Vector3T<T> value)
        => new Vector3T<T>(-value.X, -value.Y, -value.Z);

    public static Vector3T<T> operator *(Vector3T<T> left, Vector3T<T> right)
        => new Vector3T<T>(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    public static Vector3T<T> operator *(Vector3T<T> left, T right)
        => new Vector3T<T>(left.X * right, left.Y * right, left.Z * right);

    public static Vector3T<T> operator /(Vector3T<T> left, Vector3T<T> right)
        => new Vector3T<T>(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    public static Vector3T<T> operator /(Vector3T<T> left, T right)
        => new Vector3T<T>(left.X / right, left.Y / right, left.Z / right);
    
    public static explicit operator System.Numerics.Vector3(Vector3T<T> vector)
        => new System.Numerics.Vector3(float.CreateChecked(vector.X), float.CreateChecked(vector.Y), float.CreateChecked(vector.Z));
    
    public static explicit operator Vector3T<T>(System.Numerics.Vector3 vector)
        => new Vector3T<T>(T.CreateChecked(vector.X), T.CreateChecked(vector.Y), T.CreateChecked(vector.Z));

    public bool Equals(Vector3T<T> other)
        => this == other;

    public override bool Equals(object? obj)
    {
        return obj is Vector3T<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public static bool operator ==(Vector3T<T> left, Vector3T<T> right)
    {
        return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
    }

    public static bool operator !=(Vector3T<T> left, Vector3T<T> right)
    {
        return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        FormattableString formattable = $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
        return formattable.ToString(formatProvider);
    }

    public override string ToString()
    {
        return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
    }
}