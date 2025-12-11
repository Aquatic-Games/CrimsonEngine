// Copyright (c) Aquatic Games 2025. All rights reserved.
// This file is licensed to you under the MIT license.

#include "Crimson/Platform/Surface.h"

#include "SDL/SDLSurface.h"

namespace Crimson
{
    std::unique_ptr<Surface> Surface::Create(const SurfaceInfo& info)
    {
        return std::make_unique<SDLSurface>(info);
    }
}
