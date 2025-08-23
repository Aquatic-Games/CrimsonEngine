// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.
#pragma once

#include <string>
#include <format>

#include "Vector4.h"

namespace Crimson
{
    template<typename T>
    struct Matrix
    {
        Vector4<T> Row0;
        Vector4<T> Row1;
        Vector4<T> Row2;
        Vector4<T> Row3;

        /// Create an empty matrix.
        Matrix()
        {
            Row0 = Vector4<T>::Zero();
            Row1 = Vector4<T>::Zero();
            Row2 = Vector4<T>::Zero();
            Row3 = Vector4<T>::Zero();
        }

        /// Create a Matrix from four row vectors.
        /// @param row0 The first vector row.
        /// @param row1 The second vector row.
        /// @param row2 The third vector row.
        /// @param row3 The fourth vector row.
        Matrix(const Vector4<T>& row0, const Vector4<T>& row1, const Vector4<T>& row2, const Vector4<T>& row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        /// Create a Matrix from 16 values.
        Matrix(T m00, T m01, T m02, T m03, T m10, T m11, T m12, T m13, T m20, T m21, T m22, T m23, T m30, T m31, T m32, T m33)
        {
            Row0 = { m00, m01, m02, m03 };
            Row1 = { m10, m11, m12, m13 };
            Row2 = { m20, m21, m22, m23 };
            Row3 = { m30, m31, m32, m33 };
        }

        [[nodiscard]] Vector4<T> Column0() const
        {
            return { Row0.X, Row1.X, Row2.X, Row3.X };
        }

        [[nodiscard]] Vector4<T> Column1() const
        {
            return { Row0.Y, Row1.Y, Row2.Y, Row3.Y };
        }

        [[nodiscard]] Vector4<T> Column2() const
        {
            return { Row0.Z, Row1.Z, Row2.Z, Row3.Z };
        }

        [[nodiscard]] Vector4<T> Column3() const
        {
            return { Row0.W, Row1.W, Row2.W, Row3.W };
        }

        friend Matrix operator +(const Matrix& left, const Matrix& right)
        {
            return
            {
                left.Row0 + right.Row0,
                left.Row1 + right.Row1,
                left.Row2 + right.Row2,
                left.Row3 + right.Row3
            };
        }

        friend Matrix operator *(const Matrix& left, const Matrix& right)
        {
            auto row0 = left.Row0;
            auto row1 = left.Row1;
            auto row2 = left.Row2;
            auto row3 = left.Row3;

            auto column0 = right.Column0();
            auto column1 = right.Column1();
            auto column2 = right.Column2();
            auto column3 = right.Column3();

            return
            {
                { Vector4<T>::Dot(row0, column0), Vector4<T>::Dot(row0, column1), Vector4<T>::Dot(row0, column2), Vector4<T>::Dot(row0, column3) },
                { Vector4<T>::Dot(row1, column0), Vector4<T>::Dot(row1, column1), Vector4<T>::Dot(row1, column2), Vector4<T>::Dot(row1, column3) },
                { Vector4<T>::Dot(row2, column0), Vector4<T>::Dot(row2, column1), Vector4<T>::Dot(row2, column2), Vector4<T>::Dot(row2, column3) },
                { Vector4<T>::Dot(row3, column0), Vector4<T>::Dot(row3, column1), Vector4<T>::Dot(row3, column2), Vector4<T>::Dot(row3, column3) }
            };
        }

        friend bool operator ==(const Matrix& left, const Matrix& right)
        {
            return left.Row0 == right.Row0 && left.Row1 == right.Row1 && left.Row2 == right.Row2 && left.Row3 == right.Row3;
        }

        friend bool operator !=(const Matrix& left, const Matrix& right)
        {
            return !(left == right);
        }

        [[nodiscard]] std::string ToString() const
        {
            return std::format("Row0: ({}), Row1: ({}), Row2: ({}), Row3: ({})",
                Row0.ToString(), Row1.ToString(),Row2.ToString(), Row3.ToString());
        }

        /// Get the Identity matrix.
        [[nodiscard]] static Matrix Identity()
        {
            return
            {
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            };
        }
    };

    using Matrixf = Matrix<float>;
    using Matrixd = Matrix<double>;
}
