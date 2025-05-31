using System.Numerics;
using System.Runtime.CompilerServices;

namespace Crimson.Math;

/// <summary>
/// Contains operations for a <see cref="Vector2T{T}"/>.
/// </summary>
public static class Vector2T
{
    /// <summary>
    /// Calculate the dot product of two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <typeparam name="T">A numeric type.</typeparam>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(in Vector2T<T> a, in Vector2T<T> b) where T : INumber<T>
        => a.X * b.X + a.Y * b.Y;

    /// <summary>
    /// Calculate the squared magnitude/length of a vector. This does not perform the sqrt operation.
    /// </summary>
    /// <param name="vector">The vector to calculate.</param>
    /// <typeparam name="T">A numeric type.</typeparam>
    /// <returns>The squared magnitude.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MagnitudeSquared<T>(in Vector2T<T> vector) where T : INumber<T>
        => Dot(in vector, in vector);

    /// <summary>
    /// Calculate the magnitude/length of a vector.
    /// </summary>
    /// <param name="vector">The vector to calculate.</param>
    /// <typeparam name="T">A numeric type.</typeparam>
    /// <returns>The magnitude.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Magnitude<T>(in Vector2T<T> vector) where T : INumber<T>, IRootFunctions<T>
        => T.Sqrt(MagnitudeSquared(in vector));

    // https://github.com/microsoft/referencesource/blob/master/System.Numerics/System/Numerics/Vector2.cs#L290-L296
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> Transform<T>(in Vector2T<T> vector, in Matrix<T> matrix) where T : INumber<T>
    {
        return new Vector2T<T>(
            vector.X * matrix.Row0.X + vector.Y * matrix.Row1.X + matrix.Row3.X,
            vector.X * matrix.Row0.Y + vector.Y * matrix.Row1.Y + matrix.Row3.Y
        );
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> TransformNormal<T>(in Vector2T<T> vector, in Matrix<T> matrix) where T : INumber<T>
    {
        return new Vector2T<T>(
            vector.X * matrix.Row0.X + vector.Y * matrix.Row1.X,
            vector.X * matrix.Row0.Y + vector.Y * matrix.Row1.Y
        );
    }
}