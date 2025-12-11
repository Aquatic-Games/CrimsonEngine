// Copyright (c) Aquatic Games 2025. All rights reserved.
// This file is licensed to you under the MIT license.

#pragma once

#include <memory>
#include <string>

namespace Crimson
{
    struct SurfaceInfo
    {
        std::string WindowTitle = "Crimson Window";
    };

    class Surface
    {
    public:
        virtual ~Surface() = default;

        static std::unique_ptr<Surface> Create(const SurfaceInfo& info);
    };
}
