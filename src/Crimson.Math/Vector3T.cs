using System.Numerics;

namespace Crimson.Math;

public struct Vector3T<T> where T : INumber<T>
{
    public readonly T X;

    public readonly T Y;

    public readonly T Z;

    public Vector3T(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3T(T scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
    }

    public static Vector3T<T> Zero => new Vector3T<T>(T.Zero);

    public static Vector3T<T> One => new Vector3T<T>(T.One);
}