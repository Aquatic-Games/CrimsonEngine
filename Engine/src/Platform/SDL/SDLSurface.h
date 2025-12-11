// Copyright (c) Aquatic Games 2025. All rights reserved.
// This file is licensed to you under the MIT license.

#pragma once

#include "Crimson/Platform/Surface.h"

namespace Crimson
{
    class SDLSurface : public Surface
    {
    public:
        explicit SDLSurface(const SurfaceInfo &info);
        ~SDLSurface() override;
    };
}