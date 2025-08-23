// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.

#include <iostream>
#include <Crimson/Platform/Surface.h>
#include <Crimson/Graphics/Renderer.h>
#include <Crimson/Util/Logger.h>
#include <Crimson/Math/Math.h>

#include <SDL3/SDL.h>

using namespace Crimson;

int main(int argc, const char* argv[])
{
    Matrixf a = Matrixf::Identity();
    Matrixf b = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

    Matrixf c = b * a;
    std::cout << c.ToString() << std::endl;

    return 0;

    const SurfaceInfo info
    {
        .Size = { 1280, 720 },
        .Title = "Graphics Tests"
    };

    Surface::Create(info);
    Renderer::Create();

    bool alive = true;
    while (alive)
    {
        SDL_Event event;
        while (SDL_PollEvent(&event))
        {
            switch (event.type)
            {
                case SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                    alive = false;
                    break;
            }
        }

        Renderer::Present();
    }

    Renderer::Destroy();
    Surface::Destroy();

    return 0;
}