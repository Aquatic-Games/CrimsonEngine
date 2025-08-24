// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.
#pragma once

#include <cmath>
#include <string>
#include <format>

#include "Defs.h"

namespace Crimson
{
    template<typename T>
    struct Vector2
    {
        T X;
        T Y;

        Vector2()
        {
            X = 0;
            Y = 0;
        }

        explicit Vector2(T scalar)
        {
            X = scalar;
            Y = scalar;
        }

        Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        template<typename TOther>
        [[nodiscard]] Vector2<TOther> As() const
        {
            return { static_cast<TOther>(X), static_cast<TOther>(Y) };
        }

        [[nodiscard]] T Length() const
        {
            return Magnitude(*this);
        }

        friend Vector2 operator +(const Vector2& left, const Vector2& right)
        {
            return { left.X + right.X, left.Y + right.Y };
        }

        friend Vector2 operator -(const Vector2& left, const Vector2& right)
        {
            return { left.X - right.X, left.Y - right.Y };
        }

        friend Vector2 operator -(const Vector2& vector)
        {
            return { -vector.X, -vector.Y };
        }

        friend Vector2 operator *(const Vector2& left, const Vector2& right)
        {
            return { left.X * right.X, left.Y * right.Y };
        }

        friend Vector2 operator *(const Vector2& left, T right)
        {
            return { left.X * right, left.Y * right };
        }

        friend Vector2 operator /(const Vector2& left, const Vector2& right)
        {
            return { left.X / right.X, left.Y / right.Y };
        }

        friend Vector2 operator /(const Vector2& left, T right)
        {
            return { left.X / right, left.Y / right };
        }

        friend bool operator ==(const Vector2& left, const Vector2& right)
        {
            return { left.X == right.X && left.Y == right.Y };
        }

        friend bool operator !=(const Vector2& left, const Vector2& right)
        {
            return !(left == right);
        }

        [[nodiscard]] std::string ToString() const
        {
            return std::format("X: {}, Y: {}", X, Y);
        }

        [[nodiscard]] static Vector2 Zero()
        {
            return { 0, 0 };
        }

        [[nodiscard]] static Vector2 One()
        {
            return { 1, 1 };
        }

        [[nodiscard]] static Vector2 UnitX()
        {
            return { 1, 0 };
        }

        [[nodiscard]] static Vector2 UnitY()
        {
            return { 0, 1 };
        }

        [[nodiscard]] static T Dot(const Vector2& a, const Vector2& b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        [[nodiscard]] static T MagnitudeSquared(const Vector2& vector)
        {
            return Dot(vector, vector);
        }

        [[nodiscard]] static T Magnitude(const Vector2& vector)
        {
            return std::sqrt(MagnitudeSquared(vector));
        }
    };

    using Vector2f = Vector2<float>;
    using Vector2d = Vector2<double>;
    using Vector2i = Vector2<int32>;
}
