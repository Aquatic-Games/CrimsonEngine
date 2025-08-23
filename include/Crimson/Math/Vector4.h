// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.
#pragma once

#include <cmath>
#include <string>
#include <format>

#include "Defs.h"

namespace Crimson
{
    template<typename T>
    struct Vector4
    {
        T X;
        T Y;
        T Z;
        T W;

        Vector4()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 0;
        }

        explicit Vector4(T scalar)
        {
            X = scalar;
            Y = scalar;
            Z = scalar;
            W = scalar;
        }

        Vector4(T x, T y, T z, T w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        template<typename TOther>
        [[nodiscard]] Vector4<TOther> As() const
        {
            return { static_cast<TOther>(X), static_cast<TOther>(Y), static_cast<TOther>(Z), static_cast<TOther>(W) };
        }

        [[nodiscard]] T Length() const
        {
            return Magnitude(*this);
        }

        friend Vector4 operator +(const Vector4& left, const Vector4& right)
        {
            return { left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W };
        }

        friend Vector4 operator -(const Vector4& left, const Vector4& right)
        {
            return { left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W };
        }

        friend Vector4 operator -(const Vector4& vector)
        {
            return { -vector.X, -vector.Y, -vector.Z, -vector.W };
        }

        friend Vector4 operator *(const Vector4& left, const Vector4& right)
        {
            return { left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W };
        }

        friend Vector4 operator *(const Vector4& left, T right)
        {
            return { left.X * right, left.Y * right, left.Z * right, left.W * right };
        }

        friend Vector4 operator /(const Vector4& left, const Vector4& right)
        {
            return { left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W };
        }

        friend Vector4 operator /(const Vector4& left, T right)
        {
            return { left.X / right, left.Y / right, left.Z / right, left.W / right };
        }

        friend bool operator ==(const Vector4& left, const Vector4& right)
        {
            return { left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W };
        }

        friend bool operator !=(const Vector4& left, const Vector4& right)
        {
            return !(left == right);
        }

        [[nodiscard]] std::string ToString() const
        {
            return std::format("X: {}, Y: {}, Z: {}, W: {}", X, Y, Z, W);
        }

        static T Dot(const Vector4& a, const Vector4& b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        static T MagnitudeSquared(const Vector4& vector)
        {
            return Dot(vector, vector);
        }

        static T Magnitude(const Vector4& vector)
        {
            return std::sqrt(MagnitudeSquared(vector));
        }
    };

    using Vector4f = Vector4<float>;
    using Vector4d = Vector4<double>;
    using Vector4i = Vector4<int32>;
}
