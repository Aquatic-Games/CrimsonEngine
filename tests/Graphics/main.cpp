// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.

#include <Crimson/Platform/Surface.h>

using namespace Crimson;

int main(int argc, const char* argv[])
{
    const SurfaceInfo info
    {
        .Size = { 1280, 720 },
        .Title = "Graphics Tests"
    };

    Surface::Create(info);
    Surface::Destroy();

    return 0;
}