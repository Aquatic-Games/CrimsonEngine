// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.
#pragma once

#include "Defs.h"

namespace Crimson
{
    template <typename T>
    struct Size
    {
        T Width;

        T Height;

        Size()
        {
            Width = 0;
            Height = 0;
        }

        explicit Size(T wh)
        {
            Width = wh;
            Height = wh;
        }

        Size(T width, T height)
        {
            Width = width;
            Height = height;
        }
    };

    using Sizei = Size<int32>;
    using Sizeu = Size<uint32>;
    using Sizef = Size<float>;
}