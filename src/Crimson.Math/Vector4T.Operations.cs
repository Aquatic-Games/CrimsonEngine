using System.Numerics;
using System.Runtime.CompilerServices;

namespace Crimson.Math;

/// <summary>
/// Contains operations for a <see cref="Vector4T{T}"/>.
/// </summary>
public static class Vector4T
{
    /// <summary>
    /// Calculate the dot product of two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <typeparam name="T">A numeric type.</typeparam>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(in Vector4T<T> a, in Vector4T<T> b) where T : INumber<T>
        => a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

    /// <summary>
    /// Calculate the squared magnitude/length of a vector. This does not perform the sqrt operation.
    /// </summary>
    /// <param name="vector">The vector to calculate.</param>
    /// <typeparam name="T">A numeric type.</typeparam>
    /// <returns>The squared magnitude.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MagnitudeSquared<T>(in Vector4T<T> vector) where T : INumber<T>
        => Dot(in vector, in vector);

    /// <summary>
    /// Calculate the magnitude/length of a vector.
    /// </summary>
    /// <param name="vector">The vector to calculate.</param>
    /// <typeparam name="T">A numeric type implementing <see cref="IRootFunctions{TSelf}"/>.</typeparam>
    /// <returns>The magnitude.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Magnitude<T>(in Vector4T<T> vector) where T : INumber<T>, IRootFunctions<T>
        => T.Sqrt(MagnitudeSquared(in vector));
    
    /// <summary>
    /// Calculate the squared Euclidean distance between two vectors. This does not perform the sqrt operation.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <typeparam name="T">A numeric type.</typeparam>
    /// <returns>The squared distance.</returns>
    // https://www.geeksforgeeks.org/euclidean-distance/
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DistanceSquared<T>(in Vector4T<T> a, in Vector4T<T> b) where T : INumber<T>
        => Dot(b - a, b - a);

    /// <summary>
    /// Calculate the Euclidean distance between two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <typeparam name="T">A numeric type implementing <see cref="IRootFunctions{TSelf}"/>.</typeparam>
    /// <returns>The distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Distance<T>(in Vector4T<T> a, in Vector4T<T> b) where T : INumber<T>, IRootFunctions<T>
        => T.Sqrt(DistanceSquared(in a, in b));
}