using System.Numerics;

namespace Crimson.Math;

public readonly struct Matrix<T> : 
    IEquatable<Matrix<T>>,
    IAdditionOperators<Matrix<T>, Matrix<T>, Matrix<T>>,
    ISubtractionOperators<Matrix<T>, Matrix<T>, Matrix<T>>,
    IMultiplyOperators<Matrix<T>, Matrix<T>, Matrix<T>>,
    IFormattable
    where T : INumber<T>
{
    /// <summary>
    /// Get the identity matrix.
    /// </summary>
    public static Matrix<T> Identity =>
        new Matrix<T>(Vector4T<T>.UnitX, Vector4T<T>.UnitY, Vector4T<T>.UnitZ, Vector4T<T>.UnitW);
    
    /// <summary>
    /// The first row.
    /// </summary>
    public readonly Vector4T<T> Row0;

    /// <summary>
    /// The second row.
    /// </summary>
    public readonly Vector4T<T> Row1;

    /// <summary>
    /// The third row.
    /// </summary>
    public readonly Vector4T<T> Row2;

    /// <summary>
    /// The fourth row.
    /// </summary>
    public readonly Vector4T<T> Row3;

    /// <summary>
    /// Create a <see cref="Matrix{T}"/> with 4 rows.
    /// </summary>
    /// <param name="row0">The first row.</param>
    /// <param name="row1">The second row.</param>
    /// <param name="row2">The third row.</param>
    /// <param name="row3">The fourth row.</param>
    public Matrix(Vector4T<T> row0, Vector4T<T> row1, Vector4T<T> row2, Vector4T<T> row3)
    {
        Row0 = row0;
        Row1 = row1;
        Row2 = row2;
        Row3 = row3;
    }

    /// <summary>
    /// Create a <see cref="Matrix{T}"/> from individual values.
    /// </summary>
    /// <param name="m00">The first row, first value.</param>
    /// <param name="m01">The first row, second value.</param>
    /// <param name="m02">The first row, third value.</param>
    /// <param name="m03">The first row, fourth value.</param>
    /// <param name="m10">The second row, first value.</param>
    /// <param name="m11">The second row, second value.</param>
    /// <param name="m12">The second row, third value.</param>
    /// <param name="m13">The second row, fourth value.</param>
    /// <param name="m20">The third row, first value.</param>
    /// <param name="m21">The third row, second value.</param>
    /// <param name="m22">The third row, third value.</param>
    /// <param name="m23">The third row, fourth value.</param>
    /// <param name="m30">The fourth row, first value.</param>
    /// <param name="m31">The fourth row, second value.</param>
    /// <param name="m32">The fourth row, third value.</param>
    /// <param name="m33">The fourth row, fourth value.</param>
    public Matrix(
        T m00, T m01, T m02, T m03,
        T m10, T m11, T m12, T m13,
        T m20, T m21, T m22, T m23,
        T m30, T m31, T m32, T m33
    )
    {
        Row0 = new Vector4T<T>(m00, m01, m02, m03);
        Row0 = new Vector4T<T>(m10, m11, m12, m13);
        Row0 = new Vector4T<T>(m20, m21, m22, m23);
        Row0 = new Vector4T<T>(m30, m31, m32, m33);
    }
    
    /// <summary>
    /// Get the row at the given index.
    /// </summary>
    /// <param name="index">The index to get the value.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid index is given.</exception>
    public Vector4T<T> this[int index]
    {
        get
        {
            return index switch
            {
                0 => Row0,
                1 => Row1,
                2 => Row2,
                3 => Row3,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }
    }

    public static Matrix<T> operator +(Matrix<T> left, Matrix<T> right)
        => new Matrix<T>(left.Row0 + right.Row0, left.Row1 + right.Row1, left.Row2 + right.Row2, left.Row3 + right.Row3);
    
    public static Matrix<T> operator -(Matrix<T> left, Matrix<T> right)
        => new Matrix<T>(left.Row0 - right.Row0, left.Row1 - right.Row1, left.Row2 - right.Row2, left.Row3 - right.Row3);
    
    public static Matrix<T> operator *(Matrix<T> left, Matrix<T> right)
    {
        throw new NotImplementedException();
    }

    public bool Equals(Matrix<T> other)
        => this == other;

    public override bool Equals(object? obj)
    {
        return obj is Matrix<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row0, Row1, Row2, Row3);
    }

    public static bool operator ==(Matrix<T> left, Matrix<T> right)
        => left.Row0 == right.Row0 && left.Row1 == right.Row1 && left.Row2 == right.Row2 && left.Row3 == right.Row3;

    public static bool operator !=(Matrix<T> left, Matrix<T> right)
        => left.Row0 != right.Row0 || left.Row1 != right.Row1 || left.Row2 != right.Row2 || left.Row3 != right.Row3;

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        FormattableString formattable =
            $"{nameof(Row0)}: {Row0}, {nameof(Row1)}: {Row1}, {nameof(Row2)}: {Row2}, {nameof(Row3)}: {Row3}";
        return formattable.ToString(formatProvider);
    }

    public override string ToString()
    {
        return $"{nameof(Row0)}: {Row0}, {nameof(Row1)}: {Row1}, {nameof(Row2)}: {Row2}, {nameof(Row3)}: {Row3}";
    }
}