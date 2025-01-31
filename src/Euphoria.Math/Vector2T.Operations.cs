using System.Numerics;
using System.Runtime.CompilerServices;

namespace Euphoria.Math;

public static class Vector2T
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T Dot<T>(in Vector2T<T> a, in Vector2T<T> b) where T : INumber<T>
        => a.X * b.X + a.Y * b.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T MagnitudeSquared<T>(in Vector2T<T> vector) where T : INumber<T>
        => Dot(in vector, in vector);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T Magnitude<T>(in Vector2T<T> vector) where T : INumber<T>, IRootFunctions<T>
        => T.Sqrt(MagnitudeSquared(in vector));
}