using System.Numerics;

namespace Crimson.Math;

public static class Matrix
{
    // https://en.wikipedia.org/wiki/Rotation_matrix#In_three_dimensions
    public static Matrix<T> RotateX<T>(T rotation) where T : INumber<T>, ITrigonometricFunctions<T>
    {
        T sinTheta = T.Sin(rotation);
        T cosTheta = T.Cos(rotation);

        return new Matrix<T>(
            Vector4T<T>.UnitX,
            new Vector4T<T>(T.Zero, cosTheta, sinTheta, T.Zero),
            new Vector4T<T>(T.Zero, -sinTheta, cosTheta, T.Zero),
            Vector4T<T>.UnitW
        );
    }
    
    public static Matrix<T> RotateY<T>(T rotation) where T : INumber<T>, ITrigonometricFunctions<T>
    {
        T sinTheta = T.Sin(rotation);
        T cosTheta = T.Cos(rotation);

        return new Matrix<T>(
            new Vector4T<T>(cosTheta, T.Zero, -sinTheta, T.Zero),
            Vector4T<T>.UnitY, 
            new Vector4T<T>(sinTheta, T.Zero, cosTheta, T.Zero),
            Vector4T<T>.UnitW
        );
    }
    
    public static Matrix<T> RotateZ<T>(T rotation) where T : INumber<T>, ITrigonometricFunctions<T>
    {
        T sinTheta = T.Sin(rotation);
        T cosTheta = T.Cos(rotation);

        return new Matrix<T>(
            new Vector4T<T>(cosTheta, sinTheta, T.Zero, T.Zero),
            new Vector4T<T>(-sinTheta, cosTheta, T.Zero, T.Zero),
            Vector4T<T>.UnitZ,
            Vector4T<T>.UnitW
        );
    }
}