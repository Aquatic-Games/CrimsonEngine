using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Euphoria.Math;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Vector2T<T> :
    IEquatable<Vector2T<T>>, 
    IEqualityOperators<Vector2T<T>, Vector2T<T>, bool>,
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
    public static Vector2T<T> Zero => new Vector2T<T>(T.Zero, T.Zero);

    public static Vector2T<T> One => new Vector2T<T>(T.One, T.One);

    public static Vector2T<T> UnitX => new Vector2T<T>(T.One, T.Zero);

    public static Vector2T<T> UnitY => new Vector2T<T>(T.Zero, T.One);
    
    public readonly T X;

    public readonly T Y;

    public Vector2T(T scalar)
    {
        X = scalar;
        Y = scalar;
    }

    public Vector2T(T x, T y)
    {
        X = x;
        Y = y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Vector2T<TOther> As<TOther>() where TOther : INumber<TOther>
        => new Vector2T<TOther>(TOther.CreateChecked(X), TOther.CreateChecked(Y));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(Vector2T<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<T>.Default.Equals(Y, other.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is Vector2T<T> other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(Vector2T<T> left, Vector2T<T> right)
    {
        return left.Equals(right);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(Vector2T<T> left, Vector2T<T> right)
    {
        return !left.Equals(right);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2T<T> operator +(Vector2T<T> left, Vector2T<T> right)
    {
        return new Vector2T<T>(left.X + right.X, left.Y + right.Y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2T<T> operator -(Vector2T<T> left, Vector2T<T> right)
    {
        return new Vector2T<T>(left.X - right.X, left.Y - right.Y);
    }
    
    public static Vector2T<T> operator -(Vector2T<T> value)
        => new Vector2T<T>(-value.X, -value.Y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2T<T> operator *(Vector2T<T> left, Vector2T<T> right)
    {
        return new Vector2T<T>(left.X * right.X, left.Y * right.Y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2T<T> operator *(Vector2T<T> left, T right)
    {
        return new Vector2T<T>(left.X * right, left.Y * right);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2T<T> operator /(Vector2T<T> left, Vector2T<T> right)
    {
        return new Vector2T<T>(left.X / right.X, left.Y / right.Y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2T<T> operator /(Vector2T<T> left, T right)
    {
        return new Vector2T<T>(left.X / right, left.Y / right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        FormattableString formattable = $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        return formattable.ToString(formatProvider);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override string ToString()
    {
        return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
    }
}