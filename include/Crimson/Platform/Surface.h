// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.

#pragma once

#include "Crimson/Math/Size.h"

#include <string>

namespace Crimson
{
    struct SurfaceInfo
    {
        Sizei Size;
        std::string Title;
    };

    class Surface
    {
    public:
        static void Create(const SurfaceInfo& info);
        static void Destroy();

        [[nodiscard]] static void* GetHandle();
    };

}
